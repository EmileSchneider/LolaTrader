using Microsoft.Extensions.Logging;

namespace Lola;

public class SignalReversalTrader : ITrader
{
    private TradeSignal _previousSignal;
    private ILogger<SignalReversalTrader> _logger;
    private IOrderPlacer _orderPlacer;

    public decimal BTC { get; set; }
    public decimal USDT { get; set; }

    public SignalReversalTrader(ILogger<SignalReversalTrader> logger, IOrderPlacer orderPlacer)
    {
        _logger = logger;
        _orderPlacer = orderPlacer;
    }

    async public Task NewTradeSignal(TradeSignal tradeSignal)
    {
        _logger.LogInformation($"CURRENT HOLDINGS BTC: {BTC} USDT: {USDT}");
        if (_previousSignal != tradeSignal)
        {
            if (tradeSignal == TradeSignal.LONG)
            {
                _logger.LogInformation($"TRADE LONG {USDT}");
                await _orderPlacer.BuyMarket("BTCUSDT", quoteQuantity: USDT);
            }

            if (tradeSignal == TradeSignal.SHORT)
            {
                _logger.LogInformation($"TRADE SHORT {BTC}");
                await _orderPlacer.SellMarket("BTCUSDT", BTC);
            }
        }

        _previousSignal = tradeSignal;
    }
}