namespace BiyLineApi.Features.Products.Queries.GetProductWarehouseLogs
{
    public class GetProductWarehouseLogsFeature
    {
        public sealed class Request : IRequest<PagedList<Response>>
        {
          
            public DateTime? From { get; set; }
            public DateTime? To { get; set; }
            public int ProductId { get; set; }
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
        }

        public sealed class Response
        {
            public WarehouseLogType Type { get; set; }
            public DateTime OperationDate { get; set; }
            public DocumentType DocumentType { get; set; }
            public double Quantity { get; set; }
            public decimal? SellingPrice { get; set; }
            public Guid Code { get; set; }

        }


       

        public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
        {
            private readonly BiyLineDbContext _context;
            private readonly IDateTimeProvider _dateTimeProvider;
            private readonly IMapper _mapper;

            public Handler(
                BiyLineDbContext context,
                IDateTimeProvider dateTimeProvider,
                IMapper mapper)
            {
                _context = context ??
                    throw new ArgumentNullException(nameof(context));
                _dateTimeProvider = dateTimeProvider ??
                    throw new ArgumentNullException(nameof(dateTimeProvider));
                _mapper = mapper ??
                    throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
             

                return await PagedList<Response>.CreateAsync(
                   _context.WarehouseLogs
                    .Where(whl => whl.ProductId == request.ProductId && (whl.OperationDate >= request.From || request.From == null) && (whl.OperationDate <= request.To || request.To == null) )
                    .GroupBy(whl => new { whl.Code, whl.DocumentType, whl.OperationDate, whl.Type })
                    .Select(group => new Response
                    {
                        Code = group.Key.Code,
                        DocumentType = group.Key.DocumentType,
                        OperationDate = group.Key.OperationDate,
                        Type = group.Key.Type,
                        Quantity = group.Sum(whl => whl.Quantity),
                        SellingPrice = group.Sum(whl => whl.SellingPrice),
                    }),
                    request.PageNumber.Value,
                    request.PageSize.Value);
            }
        }
    }
}
