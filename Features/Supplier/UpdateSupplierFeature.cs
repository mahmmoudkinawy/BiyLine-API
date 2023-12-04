using Microsoft.Identity.Client;

namespace BiyLineApi.Features.Supplier;

public sealed class UpdateSupplierFeature
{

    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? TradeName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? TaxCard { get; set; }
        public string? CommercialRecord { get; set; }
        public string? PaymentMethod { get; set; }
        public string? AccountNumber { get; set; }
    }
    public sealed class Response { }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.Name)
                .NotEmpty()
                .WithMessage("The Name Is Required");

            When(s => !string.IsNullOrEmpty(s.Name), () =>
            {
                RuleFor(s => s.Name)
                    .Must(s => s.Any(c => !char.IsDigit(c)))
                    .WithMessage("The Name Must Not Contain Any letters");
            });

            RuleFor(s => s.PhoneNumber)
                .NotEmpty()
                .WithMessage("The Phone Number Is Required");

            When(s => !string.IsNullOrEmpty(s.PhoneNumber), () =>
            {
                RuleFor(s => s.PhoneNumber)
                    .MaximumLength(13)
                    .WithMessage("Phone Number Can Not Be More Than 13")
                    .Must(s => Regex.IsMatch(s, @"^\d+$"))
                    .WithMessage("Phone Number Must Be Only Numbers");
            });

            When(
               s => s.PaymentMethod != null,
               () =>
               {
                   RuleFor(s => s.PaymentMethod)
                      .Must((request, paymentMethod) =>
                      {
                          return Enum.TryParse(paymentMethod, true, out PaymentMethodEnum method);
                      })
                       .WithMessage("The Payment Method Is InValid");
               });
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == userId);

            if (store == null)
            {
                return Result<Response>.Failure("This Store Is Not Found");
            }

            var supplierId = _httpContextAccessor.GetValueFromRoute("supplierId");
            var supplierFromDb = await _context.Suppliers.Where(s => s.StoreId == store.Id).FirstOrDefaultAsync(s => s.Id == supplierId);

            if (supplierFromDb is null)
            {
                return Result<Response>.Failure("This Supplier Is Not Found");
            }

            supplierFromDb.Name = request.Name;
            supplierFromDb.TradeName = request.TradeName;
            supplierFromDb.PhoneNumber = request.PhoneNumber;
            supplierFromDb.Email = request.Email;
            supplierFromDb.Address = request.Address;
            supplierFromDb.AccountNumber = request.AccountNumber;
            supplierFromDb.CommercialRecord = request.CommercialRecord;
            supplierFromDb.TaxCard = request.TaxCard;
            supplierFromDb.PaymentMethod = request.PaymentMethod;


            _context.Update(supplierFromDb);

            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
