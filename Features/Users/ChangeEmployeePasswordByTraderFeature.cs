using Microsoft.Identity.Client;

namespace BiyLineApi.Features.Users;

public sealed class ChangeEmployeePasswordByTraderFeature
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string NewPassword { get; set; }
    }
    
    public sealed class Response
    {
        public string Message { get; set; }
    }
}
