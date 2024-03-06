namespace BiyLineApi.Features.WarehouseTransefer.Queries.GetWarehouseTransefer
{
    public class GetWarehouseTranseferQuery
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int WareHouseTranseferId { get; set; }
        }

        public sealed class Response
        {
            public int Id { get; set; }
            public int StoreId { get; set; }
            public int SourceWarehouseId { get; set; }
            public int DestinationWarehouseId { get; set; }
            public decimal TranseferCost { get; set; }
            public DateTime OperationDate { get; set; }
            public decimal TotalCost { get; set; }
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
                _context = context ??
                    throw new ArgumentNullException(nameof(context));
                _mapper = mapper ??
                    throw new ArgumentNullException(nameof(mapper));
                _httpContextAccessor = httpContextAccessor ??
                    throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.GetUserById();

                var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

                var wareHouse = await _context.WarehouseTransefers
                .FirstOrDefaultAsync(u => u.StoreId == store.Id && u.Id == request.WareHouseTranseferId, cancellationToken: cancellationToken);

                if (wareHouse == null)
                {
                    return Result<Response>.Failure(new List<string> { "Warehouse transefer not found" });
                }


                return Result<Response>.Success(new Response
                {
                    SourceWarehouseId = wareHouse.Id,
                    DestinationWarehouseId = wareHouse.Id,
                    Id = wareHouse.Id,
                    OperationDate = wareHouse.OperationDate,
                    StoreId = wareHouse.StoreId,
                    TotalCost = wareHouse.TotalCost,
                    TranseferCost = wareHouse.TranseferCost
                });
            }
        }
    }
}
