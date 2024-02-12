namespace BiyLineApi.Entities
{
    public class CouponUsageEntity
    {
        public int Id { get; set; }
        public int CouponId { get; set; }
        public virtual CouponEntity Coupon { get; set; }
        public decimal Price { get; set; }

        public int? ItemCount { get; set; }
        public int UserId { get; set; }
        public virtual UserEntity User { get; set; }
    }
}
