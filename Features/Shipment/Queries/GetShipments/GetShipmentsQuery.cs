namespace BiyLineApi.Features.Shipment.Queries.GetShipments
{
    public class GetShipmentsQuery
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public ShipmentStatus? ShipmentStatus { get; set; }
        }

        public sealed class Response
        {
            public List<ShipmentEntity> Shipments { get; internal set; }
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
                    var shipments = await _context.Shipments.Include(s => s.ShippingCompany)
                        .Include(s => s.Warehouse)
                        .Include(s => s.PickUpPoint)
                        .Include(s => s.Governorate)
                        .Include(s => s.StoreWallet)
                        .Include(s => s.Store)
                        
                        .AsNoTracking().Where(w => w.StoreId == user.StoreId &&(w.Status == request.ShipmentStatus || request.ShipmentStatus == null)).ToListAsync();
                    if (shipments == null)
                    {
                        return Result<Response>.Failure("shipments Not Found");
                    }

                    return Result<Response>.Success(new Response { Shipments = shipments });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(ex.Message);
                }
            }
        }
    }
}
