namespace BiyLineApi.Extensions;
public static class CouponsExtensions
{
    public static bool IsCouponActive(this CouponEntity coupon) =>
        coupon != null ? DateTime.UtcNow <= coupon.EndDate
        : throw new ArgumentException(nameof(coupon));
}
