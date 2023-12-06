using System.Text.Json.Serialization;

namespace BiyLineApi.Services;
public sealed class BasketService : IBasketService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly BiyLineDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BasketService(
        IDateTimeProvider dateTimeProvider,
        BiyLineDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _dateTimeProvider = dateTimeProvider ??
            throw new ArgumentNullException(nameof(dateTimeProvider));
        _context = context ??
            throw new ArgumentNullException(nameof(context));
        _httpContextAccessor = httpContextAccessor ??
            throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public async Task<BasketEntity> GetBasketFromCookieAsync()
    {
        var basketJson = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

        BasketEntity basket;

        if (!string.IsNullOrEmpty(basketJson))
        {
            basket = JsonSerializer.Deserialize<BasketEntity>(basketJson, new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            });
        }
        else
        {
            basket = new BasketEntity
            {
                CreatedAt = _dateTimeProvider.GetCurrentDateTimeUtc(),
                UserId = null
            };

            _context.Baskets.Add(basket);
            await _context.SaveChangesAsync();
        }

        return basket;
    }

    public void SetBasketInCookie(BasketEntity basket)
    {
        var basketJson = JsonSerializer.Serialize(basket, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        });

        _httpContextAccessor.HttpContext.Response.Cookies
            .Append("basket", basketJson, new CookieOptions
            {
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
    }

    public async Task RemoveItemFromBasketAsync(int productId)
    {
        var basket = await GetBasketFromCookieAsync();

        var itemToRemove = basket.BasketItems.FirstOrDefault(bi => bi.ProductId == productId);

        if (itemToRemove != null)
        {
            var product = _context.Products.Find(productId);

            if (product != null)
            {
                basket.BasketItems.Remove(itemToRemove);
                basket.TotalPrice -= (product.SellingPrice.Value * itemToRemove.Quantity.Value);
                basket.UpdatedAt = _dateTimeProvider.GetCurrentDateTimeUtc();

                await _context.SaveChangesAsync();

                SetBasketInCookie(basket);
            }
        }
    }

}
