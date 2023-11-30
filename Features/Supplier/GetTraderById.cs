namespace BiyLineApi.Features.Supplier;

public sealed class GetTraderById
{
    public sealed class Request : IRequest<Result<Response>> 
    { 
            public int Id { get; set; }
    }

    public sealed class Response
    {
        public  int Id { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? CountryName { get; set; }
        public string? GovernorateName { get; set; }
        public string? RegionName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>> 
    {
        private readonly BiyLineDbContext _context;
        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                            throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var traderFromDb = await _context.Users
                .Where(u => u.StoreId != null)
                .Include(u => u.Store)
                .Where(u => u.Store.Activity != StoreActivityEnum.Sectional.ToString())
                .FirstOrDefaultAsync(s=>s.Id== request.Id);

            if (traderFromDb == null)
            {
                return Result<Response>.Failure("This Trader Not Found");
            }

            var trader = new Response
            {
                Id = traderFromDb.Id,
                Name = traderFromDb.Name,
                Address = traderFromDb.Store?.Address,
                PhoneNumber = traderFromDb.PhoneNumber,
                CountryName = traderFromDb.Store?.Country?.Name,
                GovernorateName = traderFromDb.Store?.Governorate?.Name,
                RegionName = traderFromDb.Store?.Region?.Name,
                StoreName = traderFromDb.Store?.EnglishName,
                StoreId = traderFromDb.Store.Id

            };

            return Result<Response>.Success(trader);


        }
    }
}
