namespace BiyLineApi.Entities
{
    public class StoreWalletLogs
    {
        public int Id { get; set; }
        public int? StoreWalletId { get; set; }
        public StoreWalletEntity? StoreWallet { get; set; }
        public decimal? Value { get; set; }
        public StoreWalletLogType? LogStatus { get; set; }
        public int? EmpId { get; set; }
        public UserEntity? Emp { get; set; }

    }
}
