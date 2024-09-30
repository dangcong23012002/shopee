public class SellerViewModel
{
    public Status Status { get; set; }
    public int SellerID { get; set; }
    public string SellerUsername { get; set; }
    public string HtmlOrdersWaitSettlementItem { get; set; }
    public string HtmlOrdersWaitPickupItem { get; set; }
    public string HtmlOrdersProcessedItem { get; set; }
    public IEnumerable<Order> OrdersWaitSettlement { get; set; }
    public IEnumerable<Order> OrdersWaitPickup { get; set; }
    public IEnumerable<Order> OrdersWaitDelivery { get; set; }
    public IEnumerable<Order> OrdersProcessed { get; set; }
    public IEnumerable<OrderDetail> OrderDetailsWaitDelivery { get; set; }
    public IEnumerable<SellerInfo> SellerInfos { get; set; }
    public IEnumerable<ShippingOrder> ShippingOrders { get; set; }
    public IEnumerable<Address> DeliveryAddresses { get; set; }
}