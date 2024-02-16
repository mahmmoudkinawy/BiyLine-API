using System.ComponentModel.DataAnnotations.Schema;
using static BiyLineApi.Features.Shipment.Commands.CreateShipment.CreateShipmentCommand;

namespace BiyLineApi.Features.Shipment.Commands.CreateShipment
{
    public class CreateShipmentCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int WarehouseId { get; set; }
            public string? ClientName { get; set; }
            public string? ClientPhone { get; set; }
            public double? TotalDiscountPercentage { get; set; }
            public double? ValueAddedTax { get; set; }
            public CashOutType CashOutType { get; set; }
            public ShipmentStatus Status { get; set; }
            public CollectingShipmentCost CollectingShipmentCost { get; set; }
            public CollectingDeliveryCost CollectingDeliveryCost { get; set; }

            public int GovernorateId { get; set; }

            public string DetailedAddress { get; set; }
            public double ShippingCost { get; set; }
            public int? ShippingCompanyId { get; set; }
            public int? PickUpPointId { get; set; }

            public PaymentStatus? PaymentStatus { get; set; }

            public int? StoreWalletId { get; set; }

            public double PaidAmount { get; set; } = 0;


            public ICollection<ShipmentDetailsVM> ShipmentDetails { get; set; } = new List<ShipmentDetailsVM>();
        }
        public class ShipmentDetailsVM
        {
            public int ShipmentId { get; set; }
            public int ProductId { get; set; }
            public double UnitCost { get; set; }
            public double DiscountPercentage { get; set; }
            public double Quantity { get; set; }
            public int ProductVariationId { get; internal set; }
        }
        public sealed class Response
        {
            public ShipmentEntity Shipment { get; internal set; }
        }
        public sealed class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly BiyLineDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<UserEntity> _userManager;
            private readonly IImageService _imageService;
            private readonly IDateTimeProvider _dateTimeProvider;

            public Handler(
                BiyLineDbContext context,
                UserManager<UserEntity> userManager,
                            IImageService imageService,
                                        IDateTimeProvider dateTimeProvider,

                IHttpContextAccessor httpContextAccessor)
            {
                _userManager = userManager ??
        throw new ArgumentNullException(nameof(userManager));
                _context = context ??
                    throw new ArgumentNullException(nameof(context));
                _httpContextAccessor = httpContextAccessor ??
                    throw new ArgumentNullException(nameof(httpContextAccessor));
                _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
                _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var userId = _httpContextAccessor.GetUserById();
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user == null)
                    {
                        return Result<Response>.Failure("User Not Found");
                    }
                    var governorate = await _context.Governments.FirstOrDefaultAsync(g => g.Id == request.GovernorateId);
                    if (governorate == null)
                    {
                        return Result<Response>.Failure("Governorate Not Found");
                    }

                    var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == user.StoreId);
                    if (store == null)
                    {
                        return Result<Response>.Failure("Store Not Found");
                    }
                    if(request.PickUpPointId.HasValue || request.ShippingCompanyId.HasValue)
                    {
                        var pickUpPoint = await _context.PickUpPoints.FirstOrDefaultAsync(p => p.Id == request.PickUpPointId);
                        if (pickUpPoint == null)
                        {
                            return Result<Response>.Failure("PickUp Point Not Found");
                        }
                        var shippingCompany = await _context.ShippingCompanies.FirstOrDefaultAsync(sc => sc.Id == request.ShippingCompanyId);
                        if (shippingCompany == null)
                        {
                            return Result<Response>.Failure("Shipping Company Not Found");
                        }
                    }
                   
                    var storeWallet = await _context.StoreWallets.FirstOrDefaultAsync(sw => sw.Id == request.StoreWalletId);
                    if (storeWallet == null)
                    {
                        return Result<Response>.Failure("Store Wallet Not Found");
                    }
                    
                    var wareHouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.WarehouseId);
                    if (wareHouse == null)
                    {
                        return Result<Response>.Failure("wareHouse Not Found");
                    }
                    if (request.CashOutType == CashOutType.ReadyToShipping)
                    {
                        if (!(request.PickUpPointId.HasValue && request.ShippingCompanyId.HasValue))
                        {
                            return Result<Response>.Failure("Shipping Company Or PickUp  Not Found");
                        }
                    }
                    var shipment = new ShipmentEntity
                    {
                        ClientName = request.ClientName,
                        DetailedAddress = request.DetailedAddress,
                        GovernorateId = request.GovernorateId,
                        ClientPhone = request.ClientPhone,
                        CollectingDeliveryCost = request.CollectingDeliveryCost,
                        CollectingShipmentCost = request.CollectingShipmentCost,
                        CreatedDate = _dateTimeProvider.GetCurrentDateTimeUtc(),
                        PaidAmount = request.PaidAmount,
                        PaymentStatus = request.PaymentStatus,
                        PickUpPointId = request.PickUpPointId,
                        ShippingCompanyId = request.ShippingCompanyId,
                        ShippingCost = request.ShippingCost,
                        Status = (ShipmentStatus)request.CashOutType,
                        StoreId = store.Id,
                        StoreWalletId = request.StoreWalletId,
                        TotalDiscountPercentage = request.TotalDiscountPercentage,
                        WarehouseId = request.WarehouseId,
                        ValueAddedTax = request.ValueAddedTax,
                        CashOutType = request.CashOutType
                    };
                   
                    _context.Shipments.Add(shipment);
                    await _context.SaveChangesAsync();
                    var details = new List<ShipmentDetailsEntity>();
                    Parallel.ForEach(request.ShipmentDetails, shipmentDetail =>
                    {
                        var shipmentDetails = new ShipmentDetailsEntity
                        {
                            DiscountPercentage = shipmentDetail.DiscountPercentage,
                            ProductId = shipmentDetail.ProductId,
                            ShipmentId = shipment.Id,
                            ProductVariationId = shipmentDetail.ProductVariationId,
                            Quantity = shipmentDetail.Quantity,
                            UnitCost = shipmentDetail.UnitCost
                        };
                        details.Add(shipmentDetails);
                    });
                    _context.ShipmentDetails.AddRange(details);
                    await _context.SaveChangesAsync();


                    // صرف من المخزن في حالة صرف الان
                    if(shipment.CashOutType == CashOutType.Now)
                    {
                        var warehouseLogs = new List<WarehouseLogEntity>();
                        Parallel.ForEach(request.ShipmentDetails, shipmentDetail =>
                        {
                            var warehouselog = new WarehouseLogEntity
                            {
                                ProductId = shipmentDetail.ProductId,
                                ProductVariationId = shipmentDetail.ProductVariationId,
                                Quantity = shipmentDetail.Quantity,
                                WarehouseId = shipment.WarehouseId,
                                Type = WarehouseLogType.Out,
                                DocumentType = DocumentType.Shipment,
                                DocumentId = shipment.Id
                            };
                            warehouseLogs.Add(warehouselog);

                        });
                       _context.WarehouseLogs.AddRange(warehouseLogs);
                    }
                    // دفع للخزنة ف حالة مدفوع
                    if(shipment.PaymentStatus == PaymentStatus.Paid)
                    {
                        var storeWalletLog = new StoreWalletLogs
                        {
                            DocumentId = shipment.Id,
                            DocumentType = DocumentType.Shipment,
                            EmpId = user.Id,
                            LogStatus = StoreWalletLogType.In,
                            StoreWalletId = storeWallet.Id,
                            Value = (decimal?)shipment.TotalCostAfterDiscount
                        };
                        _context.StoreWalletLogs.Add(storeWalletLog);
                    }
                    //في حالة دفع جزء من الفاتورة
                    else if(shipment.PaymentStatus == PaymentStatus.UnPaid && shipment.PaidAmount != 0)
                    {
                        var storeWalletLog = new StoreWalletLogs
                        {
                            DocumentId = shipment.Id,
                            DocumentType = DocumentType.Shipment,
                            EmpId = user.Id,
                            LogStatus = StoreWalletLogType.In,
                            StoreWalletId = storeWallet.Id,
                            Value = (decimal?)shipment.PaidAmount
                        };
                        _context.StoreWalletLogs.Add(storeWalletLog);
                    }
                    await _context.SaveChangesAsync();

                    return Result<Response>.Success(new Response { Shipment = shipment });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(ex.Message);
                }
            }
        }
    }
}
