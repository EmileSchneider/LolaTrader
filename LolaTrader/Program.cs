using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Binance.Net;
using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Models;
using Binance.Net.Objects.Models.Spot.SubAccountData;
using CryptoExchange.Net.Authentication;
using Lola;
using Lola.TradeWithLimitOrders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

var testnetKey = "Lhnf4gaiIhtmNP82dHEw2DATcuIbuOGcm80KkqOHd7kwuMdcrS1RXgv7IW7NGly4";
var testnetSecret = "5RQk487XYw2f256tp6abubj5o2TjHRysc4n5Nky1jgjpceZ6rMwae3COGHTOIrOg";

var realKey = "F3CpEbSCJ4EQm753mZULTvo8oQWlp3uLgeMJ1Ae5SrSemvY2qX4dAcJTMhOXQky0";
var realSecret = "DXrUaFN8QoviJHib2o1lHikW91zRcBlR1QddKvy4vLTQlA4GGs2ZvUHKz0kc7sR5";

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services
            .AddSingleton<IOrderPlacer, BinanceOrderPlacer>()
            .AddSingleton<AutoRegressionModel, AutoRegressionModel>()
            .AddBinance((restClientOptions, socketClientOptions) =>
            {
                restClientOptions.ApiCredentials = new ApiCredentials(
                    testnetKey,
                    testnetSecret);
                restClientOptions.HttpClient = new HttpClient();
                restClientOptions.SpotApiOptions.BaseAddress = "https://testnet.binance.vision";
                restClientOptions.LogLevel = LogLevel.Trace;
            })
    )
    .Build();


var socket = host.Services.GetRequiredService<IBinanceSocketClient>();
var client = host.Services.GetRequiredService<IBinanceClient>();
var model = host.Services.GetRequiredService<AutoRegressionModel>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

var closes = await client.SpotApi.ExchangeData.GetKlinesAsync("BTCUSDT", KlineInterval.OneMinute, limit: 6);
foreach (var kline in closes.Data.SkipLast(1))
{
    model.NewClose(kline.ClosePrice);
}


var accountInfo = await client.SpotApi.Account.GetAccountInfoAsync();
if (!accountInfo.Success)
{
    Console.WriteLine(accountInfo.Error);
}

var balances = accountInfo.Data.Balances;

var BTC = balances.First(b => b.Asset == "BTC").Available;
var USDT = balances.First(b => b.Asset == "USDT").Available;

var limitOrderTrader =
    new LimitOrderTrader(
        new BinanceOrderPlacer(client, host.Services.GetRequiredService<ILogger<BinanceOrderPlacer>>()), BTC: BTC, USDT:USDT);

limitOrderTrader.NewClose(closes.Data.SkipLast(1).Last().ClosePrice);

var klineSubscription = await socket.SpotStreams.SubscribeToKlineUpdatesAsync("BTCUSDT", KlineInterval.OneMinute, async data =>
    {
        if (data.Data.Data.Final)
        {
            var signal = model.NewClose(data.Data.Data.ClosePrice);
            logger.LogInformation($"Signal: {signal}");
            await limitOrderTrader.NewSignal(signal);
            limitOrderTrader.NewClose(data.Data.Data.ClosePrice);
        }
    });

var marketTradeSubscription = await socket.SpotStreams.SubscribeToTradeUpdatesAsync("BTCUSDT", data =>
{
    limitOrderTrader.NewMarketTrade(new MarketTrade()
    {
        Price = data.Data.Price,
        Quantity = data.Data.Quantity,
        Time = data.Data.TradeTime
    });
});

var userDataStreamSubscription = await client.SpotApi.Account.StartUserStreamAsync();
var userStreamKey = userDataStreamSubscription.Data;
String message;
ConcurrentQueue<string> userDataMessageQueue = new();

var userDataSocket = new UserDataStreamSocket(userStreamKey, userDataMessageQueue);
await userDataSocket.Run();

while (true)
{
    if(userDataMessageQueue.TryDequeue(out message))
    {
        var token = JToken.Parse(message);
        if (token["e"].ToString().Equals("executionReport"))
        {
            Console.WriteLine(token.ToString());
            limitOrderTrader.NewOrderUpdate(new OrderFillUpdate()
            {
                OrderId = long.Parse(token["i"].ToString()),
               TotalFill = decimal.Parse(token["z"].ToString())
            });
        }
    }
}

await host.RunAsync();