using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Weavly.Configuration.Shared.Features.LoadConfig;

namespace Weavly.Auth.Implementation;

public class AppJwtBearerEvents : JwtBearerEvents
{
    public AppJwtBearerEvents()
    {
        SetOnMessageReceivedHandler();
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
                await LoadConfigurationCommand.Create<AuthModule>("Jwt").ExecuteAsync()
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
