using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Weavly.Auth.Models;
using Weavly.Configuration.Shared.Extensions;
using Wolverine;

namespace Weavly.Auth.Implementation.JsonWebToken;

public sealed class JwtProvider(IMessageBus bus) : IJwtProvider
{
    public async Task<string> GenerateTokenAsync(AppUser user, DateTime expires)
    {
        var options = JwtOptions.FromConfigurationResponse(await bus.LoadConfigurationAsync<AuthModule>());

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.Value),
            new(JwtRegisteredClaimNames.Email, user.Email),
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(options.Issuer, options.Audience, claims, null, expires, signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
