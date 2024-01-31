namespace BiyLineApi.Entities
{
    public class CouponCategory
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public CouponEntity Coupon { get; set; }

        public int CategoryId { get; set; }
        public CategoryEntity Category { get; set; }
    }
}
