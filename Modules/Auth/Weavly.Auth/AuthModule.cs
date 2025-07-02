using System.Security.Cryptography;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Weavly.Auth.Contracts;
using Weavly.Auth.Implementation;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Configuration.Shared;
using Weavly.Core;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth;

public sealed class AuthModule : WeavlyModule
{
    public override void Configure(WebApplicationBuilder builder)
    {
        builder
            .Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGitHub(options => SetOAuthOptions("GitHub", options, builder))
            .AddDiscord(options => SetOAuthOptions("Discord", options, builder));

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
                options.Events = new AppJwtBearerEvents();
            });

        builder.Services.AddAuthorization();

        builder.Services.AddScoped(_ => new PasswordHasher<AppUser>());
        builder.Services.AddScoped<IJwtProvider, JwtProvider>();

        builder.Services.AddScoped<IUserContext<AppUserId>, AppUserContext>();

        builder.Services.AddSingleton<IUserContextFactory<AppUserId>, AppUserContextFactory>();

        builder.AddWeavlyModuleDbContext<AuthModule, AuthDbContext>();

        base.Configure(builder);
    }

    private static void SetOAuthOptions(string provider, OAuthOptions options, WebApplicationBuilder builder)
    {
        options.ClientId = builder.Configuration[$"Authentication:{provider}:ClientId"] ?? string.Empty;
        options.ClientSecret = builder.Configuration[$"Authentication:{provider}:ClientSecret"] ?? string.Empty;
        options.CallbackPath = $"/signin/{provider.ToLowerInvariant()}";
    }

    public override void Use(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        base.Use(app);
    }

    public override async Task InitializeAsync()
    {
        await new CreateAppUserCommand("system@weavly.local", "system", "System").ExecuteAsync();

        CreateAppRoleCommand[] initialRoles = [new("Administrator"), new("User")];
        foreach (var role in initialRoles)
        {
            await role.ExecuteAsync();
        }

        CreateConfigurationCommand[] configItems =
        [
            CreateConfigurationCommand.Create<AuthModule>("Secret", GenerateEncryptionKey(256), "Jwt"),
            CreateConfigurationCommand.Create<AuthModule>("Issuer", "https://weavly.api", "Jwt"),
            CreateConfigurationCommand.Create<AuthModule>("Audience", "https://weavly.api", "Jwt"),
        ];

        foreach (var configItem in configItems)
        {
            await configItem.ExecuteAsync();
        }
    }

    private static string GenerateEncryptionKey(int keySize)
    {
        using var aes = Aes.Create();

        aes.KeySize = keySize;
        aes.GenerateKey();

        return Convert.ToBase64String(aes.Key);
    }

    public override Type[] DbContextTypes => [typeof(AuthDbContext)];
}
