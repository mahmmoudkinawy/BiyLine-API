
namespace BiyLineApi.Features.Warehouses.WareHouseLog.Queries.GetWarehouseLog
{
    public class GetWarehouseLogs
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int DocumentId { get; set; }
            public DocumentType DocumentType { get; set; }
        }

        public sealed class Response
        {
            public List<WarehouseLogEntity> Logs { get; internal set; }
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
                    var wareHouselogs = await _context.WarehouseLogs.AsNoTracking().Where(w => w.DocumentId == request.DocumentId && w.DocumentType == request.DocumentType).ToListAsync();
                    if (wareHouselogs == null)
                    {
                        return Result<Response>.Failure("wareHouse Not Found");
                    }

                    return Result<Response>.Success(new Response { Logs = wareHouselogs });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(ex.Message);
                }
            }
        }
    }
}
