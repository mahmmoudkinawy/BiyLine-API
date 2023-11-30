

namespace BiyLineApi.Features.Supplier;

public sealed class CreateContractOrderFeature
{
    public sealed class VariationRequest
    {
        public int Quantity { get; set; }
        public int ProductVariationId { get; set; }
    }

    public sealed class ContractOrderProductRequest
    {
        public int ProductId { get; set; }
        public List<VariationRequest> Variations { get; set; }  
    }
    public sealed class Request : IRequest<Result<Response>>
    {
        public List<ContractOrderProductRequest> ContractOrderProducts { get; set; }
    }

    public sealed class Response { }

    public sealed class CreateContractOrderValidator : AbstractValidator<Request>
    {
        public CreateContractOrderValidator()
        {
            RuleForEach(x => x.ContractOrderProducts).SetValidator(new ContractOrderProductValidator());
        }
    }

    public sealed class ContractOrderProductValidator : AbstractValidator<CreateContractOrderFeature.ContractOrderProductRequest>
    {
        public ContractOrderProductValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleForEach(x => x.Variations).SetValidator(new VariationValidator());
        }
    }

    public sealed class VariationValidator : AbstractValidator<CreateContractOrderFeature.VariationRequest>
    {
        public VariationValidator()
        {
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.ProductVariationId).GreaterThan(0);
        }
    }
    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var supplierId = _httpContextAccessor.GetValueFromRoute("supplierId");

            var supplierFromDb = await _context.Users.Include(s => s.Store).FirstOrDefaultAsync(s => (s.Id == supplierId) && (s.StoreId != null) && (s.Store.Activity != StoreActivityEnum.Sectional.ToString()));

            if (supplierFromDb is null)
            {
                return Result<Response>.Failure("This Supplier Is Not Found");
            }

            var traderId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == traderId);

            if (store == null)
            {
                return Result<Response>.Failure("This Store Is Not Found");
            }

            var contractOrder = new ContractOrderEntity
            {
                FromStoreId = traderId,
                ToStoreId = supplierId,
                Status = ContractOrderStatus.Pending.ToString()
            };

            foreach ( var item in request.ContractOrderProducts)
            {
                foreach (var variation in item.Variations)
                {
                     var productVariation = await _context.ProductVariations.FirstOrDefaultAsync(pv => (pv.Id == variation.ProductVariationId) && (pv.ProductId == item.ProductId));

                    if (productVariation is null)
                    {
                        return Result<Response>.Failure("This Product Does not Have This Variation");
                    }
                }

                contractOrder.ContractOrderProducts.Add(new ContractOrderProductEntity
                {
                    ProductId = item.ProductId,
                    ContractOrderVariations = item.Variations.Select(x => new ContractOrderVariationEntity
                    {
                        Quantity = x.Quantity,
                        ProductVariationId = x.ProductVariationId,
                    }).ToList()
                }) ;
            }

            _context.ContractOrders.Add(contractOrder);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
