using Weavly.Auth.Models;

namespace Weavly.Auth.Implementation.JsonWebToken;

public interface IJwtProvider
{
    Task<string> GenerateTokenAsync(AppUser user, DateTime expires);
}