namespace Lola;

public interface IOrderPlacer
{
    public Task<PlacedOrder> BuyMarket(string symbol, decimal quantity = 0m, decimal quoteQuantity = 0m);
    public Task<PlacedOrder> SellMarket(string symbol, decimal quantity);

    public Task<PlacedOrder> BuyLimit(string symbol, decimal limitPrice, decimal quantity = 0m,
        decimal quoteQuantity = 0m);

    public Task<PlacedOrder> SellLimit(string symbol, decimal quantity, decimal limitPrice);
}

public class PlacedOrder
{
    public long OrderId { get; set; }
    public TradeSide Side { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
}

public class OrderUpdate
{
    public long TradeId { get; set; }
    public TradeSide Side { get; set; }
    public decimal MarketPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal FillQuantity { get; set; }
    public decimal Unfilled { get; set; }
    public decimal LastPriceFilled { get; set; }
    public decimal Fee { get; set; }
}

public enum TradeSide
{
    LONG,
    SHORT
}