namespace BiyLineApi.Features.Products.Commands
{
    public sealed class DeleteProductRateFeature
    {
        public class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }
           
        }
        public class Response
        {
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
                When(r => r.Id > 0, () =>
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
                    if (rate == null) return Result<Response>.Failure(new List<string> { "Rate with the given Id does not exist." });
                    _context.Rates.Remove(rate);
                    await _context.SaveChangesAsync();
                    return Result<Response>.Success(new Response {  });
                }
                catch (Exception error)
                {
                    return Result<Response>.Failure(new List<string> { error.Message });
                }
            }
        }
    }
}
