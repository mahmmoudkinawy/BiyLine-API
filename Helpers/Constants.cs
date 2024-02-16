using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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
        public const string MustBeEmployee = "MustBeEmployee";
        public const string MustBeTraderOrEmployee = "MustBeTraderOrEmployee";
        public const string AddressWrite = "Address.Write";
        public const string AddressRead = "Address.Read";
        public const string ProductRead = "Product.Read";
        public const string ProductWrite = "Product.Write";
        public const string EmployeeRead = "Employee.Read";
        public const string EmployeeWrite = "Employee.Write";
        public const string ShippingCompanyRead = "ShippingCompany.Read";
        public const string ShippingCompanyWrite = "ShippingCompany.Write"; 
        public const string DashboardRead = "Dashboard.Read";
        public const string DashboardWrite = "Dashboard.Write";
        public const string OrdersRead = "Orders.Read";
        public const string OrdersWrite = "Orders.Write";
        public const string SupportRead = "Support.Read";
        public const string SupportWrite = "Support.Write";
        public const string WarehouseRead = "Warehouse.Read";
        public const string WarehouseWrite = "Warehouse.Write";
        public const string CouponsRead = "Coupons.Read";
        public const string CouponsWrite = "Coupons.Write";

    }


    public abstract class Cors
    {
        public const string OriginSectionKey = "CrossOriginRequests:AllowedOrigins";
        public const string PolicyName = "default";
    }
}
