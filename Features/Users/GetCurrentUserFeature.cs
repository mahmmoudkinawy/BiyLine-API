namespace BiyLineApi.Features.Users;
public sealed class GetCurrentUserFeature
{
    public sealed class Request : IRequest<Response> { };

    public sealed class Response
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public StoreResponse Store { get; set; }
        public List<string> Roles { get; set; }
    }

    public sealed class StoreResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Address { get; set; }
        public string CoverImageUrl { get; set; }
        public string ProfileImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserEntity, Response>();
            CreateMap<StoreEntity, StoreResponse>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var user = await _userManager.Users
                .Include(u => u.Store)
                    .ThenInclude(s => s.ProfilePictureImage)
                .Include(u => u.Store)
                    .ThenInclude(s => s.ProfileCoverImage)
                .FirstOrDefaultAsync(u => u.Id == currentUserId, cancellationToken: cancellationToken);

            var result = _mapper.Map<Response>(user);

            var baseUri = _httpContextAccessor.BaseUri(nameof(_httpContextAccessor));

            if (result.Store != null)
            {
                result.Store.ProfileImageUrl = baseUri.CombineUri(user.Store?.ProfilePictureImage?.ImageUrl);
                result.Store.CoverImageUrl = baseUri.CombineUri(user.Store?.ProfileCoverImage?.ImageUrl);
            }

            var roles = await _userManager.GetRolesAsync(user);

            result.Roles = roles.ToList();

            var acceptLanguageHeader = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

            result.Name = acceptLanguageHeader.Contains("ar") ? user.Store?.ArabicName : user.Store?.EnglishName;

            return result;
        }
    }
}
