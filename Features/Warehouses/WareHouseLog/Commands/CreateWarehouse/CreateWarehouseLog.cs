namespace BiyLineApi.Features.Warehouses.WareHouseLog.Commands.CreateWarehouse
{
    public class CreateWarehouseLog
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }
            public int WarehouseId { get; set; }
            public double Quantity { get; set; }
            public decimal? SellingPrice { get; set; }
            public int ProductId { get; set; }
            public WarehouseLogType Type { get; set; }
            public int ProductVariationId { get; set; }
            public ProductVariationEntity ProductVariation { get; set; }
            public int DocumentId { get; set; }
            public DocumentType DocumentType { get; set; }
        }

        public sealed class Response
        {
            public WarehouseLogEntity Log { get; internal set; }
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
                    var wareHouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == request.WarehouseId);
                    if (wareHouse == null)
                    {
                        return Result<Response>.Failure("wareHouse Not Found");
                    }
                    var warehouselog = new WarehouseLogEntity
                    {
                        ProductId = request.ProductId,
                        ProductVariationId = request.ProductVariationId,
                        Quantity = request.Quantity,
                        WarehouseId = request.WarehouseId,
                        Type = request.Type,
                        DocumentType = request.DocumentType,
                        DocumentId = request.DocumentId,
                        Code = Guid.NewGuid(),
                        OperationDate = _dateTimeProvider.GetCurrentDateTimeUtc(),
                        SellingPrice = request.SellingPrice
                    };
                    _context.WarehouseLogs.Add(warehouselog);
                    await _context.SaveChangesAsync();



                    return Result<Response>.Success(new Response { Log = warehouselog});
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(ex.Message);
                }
            }
        }
    }
}
