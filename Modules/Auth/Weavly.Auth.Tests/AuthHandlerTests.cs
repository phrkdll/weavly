using NSubstitute;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Tests;

namespace Weavly.Auth.Tests;

public abstract class AuthHandlerTests : WeavlyHandlerTests<TestAuthDbContext>
{
    protected readonly IUserContext<AppUserId> userContextMock = Substitute.For<IUserContext<AppUserId>>();
}
