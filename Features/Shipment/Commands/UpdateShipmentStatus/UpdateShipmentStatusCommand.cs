﻿namespace BiyLineApi.Features.Shipment.Commands.UpdateShipmentStatus
{
    public class UpdateShipmentStatusCommand
    {
        public sealed class Request : IRequest<Result<Response>>
        {
            public int Id { get; set; }
            public ShipmentStatus ShipmentStatus { get; set; }
        }

        public sealed class Response
        {
        }
        public sealed class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly BiyLineDbContext _context;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<UserEntity> _userManager;
            private readonly IImageService _imageService;
            private readonly IDateTimeProvider _dateTimeProvider;

            public Handler(
                BiyLineDbContext context,
                UserManager<UserEntity> userManager,
                            IImageService imageService,
                                        IDateTimeProvider dateTimeProvider,

                IHttpContextAccessor httpContextAccessor)
            {
                _userManager = userManager ??
        throw new ArgumentNullException(nameof(userManager));
                _context = context ??
                    throw new ArgumentNullException(nameof(context));
                _httpContextAccessor = httpContextAccessor ??
                    throw new ArgumentNullException(nameof(httpContextAccessor));
                _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
                _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    var userId = _httpContextAccessor.GetUserById();
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (user == null)
                    {
                        return Result<Response>.Failure("User Not Found");
                    }
                    var shipment = await _context.Shipments.FirstOrDefaultAsync(sc => sc.Id == request.Id);
                    if (shipment == null)
                    {
                        return Result<Response>.Failure("Shipment Not Found");
                    }
                    shipment.Status = request.ShipmentStatus;
                    _context.Shipments.Update(shipment);
                    await _context.SaveChangesAsync();
                    return Result<Response>.Success(new Response { });
                }
                catch (Exception ex)
                {
                    return Result<Response>.Failure(ex.Message);
                }
            }
        }
    }
}
