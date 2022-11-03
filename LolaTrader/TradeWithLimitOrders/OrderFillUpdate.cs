namespace Lola.TradeWithLimitOrders;

public class OrderFillUpdate
{
    public long OrderId { get; set; }
    public decimal FillQuantity { get; set; }
    public decimal TotalFill { get; set; }
    public decimal Quantity { get; set; }
}
