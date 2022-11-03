using System.Diagnostics;
using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace Lola;

public class Trader : ITrader
{
    public decimal BTC { get; set; }
    public decimal USDT { get; set; }

    protected TradeSignal? previousSignal;

    protected ILogger<Trader> _logger;
    protected IOrderPlacer _orderPlacer;

    public Trader(ILogger<Trader> logger, IOrderPlacer orderPlacer)
    {
        _orderPlacer = orderPlacer;
        _logger = logger;
    }

    async public Task NewTradeSignal(TradeSignal tradeSignal)
    {
        _logger.LogInformation($"PrevSig: {previousSignal} CurSig: {tradeSignal} BTC: {BTC} USDT: {USDT}");
        if (previousSignal == TradeSignal.LONG)
        {
            _logger.LogInformation($"EXIT TRADE LONG {BTC}");
            if (BTC > 0.005m)
            {
                _logger.LogInformation($"EXITING BTC: {BTC}");
                var exitOrder = await _orderPlacer.SellMarket("BTCUSDT", BTC);
                Thread.Sleep(50);
            }
        }

        if (previousSignal == TradeSignal.SHORT)
        {
            _logger.LogInformation($"EXIT TRADE SHORT {USDT}");
            if (USDT > 10)
            {
                _logger.LogInformation($"EXITING USDT: {USDT}");
                var exitOrder = await _orderPlacer.BuyMarket("BTCUSDT", quoteQuantity: USDT);
                Thread.Sleep(50);
            }
        }

        if (tradeSignal == TradeSignal.LONG && previousSignal != TradeSignal.SHORT)
        {
            if (USDT > 10)
            {
                _logger.LogInformation($"LONG BTC: {BTC} USDT: {USDT}");
                var entryOrder = await _orderPlacer.BuyMarket("BTCUSDT", quoteQuantity: USDT);
            }
        }

        if (tradeSignal == TradeSignal.SHORT && previousSignal != TradeSignal.LONG)
        {
            if (BTC > 0.005m)
            {
                _logger.LogInformation($"SHORT BTC: {BTC} USDT: {USDT}");
                var entryOrder = await _orderPlacer.SellMarket("BTCUSDT", BTC);
            }
        }

        previousSignal = tradeSignal;
    }
}