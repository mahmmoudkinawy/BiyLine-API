﻿namespace BiyLineApi.Features.Stores;
public sealed class GetNationalIdImageFeature
{
    public sealed class Request : IRequest<Response> { }

    public sealed class Response
    {
        public string ImageUrl { get; set; }
        public bool IsNationalIdImageCompleted { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.OwnerId == userId, cancellationToken: cancellationToken);

            var storeNationalIdImage = await _context.Images
                .FirstOrDefaultAsync(i =>
                    i.IsMain.Value && i.StoreId == store.Id && i.Type == "NationalIdImage",
                        cancellationToken: cancellationToken);

            if (store == null || storeNationalIdImage == null)
            {
                return new Response
                {
                    IsNationalIdImageCompleted = false
                };
            }

            var baseUri = _httpContextAccessor.BaseUri(nameof(_httpContextAccessor));

            return new Response
            {
                ImageUrl = baseUri.CombineUri(storeNationalIdImage?.ImageUrl),
                IsNationalIdImageCompleted = true
            };
        }
    }
}
