using BiyLineApi.Services;
using Microsoft.AspNetCore.Identity;

namespace BiyLineApi.Features.ShippingCompanies;
public sealed class CreateShippingCompanyFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? CountryCode { get; set; }
        public IFormFile? CommercialRegisterImage { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? IDImage { get; set; }
    }

    public sealed class Response
    {
        public ShippingCompanyEntity ShippinCompany { get; internal set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(IStringLocalizer<CommonResources> localizer, BiyLineDbContext context)
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(255);



            RuleFor(r => r.CountryCode)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.CountryCodeIsRequired].Value);

            RuleFor(r => r.Address)
                .NotEmpty()
                    .WithMessage(localizer[CommonResources.CountryCodeIsRequired].Value);
            RuleFor(r => r.IDImage)
    .Must(IsValidImage)
        .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");
            RuleFor(r => r.ProfileImage)
    .Must(IsValidImage)
        .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");
            RuleFor(r => r.CommercialRegisterImage)
    .Must(IsValidImage)
        .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");

        }
        private static bool IsValidImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return false;
            }

            if (image.Length > 2 * 1024 * 1024)
            {
                return false;
            }

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName);
            return validExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));
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
        private readonly UserManager<UserEntity> _userManager;
        private readonly IImageService _imageService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            BiyLineDbContext context,
            UserManager<UserEntity> userManager,
                        IImageService imageService,
                                    IDateTimeProvider dateTimeProvider,

            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager ??
    throw new ArgumentNullException(nameof(userManager));
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
                var userId = _httpContextAccessor.GetUserById();


                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return Result<Response>.Failure("User Not Found");
                }
                bool isShippingCompany = await _userManager.IsInRoleAsync(user, Constants.Roles.ShippingCompany);
                if (!isShippingCompany)
                {
                    return Result<Response>.Failure("this account is not a shipping company");
                }

                var shippingCompany = new ShippingCompanyEntity
                {
                    StoreId = user.StoreId,
                    CountryCode = request.CountryCode,
                    Name = request.Name,
                    Address = request.Address,
                    UserEntityId = userId,
                    
                };
                _context.ShippingCompanies.Add(shippingCompany);

                await _context.SaveChangesAsync(cancellationToken);

                if(request.IDImage != null)
                {
                    var IDIMG = await _imageService.UploadImageAsync(request.IDImage, "ShippingCompanyImages");

                    var IDImageToCreate = new ImageEntity
                    {
                        FileName = request.IDImage.FileName,
                        ImageMimeType = request.IDImage.ContentType,
                        DateUploaded = _dateTimeProvider.GetCurrentDateTimeUtc(),
                        ImageUrl = IDIMG,
                        Type = "ShippingCompanyImage",
                        ShippingCompanyEntityID = shippingCompany.Id
                    };
                    await _context.Images.AddAsync(IDImageToCreate);
                }
                if (request.ProfileImage != null)
                {
                    var ProfileIMG = await _imageService.UploadImageAsync(request.IDImage, "ShippingCompanyImages");
                    var ProfileIMGToCreate = new ImageEntity
                    {
                        FileName = request.ProfileImage.FileName,
                        ImageMimeType = request.ProfileImage.ContentType,
                        DateUploaded = _dateTimeProvider.GetCurrentDateTimeUtc(),
                        ImageUrl = ProfileIMG,
                        Type = "ShippingCompanyImage",
                        ShippingCompanyEntityID = shippingCompany.Id

                    };
                    await _context.Images.AddAsync(ProfileIMGToCreate);
                }
                if (request.CommercialRegisterImage != null)
                {
                    var CommercialRegisterIMG = await _imageService.UploadImageAsync(request.IDImage, "ShippingCompanyImages");
                    var CommercialRegisterIMGToCreate = new ImageEntity
                    {
                        FileName = request.CommercialRegisterImage.FileName,
                        ImageMimeType = request.CommercialRegisterImage.ContentType,
                        DateUploaded = _dateTimeProvider.GetCurrentDateTimeUtc(),
                        ImageUrl = CommercialRegisterIMG,
                        Type = "ShippingCompanyImage",
                        ShippingCompanyEntityID = shippingCompany.Id
                    };
                    await _context.Images.AddAsync(CommercialRegisterIMGToCreate);

                }
                await _context.SaveChangesAsync(cancellationToken);

                return Result<Response>.Success(new Response { ShippinCompany = shippingCompany });
            
        }
    }
}
