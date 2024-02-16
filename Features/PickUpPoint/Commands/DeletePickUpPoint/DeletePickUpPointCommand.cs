namespace BiyLineApi.Features.PickUpPoint.Commands.DeletePickUpPoint
{
    public class DeletePickUpPointCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }
            public int GovernorateId { get; set; }
            public string Address { get; set; }
        }

        public sealed class Response
        {
        }

        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator(IStringLocalizer<CommonResources> localizer, BiyLineDbContext context)
            {
                RuleFor(r => r.Address)
                    .NotEmpty()
                    .MinimumLength(2)
                    .MaximumLength(255);
            }


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

                    var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == user.StoreId);
                    if (store == null)
                    {
                        return Result<Response>.Failure("Store Not Found");
                    }
                    var pickUpPoint = await _context.PickUpPoints.FirstOrDefaultAsync(p => p.Id == request.Id);
                    if (pickUpPoint == null)
                    {
                        return Result<Response>.Failure("PickUp Point Not Found");
                    }
                  

                    _context.PickUpPoints.Remove(pickUpPoint);
                    await _context.SaveChangesAsync();



                    return Result<Response>.Success(new Response { });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(ex.Message);
                }
            }
        }
    }
}
