using Microsoft.Identity.Client;
using System.Runtime.InteropServices;

namespace BiyLineApi.Features.CashDepostiePermission;

public sealed class CreateCashDepostiePermissionFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Reason { get; set; }
        public decimal Amount { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.Amount)
                .GreaterThan(0);
        }
    }

    public sealed class Handler : IRequestHandler<Request,Result<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BiyLineDbContext _context;

        public Handler(IHttpContextAccessor httpContextAccessor, BiyLineDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {

            var storeWalletId = _httpContextAccessor.GetValueFromRoute("storeWalletId");

            var userId = _httpContextAccessor.GetUserById();

            var role = _httpContextAccessor.GetUserRole();

            StoreWalletEntity? storeWallet = null ;
            
            if(role== Constants.Roles.Trader)
            {
                  storeWallet = await _context.StoreWallets
                    .Include(s=>s.Store)
                    .FirstOrDefaultAsync(s => s.Id == storeWalletId && s.Store.OwnerId == userId);

                if(storeWallet == null)
                {
                    return Result<Response>.Failure(new List<string> { "this store wallet not found" });

                }

            }else if(role== Constants.Roles.Employee)
            {
                storeWallet = await _context.StoreWallets
                   .Include(s => s.Store)
                   .FirstOrDefaultAsync(s => s.Id == storeWalletId && s.Employee.User.Id == userId && s.EmployeeId == s.Employee.Id);


                if (storeWallet == null)
                {
                    return Result<Response>.Failure(new List<string> { "this store wallet not found" });

                }
            }

            var cashDeposite = new CashDepositePermissionEntity
            {
                Amount = request.Amount,
                Reason = request.Reason,
                Date = DateTime.UtcNow,
                StoreWalletId = storeWalletId
            };

            _context.CashDepositePermissions.Add(cashDeposite);

            storeWallet.TotalBalance += request.Amount;

           await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
