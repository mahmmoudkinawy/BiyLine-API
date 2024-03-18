using System.ComponentModel.DataAnnotations;

namespace BiyLineApi.Features.UserFavoriteProduct.Commands
{
    public sealed class AddUserFavoriteProductFeature
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            [Required]
            public int UserId { get; set; }

            [Required]
            public int ProductId { get; set; }
        }

        public sealed class Response
        {
            public BiyLineApi.Entities.UserFavoriteProduct UserFavoriteProduct { get; set; }
        }

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
                        .GreaterThan(0)
                        .Must((req, _) => _context.Categories.Any(c => c.Id == req.ProductId))
                            .WithMessage("Product with the given Id does not exist");
                RuleFor(r => r.UserId)
                    .NotEmpty()
                        .NotNull()
                        .GreaterThan(0)
                        .Must((req, _) => _context.Users.Any(c => c.Id == req.UserId))
                            .WithMessage("User with the given Id does not exist");
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
                    if(await _context.UserFavoriteProducts.AnyAsync(x=>x.ProductId==request.ProductId&&x.UserId==request.UserId))
                    {
                        return Result<Response>.Failure(new List<string> { "User already has this product as his favorits." });
                    }
                    Entities.UserFavoriteProduct userFavoriteProduct = new Entities.UserFavoriteProduct() { ProductId = request.ProductId, UserId = request.UserId };
                    await _context.UserFavoriteProducts.AddAsync(userFavoriteProduct);
                    await _context.SaveChangesAsync();
                    return Result<Response>.Success(new Response { UserFavoriteProduct = userFavoriteProduct });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }
            }
        }
    }
}
