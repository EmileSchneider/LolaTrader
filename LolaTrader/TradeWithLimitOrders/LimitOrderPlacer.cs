namespace Lola.TradeWithLimitOrders.Stated;

public class LimitOrderPlacer
{
    public decimal USDT { get; set; }
    public decimal BTC { get; set; }
    public MarketTrade LastTrade { get; set; }
    public decimal LastClose { get; set; }
    private IOrderPlacer _orderPlacer;

    public LimitOrderPlacer(IOrderPlacer orderPlacer, decimal usdt, decimal btc)
    {
        _orderPlacer = orderPlacer;
        USDT = usdt;
        BTC = btc;
    }

    async public Task<PlacedOrder> PlaceOrderLong()
    {
        if (USDT < 10m)
            throw new Exception("Not enough USDT to buy");

        return await _orderPlacer.BuyLimit("BTCUSDT", limitPrice: LastClose, quantity: USDT / LastClose);
    }

    async public Task<PlacedOrder> PlaceOrderShort()
    {
        if (BTC <= 0.005m)
            throw new Exception("Not enough BTC to sell");
        
        return await _orderPlacer.SellLimit("BTCUSDT", limitPrice: LastClose, quantity: BTC);
    }
}