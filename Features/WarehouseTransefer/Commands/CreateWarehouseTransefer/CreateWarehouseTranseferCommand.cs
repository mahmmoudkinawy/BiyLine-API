namespace BiyLineApi.Features.WarehouseTransefer.Commands.CreateWarehouseTransefer
{
    public record CreateWarehouseTranseferCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int SourceWarehouseId { get; set; }
            public int DestinationWarehouseId { get; set; }
            public decimal TranseferCost { get; set; }
            public List<TranseferDetails> Details { get; set; } = new List<TranseferDetails>();
        }
        public class TranseferDetails
        {
            public int ProductVariationId { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public int ProductId { get; set; }
        }
        public sealed class Response { }



        public sealed class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly BiyLineDbContext _context;

            public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
            {
                _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                using (var trans = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var userId = _httpContextAccessor.GetUserById();

                        var store = await _context.Stores
                            .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

                        if (store is null)
                        {
                            return Result<Response>.Failure(new List<string> { "Current user does not owen store." });
                        }

                        if (!await _context.Warehouses
                            .AnyAsync(w => w.StoreId == store.Id && w.Id == request.SourceWarehouseId, cancellationToken: cancellationToken))
                        {
                            return Result<Response>.Failure(new List<string> { "source Warehouse Not Found" });
                        }


                        if (!await _context.Warehouses
                            .AnyAsync(w => w.StoreId == store.Id && w.Id == request.DestinationWarehouseId, cancellationToken: cancellationToken))
                        {
                            return Result<Response>.Failure(new List<string> { "Destination Warehouse Not Found" });
                        }
                       
                        var warehouseTranseferToCreate = new WarehouseTranseferEntity
                        {
                            DestinationWarehouseId = request.DestinationWarehouseId,
                            OperationDate = DateTime.UtcNow,
                            SourceWarehouseId = request.SourceWarehouseId,
                            TranseferCost = request.TranseferCost,
                        };
                        _context.WarehouseTransefers.Add(warehouseTranseferToCreate);
                        await _context.SaveChangesAsync();
                        var code = Guid.NewGuid();
                        foreach (var item in request.Details)
                        {
                            var transeferDetails = new WarehouseTranseferDetails
                            {
                                ProductVariationId = item.ProductVariationId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                WarehouseTranseferId = warehouseTranseferToCreate.Id
                            };
                            _context.WarehouseTranseferDetails.Add(transeferDetails);

                            var ProductSummaryOut = await _context.WarehouseSummaries.Include(w => w.Product).Include(w => w.Warehouse)
                                .IgnoreQueryFilters()
                                .FirstOrDefaultAsync(w => w.Warehouse.StoreId == store.Id && w.WarehouseId == request.SourceWarehouseId && w.ProductId == item.ProductId);
                            if (ProductSummaryOut == null || ProductSummaryOut.Quantity < item.Quantity)
                            {
                                return Result<Response>.Failure(new List<string> { "source Warehouse doesnt have enough quantity" });
                            }
                            
                            
                            var ProductSummaryIn = await _context.WarehouseSummaries.Include(w => w.Product).Include(w => w.Warehouse)
                                .IgnoreQueryFilters()
                                .FirstOrDefaultAsync(w => w.Warehouse.StoreId == store.Id && w.WarehouseId == request.DestinationWarehouseId && w.ProductId == item.ProductId);
                            if (ProductSummaryIn == null)
                            {
                                ProductSummaryIn = new WarehouseSummaryEntity
                                {
                                    ProductId = item.ProductId,
                                    Quantity = 0,
                                    WarehouseId = request.DestinationWarehouseId
                                };
                                _context.WarehouseSummaries.Add(ProductSummaryIn);
                                await _context.SaveChangesAsync();
                            }
                            ProductSummaryIn.Quantity += item.Quantity;
                            ProductSummaryOut.Quantity -= item.Quantity;
                            _context.WarehouseSummaries.Update(ProductSummaryIn);
                            _context.WarehouseSummaries.Update(ProductSummaryOut);
                            await _context.SaveChangesAsync();
                            var warehouseLogIn = new WarehouseLogEntity
                            {
                                Code = code,
                                DocumentId = warehouseTranseferToCreate.Id,
                                DocumentType = DocumentType.Transefer,
                                OperationDate = DateTime.UtcNow,
                                ProductId = item.ProductId,
                                ProductVariationId= item.ProductVariationId,
                                Quantity= item.Quantity,
                                SellingPrice = item.UnitPrice,
                                Type = WarehouseLogType.In,
                                WarehouseId = request.DestinationWarehouseId
                            };
                            
                            var warehouseLogOut = new WarehouseLogEntity
                            {
                                Code = code,
                                DocumentId = warehouseTranseferToCreate.Id,
                                DocumentType = DocumentType.Transefer,
                                OperationDate = DateTime.UtcNow,
                                ProductId = item.ProductId,
                                ProductVariationId= item.ProductVariationId,
                                Quantity= item.Quantity,
                                SellingPrice = item.UnitPrice,
                                Type = WarehouseLogType.Out,
                                WarehouseId = request.SourceWarehouseId
                            };
                            _context.WarehouseLogs.Add(warehouseLogIn);
                            _context.WarehouseLogs.Add(warehouseLogOut);
                            
                        }

                        await _context.SaveChangesAsync(cancellationToken);

                        
                        



                        return Result<Response>.Success(new Response { });
                    }
                    catch (Exception ex)
                    {
                        await trans.RollbackAsync(cancellationToken);
                        return Result<Response>.Failure(new List<string> { ex.Message });
                    }


                }
            }
        }
    }
}
