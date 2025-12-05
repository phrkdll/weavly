using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Weavly.Configuration.Shared.Features.LoadConfig;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Wolverine;

namespace Weavly.Auth.Implementation;

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

            var config =
                await bus.InvokeAsync<Result>(LoadConfigurationCommand.Create<AuthModule>("Jwt"))
                as Success<LoadConfigurationResponse>;

            // Set the parameters from the provider
            parameters.ValidIssuer = config?.Data["Issuer"].AsString();
            parameters.ValidAudience = config?.Data["Audience"].AsString();
            parameters.IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    config?.Data["Secret"].AsString() ?? throw new ApplicationException("Missing secret key.")
                )
            );
        };
    }
}
