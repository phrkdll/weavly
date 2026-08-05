using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
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
using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Configuration.Shared.Features.CreateConfiguration;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Auth;

[ExcludeFromCodeCoverage]
public sealed class AuthModule : WeavlyModule
{
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

    public override async Task InitializeAsync(IMessageBus bus)
    {
        await bus.InvokeAsync<Result>(new CreateAppUserCommand("system@weavly.local", "system", "System"));

        CreateAppRoleCommand[] initialRoles = [new("Administrator"), new("User")];
        foreach (var role in initialRoles)
        {
            await bus.InvokeAsync<Result>(role);
        }

        CreateConfigurationCommand[] configItems =
        [
            CreateConfigurationCommand.Create<AuthModule>(
                "Secret",
                GenerateEncryptionKey(256),
                ConfigCategory.JsonWebToken
            ),
            CreateConfigurationCommand.Create<AuthModule>("Issuer", "https://weavly.api", ConfigCategory.JsonWebToken),
            CreateConfigurationCommand.Create<AuthModule>(
                "Audience",
                "https://weavly.api",
                ConfigCategory.JsonWebToken
            ),
            CreateConfigurationCommand.Create<AuthModule>(
                "DisableEmailVerification",
                false,
                ConfigCategory.GeneralSettings
            ),
            CreateConfigurationCommand.Create<AuthModule>(
                "DisableUserRegistration",
                false,
                ConfigCategory.GeneralSettings
            ),
            CreateConfigurationCommand.Create<AuthModule>("ForceTwoFactorAuth", false, ConfigCategory.GeneralSettings),
            CreateConfigurationCommand.Create<AuthModule>("MinimumLength", 8, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("MaximumLength", 32, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("RequireUppercase", true, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("RequireLowercase", true, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("RequireDigit", true, ConfigCategory.PasswordRules),
            CreateConfigurationCommand.Create<AuthModule>("RequireNonAlphanumeric", true, ConfigCategory.PasswordRules),
        ];

        foreach (var configItem in configItems)
        {
            await bus.InvokeAsync<Result>(configItem);
        }
    }

    public static string GenerateEncryptionKey(int keySize)
    {
        using var aes = Aes.Create();

        aes.KeySize = keySize;
        aes.GenerateKey();

        return Convert.ToBase64String(aes.Key);
    }

    public static class ConfigCategory
    {
        public const string PasswordRules = "PasswordRules";
        public const string JsonWebToken = "JsonWebToken";
        public const string GeneralSettings = "General";
    }
}
