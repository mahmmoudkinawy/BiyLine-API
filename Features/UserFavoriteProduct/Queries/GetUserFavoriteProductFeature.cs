using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BiyLineApi.Features.UserFavoriteProduct.Commands
{
    public sealed class GetUserFavoriteFeature
    {
        public sealed class Request : IRequest<OneOf<PagedList<Response>, Result<Response>>>
        {
            public int? Id { get; set; }

            public int PageNumber { get; set; }
            public int PageSize { get; set; }
        }

        public sealed class Response
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
        }

        public sealed class Handler : IRequestHandler<Request, OneOf<PagedList<Response>, Result<Response>>>
        {
            private readonly BiyLineDbContext _context;
            public Handler(

                BiyLineDbContext context)
            {

                _context = context ?? throw new ArgumentNullException(nameof(context));

            }
            public async Task<OneOf<PagedList<Response>, Result<Response>>> Handle(
            Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var ufp= _context.UserFavoriteProducts.Include(x=>x.Product).ThenInclude(x=>x.ProductTranslations).Where(x => x.Id == request.Id || request.Id==null).AsQueryable();
                    if (ufp != null)
                    {
                        var result = ufp.Select(p => new Response
                        {
                            Id = p.Id,
                            ProductId = p.ProductId,
                            ProductName = p.Product.ProductTranslations.FirstOrDefault().Name
                        });
                        return await PagedList<Response>.CreateAsync(
                result.AsNoTracking(),
                request.PageNumber,
                request.PageSize);
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
