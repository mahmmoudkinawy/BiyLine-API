namespace BiyLineApi.Helpers;
public abstract class Constants
{
    public const string TokenKey = "TokenKey";

    public abstract class Roles
    {
        public const string Customer = "Customer";
        public const string Admin = "Admin";
        public const string Trader = "Trader";
        public const string Representative = "Representative";
        public const string ShippingCompany = "ShippingCompany";
        public const string Employee = "Employee";
        public const string Manager = "Manager";
    }

    public abstract class Policies
    {
        public const string MustBeTrader = "MustBeTrader";
    }

    public abstract class Cors
    {
        public const string OriginSectionKey = "CrossOriginRequests:AllowedOrigins";
        public const string PolicyName = "default";
    }
}
