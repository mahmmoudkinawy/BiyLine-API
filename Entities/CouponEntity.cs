using System.ComponentModel.DataAnnotations.Schema;

namespace BiyLineApi.Entities;
public sealed class CouponEntity
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public StoreEntity? Store { get; set; }
    public int? StoreId { get; set; }
    public decimal? CommissionRate { get; internal set; }
    public decimal? DiscountPercentage { get; internal set; }
    [NotMapped]
    public CouponStatus Status { 
        get 
        {
            if(EndDate.HasValue && EndDate.Value < DateTime.UtcNow)
            {
                return CouponStatus.NotActive;
            }
            return CouponStatus.NotActive;
        } 
    }

    public  ICollection<CouponUsageEntity> Usage { get; set; } = new List<CouponUsageEntity>();
}