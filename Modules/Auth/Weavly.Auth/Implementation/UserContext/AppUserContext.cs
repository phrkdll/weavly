using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Implementation.UserContext;

public sealed class AppUserContext(IHttpContextAccessor contextAccessor) : IUserContext<AppUserId>
{
    private readonly IEnumerable<Claim>? claims = contextAccessor.HttpContext?.User.Claims;

    public AppUserId UserId => TryGetUserId(out var id) ? new AppUserId(id) : new AppUserId();

    private bool TryGetUserId(out string id)
    {
        id = this.claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        return id != string.Empty;
    }
}