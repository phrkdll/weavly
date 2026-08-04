using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Weavly.Configuration.Shared.Extensions;
using Wolverine;

namespace Weavly.Auth.Implementation.JsonWebToken;

public class AppJwtBearerEvents : JwtBearerEvents
{
    private readonly IMessageBus bus;

    public AppJwtBearerEvents(IMessageBus bus)
    {
        SetOnMessageReceivedHandler();
        this.bus = bus;
    }

    private void SetOnMessageReceivedHandler()
    {
        OnMessageReceived = async context =>
        {
            if (
                context.Options.TokenValidationParameters
                is not { ValidAudience: null, ValidIssuer: null, IssuerSigningKey: null } parameters
            )
            {
                return;
            }

            var options = JwtOptions.FromConfigurationResponse(await this.bus.LoadConfigurationAsync<AuthModule>());

            // Set the parameters from the provider
            parameters.ValidIssuer = options.Issuer;
            parameters.ValidAudience = options.Audience;
            parameters.IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(options.Secret)
            );
        };
    }
}