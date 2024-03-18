using System.ComponentModel.DataAnnotations;

namespace BiyLineApi.Features.UserFavoriteProduct.Commands
{
    public sealed class DeleteUserFavoriteFeature
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            [Required]
            public int Id { get; set; }
        }

        public sealed class Response
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
                        .GreaterThan(0)
                        .Must((req, _) => _context.UserFavoriteProducts.Any(c => c.Id == req.Id))
                            .WithMessage("The given Id does not exist");
                _context = context;
            }


        }

        public sealed class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly BiyLineDbContext _context;
            public Handler(

                BiyLineDbContext context)
            {

                _context = context ?? throw new ArgumentNullException(nameof(context));

            }
            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var ufp=await _context.UserFavoriteProducts.FirstOrDefaultAsync(x => x.Id == request.Id);
                    if (ufp != null)
                    {
                        _context.UserFavoriteProducts.Remove(ufp);
                        await _context.SaveChangesAsync();
                        return Result<Response>.Success(new Response { });
                    }
                    return Result<Response>.Failure(new List<string> { "Not Found." });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }
            }
        }
    }
}
