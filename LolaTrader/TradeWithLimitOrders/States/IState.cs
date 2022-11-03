namespace Lola.TradeWithLimitOrders.Stated;

public interface IState
{
    public Task<IState> NewSignal(TradeSignal signal);
    public IState NewMarketTrade(MarketTrade trade);
    public IState Fill(OrderFillUpdate orderFillUpdate);
}