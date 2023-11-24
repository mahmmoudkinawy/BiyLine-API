namespace BiyLineApi.Services;
public interface ITokenService
{
    Task<string> CreateTokenAsync(UserEntity user);
}
