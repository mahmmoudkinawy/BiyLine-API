namespace BiyLineApi.Features.WarehouseTransefer.Queries.GetWarehouseTranseferes
{
    public class GetWarehouseTranseferesQuery
    {
        public sealed class Request : IRequest<PagedList<Response>>
        {
            public string? Predicate { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
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

       

        public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
        {
            private readonly BiyLineDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(
                BiyLineDbContext context,
     
                IHttpContextAccessor httpContextAccessor)
            {
                _context = context ??
                    throw new ArgumentNullException(nameof(context));
               
                _httpContextAccessor = httpContextAccessor ??
                    throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = _httpContextAccessor.GetUserById();
                var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

                var query = _context.WarehouseTransefers
                    .Where(w => w.StoreId == store.Id)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Predicate))
                {
                    query = query.Where(w => w.Id.ToString().Contains(request.Predicate));
                }
               
                return await PagedList<Response>.CreateAsync(
                     query.Select(w => new Response
                     {
                         Id =  w.Id,
                         DestinationWarehouseId = w.DestinationWarehouseId,
                         OperationDate = w.OperationDate,
                         SourceWarehouseId = w.SourceWarehouseId,
                         StoreId = w.StoreId,
                         TotalCost = w.TotalCost,
                         TranseferCost = w.TranseferCost
                        
                     }).AsNoTracking(),
                    request.PageNumber,
                    request.PageSize);
            }
        }
    }
}
