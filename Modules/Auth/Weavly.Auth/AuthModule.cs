using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Weavly.Auth.Contracts;
using Weavly.Auth.Implementation;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Persistence.Interceptors;
using Weavly.Auth.Shared.Features.CreateAppRole;
using Weavly.Auth.Shared.Features.CreateAppUser;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Configuration.Shared.Features.CreateConfiguration;
using Weavly.Core;
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
            .AddCookie()
            .AddGitHub(options => SetOAuthOptions("GitHub", options, (WebApplicationBuilder)builder))
            .AddDiscord(options => SetOAuthOptions("Discord", options, (WebApplicationBuilder)builder));

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

        builder.Services.AddScoped<IUserContext<AppUserId>, AppUserContext>();

        builder.Services.AddSingleton<IUserContextFactory<AppUserId>, AppUserContextFactory>();

        builder.Services.AddScoped<ISaveChangesInterceptor, UserMetaEntityInterceptor>();

        builder.AddWeavlyModuleDbContext<AuthModule, AuthDbContext>();

        base.Configure(builder);
    }

    private static void SetOAuthOptions(string provider, OAuthOptions options, WebApplicationBuilder builder)
    {
        options.ClientId = builder.Configuration[$"AuthModule:{provider}:ClientId"] ?? "unknown";
        options.ClientSecret = builder.Configuration[$"AuthModule:{provider}:ClientSecret"] ?? "unknown";
        options.CallbackPath = $"/signin/{provider.ToLowerInvariant()}";
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
            CreateConfigurationCommand.Create<AuthModule>("Secret", GenerateEncryptionKey(256), "Jwt"),
            CreateConfigurationCommand.Create<AuthModule>("Issuer", "https://weavly.api", "Jwt"),
            CreateConfigurationCommand.Create<AuthModule>("Audience", "https://weavly.api", "Jwt"),
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

    public override Type[] DbContextTypes => [typeof(AuthDbContext)];
}
