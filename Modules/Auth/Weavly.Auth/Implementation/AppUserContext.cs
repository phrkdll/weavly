using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Implementation;

public sealed class AppUserContext(IHttpContextAccessor contextAccessor) : IUserContext<AppUserId>
{
    private readonly IEnumerable<Claim>? _claims = contextAccessor.HttpContext?.User.Claims;

    public AppUserId UserId => AppUserId.TryParse(GetUserId(), out var id) ? id : AppUserId.Empty;

    private string? GetUserId()
    {
        return _claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}
