namespace BiyLineApi.Features.Supplier;
public sealed class GetRetailTradersFeature
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? Predicate { get; set; }
    }

    public sealed class Response
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? CountryName { get; set; }
        public string? GovernorateName { get; set; }
        public string? RegionName { get; set; }
        public string? StoreName { get; set; }
        public string? ImageUrl { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly BiyLineDbContext _context;
        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                            throw new ArgumentNullException(nameof(context));
        }
        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _context.Users
                 .Include(u => u.Store)
                 .Where(u => u.StoreId != null && u.Store.Activity != StoreActivityEnum.Sectional.ToString())
                 .AsQueryable();

            if (!string.IsNullOrEmpty(request.Predicate))
            {
                query = query.Where(u => u.Name.Contains(request.Predicate));
            }

            var traders = query
                .Select(s => new Response
                {
                    Name = s.Name,
                    Address = s.Store.Address,
                    GovernorateName = s.Store.Governorate.Name,
                    CountryName = s.Store.Country.Name,
                    RegionName = s.Store.Region.Name,
                    StoreName = s.Store.EnglishName,
                    ImageUrl = s.Store.Images.OrderByDescending(i => i.DateUploaded).FirstOrDefault(i => i.Type == "ProfilePictureImage").ImageUrl
                });

            return await PagedList<Response>.CreateAsync(
                traders.AsNoTracking(),
                request.PageNumber.Value,
                request.PageSize.Value);
        }
    }
}


