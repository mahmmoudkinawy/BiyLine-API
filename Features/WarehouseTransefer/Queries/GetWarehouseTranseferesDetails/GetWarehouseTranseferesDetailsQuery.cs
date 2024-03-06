namespace BiyLineApi.Features.WarehouseTransefer.Queries.GetWarehouseTranseferes
{
    public class GetWarehouseTranseferesDetailsQuery
    {
        public sealed class Request : IRequest<PagedList<Response>>
        {
            public int WarehouseTranseferId { get; set; }
            public string? Predicate { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
        }

        public sealed class Response
        {
            public int Id { get; set; }
            public int WarehouseTranseferId { get; set; }
            public int ProductVariationId { get; set; }


            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal TotalPrice { get; set; }

        }

       

        public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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

            public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.GetUserById();


                var query = _context.WarehouseTranseferDetails
                    .Where(w => w.WarehouseTranseferId == request.WarehouseTranseferId)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Predicate))
                {
                    query = query.Where(w => w.Id.ToString().Contains(request.Predicate));
                }
               
                return await PagedList<Response>.CreateAsync(
                     query.Select(w => new Response
                     {
                         Id =  w.Id,
                         WarehouseTranseferId = w.Id,
                         ProductId = w.ProductId,
                         ProductVariationId = w.ProductVariationId,
                         Quantity = w.Quantity,
                         TotalPrice = w.TotalPrice,
                         UnitPrice = w.UnitPrice
                     }).AsNoTracking(),
                    request.PageNumber,
                    request.PageSize);
            }
        }
    }
}
