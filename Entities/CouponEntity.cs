using BiyLineApi.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiyLineApi.Entities;
public sealed class CouponEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public decimal? DiscountAmount { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? DiscountPercentage { get; set; }
    [NotMapped]
    public CuponStatus Status
    {
        get
        {
            if (EndDate.HasValue && EndDate < DateTime.Now)
            {
                return CuponStatus.NotActive;
            }
            else
            {
                return CuponStatus.Active;
            }
        }
    }
    public StoreEntity? Store { get; set; }
    public int? StoreId { get; set; }
    public decimal? CommissionRate { get; set; }

    public ICollection<CouponUsageEntity> Usage { get; set; } = new List<CouponUsageEntity>();
    public  ICollection<CouponCategory> CouponCategories { get; set; }
}