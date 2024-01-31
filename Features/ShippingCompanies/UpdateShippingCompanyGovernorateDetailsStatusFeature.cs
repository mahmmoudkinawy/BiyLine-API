namespace BiyLineApi.Features.ShippingCompanies
{
    public sealed class UpdateShippingCompanyGovernorateDetailsStatusFeature
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }
        }

        public sealed class Response
        {
            public ShippingCompanyGovernorateDetailsEntity ShippingCompanyGovernorateDetails { get; internal set; }
        }



        public sealed class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly BiyLineDbContext _context;
      

            public Handler(BiyLineDbContext context)
            {
                _context = context ??
                    throw new ArgumentNullException(nameof(context));
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var details = await _context.ShippingCompanyGovernorateDetails.FirstOrDefaultAsync(s => s.Id == request.Id);
                    if (details == null)
                    {
                        return Result<Response>.Failure(new List<string> { "Governorate not found" });
                    }
                    var shippingCompany = await _context.ShippingCompanies.FirstOrDefaultAsync(s => s.Id == details.ShippingCompanyId);

                    if (shippingCompany == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company not found" });
                    }
                    if (shippingCompany.PaymentMethod == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company must have Payment Method first" });
                    }
                    if (shippingCompany.PhoneNumber == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company must have Phone Number first" });
                    }
                    if (shippingCompany.DeliveryCases == null)
                    {
                        return Result<Response>.Failure(new List<string> { "shipping company must have delivery case first" });
                    }


                    details.Status = !details.Status;
                   

                    _context.ShippingCompanyGovernorateDetails.Update(details);
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result<Response>.Success(new Response { ShippingCompanyGovernorateDetails = details });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(new List<string> { ex.Message });
                }
            }
        }
    }
}
