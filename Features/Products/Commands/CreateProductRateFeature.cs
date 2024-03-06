
namespace BiyLineApi.Features.Products.Commands
{
    public sealed class CreateProductRateFeature
    {
        public class Request: IRequest<Result<Response>>
        {
            public decimal Rating { get; set; }
            public string Review { get; set; } = string.Empty;
            public DateTime RatingDate { get; set; } = DateTime.UtcNow;

            public int ProductId { get; set; }

            public int UserId { get; set; }
        }
        public class Response { }

        public sealed class Validator : AbstractValidator<Request>
        {
            private readonly BiyLineDbContext _context;

            public Validator(BiyLineDbContext context)
            {
                _context = context ??
               throw new ArgumentNullException(nameof(context));

                RuleFor(r => r.ProductId)
                    .NotEmpty()
                    .NotNull()
                    .GreaterThan(0);
                RuleFor(r => r.UserId)
                   .NotEmpty()
                   .NotNull()
                   .GreaterThan(0);

                RuleFor(r => r.Rating)
                    .NotEmpty()
                    .NotNull()
                    .GreaterThan(0).LessThanOrEqualTo(5);

                When(r => r.ProductId != null, () =>
                {
                    RuleFor(r => r.ProductId)
                        .NotEmpty()
                        .NotNull()
                        .GreaterThan(0)
                        .Must((req, _) => _context.Products.Any(c => c.Id == req.ProductId))
                            .WithMessage("Product with the given Id does not exist");
                });
                When(r => r.UserId != null, () =>
                {
                    RuleFor(r => r.UserId)
                        .NotEmpty()
                        .NotNull()
                        .GreaterThan(0)
                        .Must((req, _) => _context.Users.Any(c => c.Id == req.UserId))
                            .WithMessage("User with the given Id does not exist");
                });
            }
        }

        public sealed class Handler:IRequestHandler<Request, Result<Response>>
        {
            private readonly BiyLineDbContext _context;
            private readonly IMapper _mapper;

            public Handler(BiyLineDbContext context,IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    if(await _context.Rates.AnyAsync(x=>x.UserId==request.UserId))
                    {
                        return Result<Response>.Failure(new List<string> { "Current user already rate this product" });
                    }
                    var rateToAdd = _mapper.Map<RateEntity>(request);
                    await _context.Rates.AddAsync(rateToAdd);
                    await _context.SaveChangesAsync();
                    return Result<Response>.Success(new Response { });
                }
                catch(Exception error)
                {
                    return Result<Response>.Failure(new List<string> { error.Message });
                }
            }
        }
    }
}
