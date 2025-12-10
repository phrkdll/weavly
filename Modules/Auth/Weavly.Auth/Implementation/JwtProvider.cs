using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Weavly.Auth.Contracts;
using Weavly.Auth.Models;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Wolverine;

namespace Weavly.Auth.Implementation;

public sealed class JwtProvider(IMessageBus bus) : IJwtProvider
{
    public async Task<string> GenerateTokenAsync(AppUser user, DateTime expires)
    {
        var response = await bus.InvokeAsync<Result>(LoadConfigurationCommand.Create<AuthModule>("Jwt"));

        if (response is not Success<LoadConfigurationResponse> config)
        {
            return string.Empty;
        }

        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Data["Secret"].AsString())),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            config.Data["Issuer"].AsString(),
            config.Data["Audience"].AsString(),
            claims,
            null,
            expires,
            signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
