namespace BiyLineApi.Features.ShippingCompanies;
public sealed class CreateShippingCompanyFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }
        public int? GovernorateId { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IStringLocalizer<CommonResources> localizer, BiyLineDbContext context)
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(255);

            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.GovernorateId)
                .NotEmpty()
                .GreaterThan(0)
                .Must(governorateId => context.Governments.Any(g => g.Id == governorateId))
                    .WithMessage(localizer[CommonResources.GovernorateWithIdDoesNotExist].Value);

            RuleFor(r => r.CountryCode)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.CountryCodeIsRequired].Value);

            RuleFor(r => r.PhoneNumber)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.PhoneNumberIsRequired].Value)
                .Must((req, phoneNumber, context) => IsPhoneNumberValid(phoneNumber, req.CountryCode))
                    .WithMessage(localizer[CommonResources.PhoneNumberIsValid].Value);
        }

        private static bool IsPhoneNumberValid(string phoneNumber, string countryCode)
        {
            try
            {
                var phoneUtility = PhoneNumberUtil.GetInstance();
                var phoneNumberObj = phoneUtility.Parse(phoneNumber, countryCode);
                var result = phoneUtility.IsValidNumber(phoneNumberObj) &&
                    phoneUtility.IsValidNumberForRegion(phoneNumberObj, countryCode);
                return result;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "No store for current user." });
            }

            var shippingCompany = new ShippingCompanyEntity
            {
                StoreId = store.Id,
                Email = request.Email,
                CountryCode = request.CountryCode,
                Name = request.Name,
                PhoneNumber = request.PhoneNumber,
            };

            shippingCompany.ShippingCompanyGovernorates.Add(new ShippingCompanyGovernorateEntity
            {
                GovernorateId = request.GovernorateId.Value,
                ShippingCompanyId = shippingCompany.Id
            });

            var shippingCompanyFromDb = await _context.ShippingCompanies
                .Include(sc => sc.ShippingCompanyGovernorates)
                .FirstOrDefaultAsync(s =>
                    s.StoreId == shippingCompany.StoreId &&
                    s.Name.ToLower().Equals(shippingCompany.Name) &&
                    s.ShippingCompanyGovernorates.Any(scg => scg.GovernorateId == request.GovernorateId),
                        cancellationToken: cancellationToken);

            // Did not handle shipping company got the same 
            if (shippingCompanyFromDb == null)
            {
                return Result<Response>.Failure(new List<string> {
                    "There is already a shipping company with this name associated with your store." });
            }

            var shippingCompanyExists = await _context.ShippingCompanyGovernorates
                .FirstOrDefaultAsync(scg => scg.ShippingCompanyId == shippingCompanyFromDb.Id && scg.GovernorateId == request.GovernorateId, cancellationToken: cancellationToken);

            _context.ShippingCompanies.Add(shippingCompany);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
