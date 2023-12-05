namespace BiyLineApi.Entities;

public sealed class StoreWalletEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal TotalBalance { get; set; }
    public DateTime DateTime { get; set; }
    public string StoreWalletStatus { get; set; }
    public int EmployeeId{ get; set; }
    public EmployeeEntity Employee { get; set; }
    public int StoreId { get; set; }   
    public StoreEntity Store { get;set; }
}
