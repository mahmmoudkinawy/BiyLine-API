using System.ComponentModel.DataAnnotations;

namespace BiyLineApi.Features.ShippingCompanies
{
    public sealed class GetShippingCompanyGovernorateDetailsFeature
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            [Required(ErrorMessage ="Shipping Company Id is required")]
            public int ShippingCompanyId { get; set; }
        }

        public sealed class Response
        {
            public List<ShippingCompanyGovernorateDetailsEntity> ShippingCompanyGovernorateDetails { get; internal set; }
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
                    var shippingCompany = await _context.ShippingCompanies.FirstOrDefaultAsync(s => s.Id == request.ShippingCompanyId);

                    if (shippingCompany == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company not found" });
                    }
                   
                    var shippingCompanyGovernorateDetails = await _context.ShippingCompanyGovernorateDetails.Where(s => s.ShippingCompanyId == request.ShippingCompanyId).AsNoTracking().ToListAsync();
                    

                    return Result<Response>.Success(new Response { ShippingCompanyGovernorateDetails = shippingCompanyGovernorateDetails });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }
            }
        }
    }
}
