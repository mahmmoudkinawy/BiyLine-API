﻿namespace BiyLineApi.Features.TraderShippingCompany;

public sealed class DeleteShippingCenterFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int ShippingCenterId { get; set; }
    }
    public sealed class Response { }
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.ShippingCenterId)
                .GreaterThan(0);
        }
    }
    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var traderId = _httpContextAccessor.GetUserById();
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.OwnerId == traderId);

            if (store == null)
            {
                return Result<Response>.Failure(new List<string> { "this trader does not have a store" });
            }

            var shippingCenter = await _context.CenterShippings
                .Include(c => c.GovernorateShipping)
                .ThenInclude(c => c.TraderShippingCompany)
                .FirstOrDefaultAsync(c => c.Id == request.ShippingCenterId && c.GovernorateShipping.TraderShippingCompany.StoreId == store.Id);

            if (shippingCenter == null)
            {
                return Result<Response>.Failure(new List<string> { "this center not exist for this store" });
            }

            _context.CenterShippings.Remove(shippingCenter);
            await _context.SaveChangesAsync();

            return Result<Response>.Success(new Response { });
        }
    }
}
