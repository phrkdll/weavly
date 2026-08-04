using Microsoft.Extensions.DependencyInjection;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Auth.Implementation.UserContext;

public sealed class AppUserContextFactory(IServiceScopeFactory serviceScopeFactory) : IUserContextFactory<AppUserId>
{
    public IUserContext<AppUserId> CreateUserContext()
    {
        var scope = serviceScopeFactory.CreateScope();

        return scope.ServiceProvider.GetRequiredService<IUserContext<AppUserId>>();
    }
}