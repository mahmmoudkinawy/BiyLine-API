namespace BiyLineApi.Features.Warehouses
{
    public sealed class GetWareHouseFeature
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int WareHouseId { get; set; }
        }

        public sealed class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShippingAddress { get; set; }
            public string WarehouseStatus { get; set; }
        }

        public sealed class Mapper : Profile
        {
            public Mapper()
            {
                CreateMap<WarehouseEntity, Response>();
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

                var wareHouse = await _context.Warehouses
                .FirstOrDefaultAsync(u => u.StoreId == store.Id && u.Id == request.WareHouseId, cancellationToken: cancellationToken);

                if (wareHouse == null)
                {
                    return Result<Response>.Failure(new List<string> { "Warehouse does not found" });
                }


                return Result<Response>.Success(new Response
                {
                    Id = wareHouse.Id,
                    Name = wareHouse.Name,
                    ShippingAddress = wareHouse.ShippingAddress,
                    WarehouseStatus = wareHouse.WarehouseStatus

                });
            }
        }
    }
}
