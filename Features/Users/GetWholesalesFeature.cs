namespace BiyLineApi.Features.Users;
public sealed class GetWholesalesFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }
        public int? StoreId { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserEntity, Response>()
                .ForMember(dest => dest.StoreId, src => src.MapFrom(opt => opt.Store.Id));
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IMapper _mapper;

        public Handler(BiyLineDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .Include(u => u.Store)
                .Where(u => u.UserRoles.Any(r => r.Role.Name == Constants.Roles.Trader && r.User.Store.Activity == StoreActivityEnum.Wholesale.ToString()))
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsQueryable(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
