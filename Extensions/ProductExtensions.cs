namespace BiyLineApi.Extensions;
public static class ProductExtensions
{
    public static decimal? CalculatePriceAfterDiscount(this decimal? price, decimal? discountPercentage)
    {
        return price - ((price * discountPercentage) / 100);
    }
}
