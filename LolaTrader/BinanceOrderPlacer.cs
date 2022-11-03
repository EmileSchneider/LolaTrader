using Binance.Net.Enums;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Objects.Models.Spot;
using CryptoExchange.Net.Objects;
using Microsoft.Extensions.Logging;

namespace Lola;

public class BinanceOrderPlacer : IOrderPlacer
{
    private IBinanceClient _binanceClient;
    private ILogger<BinanceOrderPlacer> _logger;

    public BinanceOrderPlacer(IBinanceClient binanceClient, ILogger<BinanceOrderPlacer> logger)
    {
        _binanceClient = binanceClient;
        _logger = logger;
    }

    async public Task<PlacedOrder> BuyMarket(string symbol, decimal quantity = 0, decimal quoteQuantity = 0)
    {
        _logger.LogInformation($"Placing Buy Market Order: {symbol} {quantity} {quoteQuantity}");
        WebCallResult<BinancePlacedOrder> binancePlacedOrder;

        if (quantity == 0)
        {
            binancePlacedOrder = await _binanceClient.SpotApi.Trading.PlaceOrderAsync(symbol, OrderSide.Buy,
                SpotOrderType.Market,
                quoteQuantity: Math.Round(quoteQuantity, 2, MidpointRounding.ToNegativeInfinity));
            if (!binancePlacedOrder.Success)
            {
                _logger.LogInformation($"{binancePlacedOrder.Error}");
            }
        }
        else
        {
            binancePlacedOrder = await _binanceClient.SpotApi.Trading.PlaceOrderAsync(symbol, OrderSide.Buy,
                SpotOrderType.Market,
                quantity: Math.Round(quantity, 5, MidpointRounding.ToNegativeInfinity) - 0.00001m);
            if (!binancePlacedOrder.Success)
            {
                _logger.LogInformation($"{binancePlacedOrder.Error}");
            }
        }

        return new PlacedOrder()
        {
            OrderId = binancePlacedOrder.Data.Id,
            Price = binancePlacedOrder.Data.Price,
            Quantity = binancePlacedOrder.Data.Quantity,
            Side = TradeSide.LONG
        };
    }

    async public Task<PlacedOrder> SellMarket(string symbol, decimal quantity)
    {
        _logger.LogInformation($"Placing Sell Market Order: {symbol} {quantity}");
        var binancePlacedOrder = await _binanceClient.SpotApi.Trading.PlaceOrderAsync(symbol, OrderSide.Sell,
            SpotOrderType.Market, quantity: Math.Round(quantity, 5, MidpointRounding.ToNegativeInfinity) - 0.00001m);

        if (!binancePlacedOrder.Success)
        {
            _logger.LogInformation($"{binancePlacedOrder.Error}");
        }

        return new PlacedOrder()
        {
            OrderId = binancePlacedOrder.Data.Id,
            Price = binancePlacedOrder.Data.Price,
            Quantity = binancePlacedOrder.Data.Quantity,
            Side = TradeSide.LONG
        };
    }

    async public Task<PlacedOrder> BuyLimit(string symbol, decimal limitPrice, decimal quantity = 0,
        decimal quoteQuantity = 0)
    {
        BinancePlacedOrder binancePlacedOrder;

        if (quantity == 0)
        {
            var res = await _binanceClient.SpotApi.Trading.PlaceOrderAsync(symbol, OrderSide.Buy,
                SpotOrderType.Limit,
                price: limitPrice,
                timeInForce: TimeInForce.GoodTillCanceled,
                quoteQuantity: Math.Round(quoteQuantity, 2, MidpointRounding.ToNegativeInfinity));
            if (!res.Success)
            {
                _logger.LogInformation($"{res.Error}");
            }

            binancePlacedOrder = res.Data;
        }
        else
        {
            var res = await _binanceClient.SpotApi.Trading.PlaceOrderAsync(symbol, OrderSide.Buy,
                SpotOrderType.Limit,
                price: limitPrice,
                timeInForce: TimeInForce.GoodTillCanceled,
                quantity: Math.Round(quantity, 5, MidpointRounding.ToNegativeInfinity));
            binancePlacedOrder = res.Data;
            if (!res.Success)
            {
                _logger.LogInformation($"{res.Error}");
            }
        }

        return new PlacedOrder()
        {
            OrderId = binancePlacedOrder.Id,
            Price = binancePlacedOrder.Price,
            Quantity = binancePlacedOrder.Quantity,
            Side = TradeSide.LONG
        };
    }

    async public Task<PlacedOrder> SellLimit(string symbol, decimal quantity, decimal limitPrice)
    {
        _logger.LogInformation($"Placing Sell Limit Order: {symbol} {quantity} {limitPrice}");
        var binancePlacedOrder = await _binanceClient.SpotApi.Trading.PlaceOrderAsync(symbol, OrderSide.Sell,
            SpotOrderType.Limit, price: limitPrice, timeInForce: TimeInForce.GoodTillCanceled,
            quantity: Math.Round(quantity, 5, MidpointRounding.ToNegativeInfinity) - 0.00001m);

        if (!binancePlacedOrder.Success)
        {
            _logger.LogInformation($"{binancePlacedOrder.Error}");
        }

        return new PlacedOrder()
        {
            OrderId = binancePlacedOrder.Data.Id,
            Price = binancePlacedOrder.Data.Price,
            Quantity = binancePlacedOrder.Data.Quantity,
            Side = TradeSide.LONG
        };
    }
}