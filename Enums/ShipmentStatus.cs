namespace BiyLineApi.Enums
{
    public enum ShipmentStatus
    {
        Delivered = 0,
    }
    public enum CashOutType
    {
        Now = 1,
        WaitingShipping,
        ReadyToShipping
    }
    public enum CollectingShipmentCost
    {
        ByMe=1,
        ByShippingCompany
    }
    public enum CollectingDeliveryCost
    {
        ByTrader = 1,
        ByClient
    }
    public enum PaymentStatus
    {
        Paid =1,
        UnPaid
    }
}
