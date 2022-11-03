namespace Lola;

public interface ITrader
{
    public decimal BTC { get; set; }
    public decimal USDT { get; set; }
    public Task NewTradeSignal(TradeSignal tradeSignal);
}