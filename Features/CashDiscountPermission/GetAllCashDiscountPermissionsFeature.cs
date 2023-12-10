namespace BiyLineApi.Features.CashDiscountPermission;

public sealed class GetAllCashDiscountPermissionsFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public string? Predicate { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public sealed class Response
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string StoreWalletName { get; set; }
        public string EmployeeName { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
            private readonly BiyLineDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
            {
                _context = context ??
                    throw new ArgumentNullException(nameof(context));
                _httpContextAccessor = httpContextAccessor ??
                    throw new ArgumentNullException(nameof(httpContextAccessor));
            }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var storeWalletId = _httpContextAccessor.GetValueFromRoute("storeWalletId");
            var userId = _httpContextAccessor.GetUserById();
            var role = _httpContextAccessor.GetUserRole();

            IQueryable<CashDiscountPermissionEntity> query = null;

            if (role == Constants.Roles.Trader)
            {
                query = _context.CashDiscountPermissions
                  .Where(s => s.StoreWalletId == storeWalletId && s.StoreWallet.Store.OwnerId == userId)
                  .AsQueryable();

                if (!string.IsNullOrEmpty(request.Predicate))
                {
                    query = query.Where(s => s.StoreWallet.Name.Contains(request.Predicate));
                }



            }
            else if (role == Constants.Roles.Employee)
            {
                query = _context.CashDiscountPermissions
                   .Where(s => s.StoreWalletId == storeWalletId && s.StoreWallet.Employee.User.Id == userId && s.StoreWallet.EmployeeId == s.StoreWallet.Employee.Id)
                   .AsQueryable();

                if (!string.IsNullOrEmpty(request.Predicate))
                {
                    query = query.Where(s => s.StoreWallet.Name.Contains(request.Predicate));
                }

            }

            var cashDiscounts = query.Select(s => new Response
            {
                Id = s.Id,
                Amount = s.Amount,
                Date = s.Date,
                Reason = s.Reason,
                StoreWalletName = s.StoreWallet.Name,
                EmployeeName = s.StoreWallet.Employee.User.Name
            });

            return await PagedList<Response>.CreateAsync(
               cashDiscounts.AsNoTracking(),
               request.PageNumber.Value,
               request.PageSize.Value);

        }
    }
}
