using Binance.Net.Objects.Models.Spot.Socket;

namespace Lola.TradeWithLimitOrders;


public class MarketTrade
{
    public DateTime Time { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
}