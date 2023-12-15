namespace BiyLineApi.Features.StoreWallet;

public sealed class CreateStoreWalletFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
      public string StoreWalletName { get; set; }

      public int? EmployeeId { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.StoreWalletName)
                .NotEmpty();
        }
    }

    public  sealed class Handler : IRequestHandler<Request, Result<Response>>
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
        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            int traderId = _httpContextAccessor.GetUserById();

            var trader = await _context.Users.Include(u=>u.Store).FirstOrDefaultAsync(s => s.Id == traderId && s.StoreId != null); 

            if(trader == null)
            {
                return Result<Response>.Failure("this trader not found");
            }

            if (request.EmployeeId != null)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == request.EmployeeId && e.StoreId == trader.Store.Id);

                if (employee == null)
                {
                    return Result<Response>.Failure("this employee not found");
                }

            }

            var storeWallet = new StoreWalletEntity
            {
                Name = request.StoreWalletName,
                EmployeeId = request.EmployeeId,
                StoreId = trader.Store.Id,
                TotalBalance = 0,
                StoreWalletStatus = StoreWalletStatusEnum.Active.ToString(),
                DateTime = DateTime.UtcNow,
            };

            _context.StoreWallets.Add(storeWallet);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });

        }
    }
}
