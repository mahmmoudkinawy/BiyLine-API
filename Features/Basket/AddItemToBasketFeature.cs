namespace BiyLineApi.Features.Basket;
public sealed class AddItemToBasketFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public DateTime UpdatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? TotalPriceBeforeDiscount { get; set; }

        public ICollection<BasketItemResponse> BasketItems { get; set; }
    }

    public sealed class BasketItemResponse
    {
        public int? Quantity { get; set; }
        public int? ProductId { get; set; }
        public decimal Price { get; set; }
        public string? Name { get; set; }
        public string? ImageSrc { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000);
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context,
            IDateTimeProvider dateTimeProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ??
                throw new ArgumentNullException(nameof(dateTimeProvider));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User?.GetUserById();

            var basket = await _context.Baskets
                .IgnoreQueryFilters()
                .Include(b => b.BasketItems)
                .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken: cancellationToken);

            if (basket is null)
            {
                basket = new BasketEntity
                {
                    CreatedAt = _dateTimeProvider.GetCurrentDateTimeUtc(),
                    UserId = userId
                };

                _context.Baskets.Add(basket);
            }

            var productInBasket = basket.BasketItems?
                .FirstOrDefault(bi => bi.ProductId == request.ProductId);

            var product = await _context.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken: cancellationToken);

            if (product is null)
            {
                return Result<Response>.Failure("Product does not exist");
            }

            if (product.CountInStock <= 0)
            {
                return Result<Response>.Failure("Not enough stock available for this product.");
            }

            if (product.CountInStock < request.Quantity)
            {
                return Result<Response>.Failure("Not enough stock available for this product.");
            }

            if (productInBasket is not null)   // the same product
            {
                if (product.CountInStock < request.Quantity + productInBasket.Quantity)
                {
                    return Result<Response>.Failure("Not enough stock available for this product.");
                }

                productInBasket.Quantity += request.Quantity;
                basket.UpdatedAt = _dateTimeProvider.GetCurrentDateTimeUtc();
            }
            else   // another product in the basket or no product in basket
            {

                // 1 - no product in basket 
                if (basket.BasketItems.Count() == 0)
                {

                    if (product.CountInStock < request.Quantity)
                    {
                        return Result<Response>.Failure("Not enough stock available for this product.");
                    }
                    basket.StoreId = product.StoreId;

                }
                else
                {
                    if (product.StoreId != basket.StoreId)
                    {
                        return Result<Response>.Failure("The all products must are from the same store");
                    }


                    if (product.CountInStock < request.Quantity)
                    {
                        return Result<Response>.Failure("Not enough stock available for this product.");
                    }

                }

                var productTranslation = await _context.ProductTranslations
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                var image = await _context.Images
                    .FirstOrDefaultAsync(i =>
                        i.ProductId.Value == request.ProductId && i.IsMain.Value, cancellationToken: cancellationToken);

                basket.BasketItems.Add(new BasketItemEntity
                {
                    Name = productTranslation?.Name,
                    ImageSrc = image?.ImageUrl,
                    Price = product.SellingPrice.Value,
                    BasketId = basket.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                });
            }

            basket.TotalPrice += (product.SellingPrice.Value * request.Quantity);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response
            {
                Id = basket.Id,
                BasketItems = basket.BasketItems.Select(x => new BasketItemResponse
                {
                    ImageSrc = x.ImageSrc,
                    Name = x.Name,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList(),
                TotalPrice = basket.TotalPrice
            });
        }
    }
}