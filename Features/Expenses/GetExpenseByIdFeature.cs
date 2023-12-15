namespace BiyLineApi.Features.Expenses;
public sealed class GetExpenseByIdFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ExpenseId { get; set; }
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

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

        public async Task<Result<Response>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen a store" });
            }

            var expense = await _context.Expenses
                .Include(e => e.ExpenseType)
                .FirstOrDefaultAsync(e => e.Id == request.ExpenseId && e.StoreId == store.Id, cancellationToken: cancellationToken);

            if (expense is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen this expense" });
            }

            return Result<Response>.Success(_mapper.Map<Response>(expense));
        }
    }
}
