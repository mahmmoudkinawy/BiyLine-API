namespace BiyLineApi.Entities
{
    public class CouponCategory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public virtual CategoryEntity Category { get; set; }
        public int CouponId { get; set; }
        public virtual CouponEntity Coupon { get; set; }
    }
}
