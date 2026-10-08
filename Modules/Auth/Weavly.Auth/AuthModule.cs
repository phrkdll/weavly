using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Weavly.Auth.Features.RegisterUser;
using Weavly.Auth.Implementation.JsonWebToken;
using Weavly.Auth.Implementation.UserContext;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth;

[ExcludeFromCodeCoverage]
public sealed class AuthModule : WeavlyModule
{
    public override IReadOnlyCollection<string> InitializationDependencies => ["ConfigurationModule", "MailModule"];

    public override void Configure(IHostApplicationBuilder builder)
    {
        builder
            .Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie();

        builder
            .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                };

                // Configure the JwtBearerEvents
                options.EventsType = typeof(AppJwtBearerEvents);
            });

        builder.Services.AddAuthorization();

        builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
        builder.Services.AddScoped<IJwtProvider, JwtProvider>();
        builder.Services.AddScoped<AppJwtBearerEvents>();

        builder.Services.AddScoped<AuthRepository>();

        builder.Services.AddScoped<IUserContext<AppUserId>, AppUserContext>();
        builder.Services.AddSingleton<IUserContextFactory<AppUserId>, AppUserContextFactory>();

        builder.Services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();

        base.Configure(builder);
    }

    public override void Use(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        base.Use(app);
    }
}
