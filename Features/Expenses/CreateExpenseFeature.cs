namespace BiyLineApi.Features.Expenses;
public sealed class CreateExpenseFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public DateTime? Date { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public int ExpenseTypeId { get; set; }
        public int StoreWalletId { get; set; }
        public IFormFile? ReceiptImage { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Description)
                .NotEmpty()
                .MinimumLength(5)
                .MaximumLength(3000);

            RuleFor(r => r.Amount)
                .NotEmpty()
                .NotNull()
                .GreaterThanOrEqualTo(0);

            When(r => r.ReceiptImage != null, () =>
            {
                RuleFor(r => r.ReceiptImage)
                .NotNull()
                .Must(IsValidImage)
                        .WithMessage("Invalid image format or size. Maximum size is 2MB, and valid extensions are .jpg, .jpeg, .png, .gif");
            });

            RuleFor(r => r.ExpenseTypeId)
                .NotNull()
                .GreaterThan(0);

            RuleFor(r => r.StoreWalletId)
                .NotNull()
                .GreaterThan(0);
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
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IImageService _imageService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            BiyLineDbContext context,
            IDateTimeProvider dateTimeProvider,
            IImageService imageService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            if (store is null)
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen a store" });
            }

            if (!await _context.ExpenseTypes
                    .AnyAsync(et => et.StoreId == store.Id, cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen this expense type" });
            }

            if (!await _context.StoreWallets
                    .AnyAsync(et => et.StoreId == store.Id, cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string> { "Current Trader does not Owen this store wallet" });
            }

            var expenseToCreate = new ExpenseEntity
            {
                Amount = request.Amount,
                ExpenseTypeId = request.ExpenseTypeId,
                StoreId = store.Id,
                Date = request.Date,
                Description = request.Description,
                StoreWalletId = request.StoreWalletId,
            };

            if (request.ReceiptImage != null)
            {
                var imageUrl = await _imageService
                    .UploadImageAsync(request.ReceiptImage, "ExpenseImages");

                expenseToCreate.ReceiptImage = new ImageEntity
                {
                    Type = "Expense",
                    DateUploaded = _dateTimeProvider.GetCurrentDateTimeUtc(),
                    FileName = request.ReceiptImage?.FileName,
                    Expense = expenseToCreate,
                    ImageMimeType = request.ReceiptImage?.ContentType
                };
            };

            _context.Expenses.Add(expenseToCreate);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }
    }
}
