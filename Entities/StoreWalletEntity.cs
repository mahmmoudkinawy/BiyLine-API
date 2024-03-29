﻿namespace BiyLineApi.Entities;
public sealed class StoreWalletEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal? TotalBalance { get; set; }
    public DateTime DateTime { get; set; }
    public string StoreWalletStatus { get; set; }
    public int? EmployeeId{ get; set; }
    public EmployeeEntity Employee { get; set; }
    public int StoreId { get; set; }   
    public StoreEntity Store { get;set; }
    public ICollection<CashDepositePermissionEntity> CashDepositePermissions { get; set; } = new List<CashDepositePermissionEntity>();
    public ICollection<CashDiscountPermissionEntity> CashDiscountPermissions { get; set; } = new List<CashDiscountPermissionEntity>();
    public ICollection<SalaryPaymentEntity> SalaryPayments { get; set; } = new List<SalaryPaymentEntity>();
    public ICollection<StoreWalletLogs> Logs { get; set; } = new List<StoreWalletLogs>();

}
