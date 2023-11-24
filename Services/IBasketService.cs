namespace BiyLineApi.Services;
public interface IBasketService
{
    Task<BasketEntity> GetBasketFromCookieAsync();
    void SetBasketInCookie(BasketEntity basket);
    Task RemoveItemFromBasketAsync(int productId);
}
