namespace BiyLineApi.Services;
public sealed class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<UserEntity> _userManager;
    private readonly BiyLineDbContext _context;
    public TokenService(
        IConfiguration config,
        UserManager<UserEntity> userManager,
        BiyLineDbContext context)
    {
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config[Constants.TokenKey]!));
        _userManager = userManager ??
            throw new ArgumentNullException(nameof(userManager));
        _context = context;
    }

    public async Task<string> CreateTokenAsync(UserEntity user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        if(roles.Any(r=>r.Equals(Constants.Roles.Employee.ToString(), StringComparison.OrdinalIgnoreCase)))
        {
            var employee =  await _context.Employees.Include(e=> e.Permissions).FirstOrDefaultAsync(e=> e.UserId == user.Id);
            if(employee != null)
            {
                claims.AddRange(employee.Permissions.Select(permission => new Claim("Permission", permission.PermissionName)));
            }
        }

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
