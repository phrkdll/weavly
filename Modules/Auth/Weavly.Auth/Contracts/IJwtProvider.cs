using Weavly.Auth.Models;

namespace Weavly.Auth.Contracts;

public interface IJwtProvider
{
    Task<string> GenerateTokenAsync(AppUser user, DateTime expires);
}
