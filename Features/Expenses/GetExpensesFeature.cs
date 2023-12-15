namespace BiyLineApi.Features.Expenses;
public sealed class GetExpensesFeature
{
    public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
    {
        public DateTime? Date { get; set; }
        public string? Wallet { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Wallet { get; set; }
        public decimal Amount { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ExpenseEntity, Response>()
                .ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.StoreWallet.Name));
        }
    }

    public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen a store" });
            }

            var query = _context.Expenses
                .Where(e => e.StoreId == store.Id)
                .OrderBy(e => e.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Wallet))
            {
                query = query.Where(e => e.StoreWallet.Name.Contains(request.Wallet));
            }

            if (request.Date != null)
            {
                query = query.Where(e => e.Date.Value.Month == request.Date.Value.Month);
            }

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
