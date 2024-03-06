namespace BiyLineApi.Features.Products.Commands
{
    public sealed class UpdateProductRateFeature
    {
        public class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }
            public decimal Rating { get; set; }
            public string Review { get; set; } = string.Empty;
        }
        public class Response {
        public RateEntity Rate { get; set; }
        }

        public sealed class Validator : AbstractValidator<Request>
        {
            private readonly BiyLineDbContext _context;

            public Validator(BiyLineDbContext context)
            {
                _context = context ??
               throw new ArgumentNullException(nameof(context));

                RuleFor(r => r.Id)
                    .NotEmpty()
                    .NotNull()
                    .GreaterThan(0);
                

                RuleFor(r => r.Rating)
                    .NotEmpty()
                    .NotNull()
                    .GreaterThan(0).LessThanOrEqualTo(5);

                When(r => r.Id >0, () =>
                {
                    RuleFor(r => r.Id)
                        .NotEmpty()
                        .NotNull()
                        .GreaterThan(0)
                        .Must((req, _) => _context.Rates.Any(c => c.Id == req.Id))
                            .WithMessage("Rate with the given Id does not exist.");
                });
            }
        }

        public sealed class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly BiyLineDbContext _context;
            private readonly IMapper _mapper;

            public Handler(BiyLineDbContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var rate = await _context.Rates.FirstOrDefaultAsync(x => x.Id == request.Id);
                    if(rate==null) return Result<Response>.Failure(new List<string> { "Rate with the given Id does not exist." });
                    rate.Review = request.Review;
                    rate.RatingDate = DateTime.UtcNow;
                    rate.Rating = request.Rating;
                    _context.Entry(rate).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    return Result<Response>.Success(new Response { Rate=rate });
                }
                catch (Exception error)
                {
                    return Result<Response>.Failure(new List<string> { error.Message });
                }
            }
        }
    }
}
