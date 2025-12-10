using NSubstitute;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Configuration.Shared;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;
using Weavly.Core.Tests;

namespace Weavly.Auth.Tests;

public abstract class AuthHandlerTests : WeavlyHandlerTests<TestAuthDbContext>
{
    protected AuthHandlerTests()
    {
        messageBusMock
            .InvokeAsync<Result>(
                Arg.Is<LoadConfigurationCommand>(x => x.Module == "AuthModule"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Success.Create(
                    new LoadConfigurationResponse(
                        "AuthModule",
                        [
                            ConfigurationResponse.Create("Secret") with
                            {
                                StringValue = AuthModule.GenerateEncryptionKey(256),
                            },
                            ConfigurationResponse.Create("Issuer") with
                            {
                                StringValue = "Weavly",
                            },
                            ConfigurationResponse.Create("Audience") with
                            {
                                StringValue = "Weavly",
                            },
                        ]
                    )
                )
            );
    }

    protected readonly IUserContext<AppUserId> userContextMock = Substitute.For<IUserContext<AppUserId>>();
}
