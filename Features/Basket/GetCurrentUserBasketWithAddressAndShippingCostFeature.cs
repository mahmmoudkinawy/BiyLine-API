using Bogus.DataSets;

namespace BiyLineApi.Features.Basket;

public class GetCurrentUserBasketWithAddressAndShippingCostFeature
{
    public sealed class Request : IRequest<Result<Response>> 
    { 
        public int AddressId { get; set; }
    }
    public sealed class Response
    {
        public decimal MaxShippingPrice { get; set; }
        public decimal FinalTotalPrice { get; set; }
        public AddressResponse Address { get; set; }
    }

    public sealed class AddressResponse
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressDetails { get; set; }
        public int GovernorateId { get; set; }
        public string GovernorateName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request,Result< Response>>
    {
        private readonly BiyLineDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(BiyLineDbContext context, IHttpContextAccessor httpContextAccessor,IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _dateTimeProvider = dateTimeProvider;

        }
        public async Task< Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.GetUserById();

            var address = await _context.Addresses
                .Include(a=>a.User)
                .Include(x=>x.Governorate)
                .FirstOrDefaultAsync(x => x.Id == request.AddressId && x.UserId == userId); // governorate

            if(address == null) 
            {
                return Result<Response>.Failure(new List<string> { "this address not found" });
            }
            
            var basket =  await _context.Baskets.FirstOrDefaultAsync(x=>x.UserId ==  userId); // total price

            basket.AddressId = address.Id;

             await _context.SaveChangesAsync();

            var governorateShippingsForTrader = _context.GovernorateShippings
                .Include(g=>g.TraderShippingCompany)
                .Where(g => g.GovernorateId == address.GovernorateId && g.TraderShippingCompany.StoreId == basket.StoreId);

            var governorateShippings = _context.ShippingCompanyGovernorates
                .Include(g=>g.ShippingCompany)
                .Where(g=>g.GovernorateId == address.GovernorateId && g.ShippingCompany.StoreId == basket.StoreId);

            decimal shippingPrice = 0;

            if(governorateShippingsForTrader.Count() <= 0 && governorateShippings.Count() <= 0 )
            {
                return Result<Response>.Failure(new List<string> { "there is no company shippings to this governorate" });
            }

            else if(governorateShippingsForTrader.Count() > 0 && governorateShippings.Count() <= 0 )
            {
                shippingPrice = governorateShippingsForTrader.Max(g => g.ShippingPrice);
            }
             else if(governorateShippingsForTrader.Count() <= 0 && governorateShippings.Count() > 0 )
            {
                shippingPrice = governorateShippings.Max(g => g.ShippingPrice);
            }
            else
            {
                var shippingPriceForTrader = governorateShippingsForTrader.Max(g => g.ShippingPrice);
                var shippingPriceForCompany = governorateShippings.Max(g => g.ShippingPrice);

                shippingPrice = shippingPriceForTrader > shippingPriceForCompany ? shippingPriceForTrader : shippingPriceForCompany;
            }


            var finalTotalPrice = shippingPrice + basket.TotalPrice;


            var response = new Response
            {
                MaxShippingPrice = shippingPrice,
                FinalTotalPrice = finalTotalPrice,
                Address = new AddressResponse
                {
                    Id = address.Id,
                    AddressDetails = address.AddressDetails,
                    GovernorateId = address.GovernorateId,
                    PhoneNumber = address.PhoneNumber,
                    UserId = address.UserId,
                    UserName = address.User.Name,
                    GovernorateName = address.Governorate.Name
                }
            };

            return Result<Response>.Success(response);

        }
    }

}
