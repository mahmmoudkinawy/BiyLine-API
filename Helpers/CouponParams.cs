namespace BiyLineApi.Helpers;
public sealed class CouponParams : PaginationParams
{
    public string? Code { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
