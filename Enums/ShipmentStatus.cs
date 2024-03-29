﻿namespace BiyLineApi.Enums
{
    public enum ShipmentStatus
    {
        Now = 1,
        WaitingShipping,
        ReadyToShipping,
        ShippingCompanyAcceptance,
        SendingRepresentativeToPickUp,
        ReceivingShipmentFromStore,
        InShippningCompanyWareHouse,
        DeliveredToDeliveryRepresentative,
        OnTheWay,
        Delivered
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
    public enum WarehouseLogType
    {
        In = 1,
        Out 
    }
}
