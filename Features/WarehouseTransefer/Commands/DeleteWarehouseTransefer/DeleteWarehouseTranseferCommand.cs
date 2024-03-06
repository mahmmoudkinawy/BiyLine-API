namespace BiyLineApi.Features.WarehouseTransefer.Commands.DeleteWarehouseTransefer
{
    public class DeleteWarehouseTranseferCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int WareHouseTranseferId { get; set; }
        }

        public sealed class Response
        {
           
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
                try
                {
                    var userId = _httpContextAccessor.GetUserById();

                    var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

                    var wareHouse = await _context.WarehouseTransefers
                    .FirstOrDefaultAsync(u => u.StoreId == store.Id && u.Id == request.WareHouseTranseferId, cancellationToken: cancellationToken);

                    if (wareHouse == null)
                    {
                        return Result<Response>.Failure(new List<string> { "Warehouse transefer not found" });
                    }

                    var transeferDetails = await _context.WarehouseTranseferDetails.Where(td => td.WarehouseTranseferId == request.WareHouseTranseferId).ToListAsync();
                    _context.WarehouseTranseferDetails.RemoveRange(transeferDetails);

                    _context.WarehouseTransefers.Remove(wareHouse);

                    await _context.SaveChangesAsync();
                    return Result<Response>.Success(new Response
                    {

                    });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }
                
            }
        }
    }
}
