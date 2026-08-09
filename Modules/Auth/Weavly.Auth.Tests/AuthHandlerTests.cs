using NSubstitute;
using Weavly.Auth.Implementation;
using Weavly.Auth.Persistence;
using Weavly.Auth.Seeding;
using Weavly.Configuration.Shared;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Implementation;
using Weavly.Core.Tests.Shared;

namespace Weavly.Auth.Tests;

public abstract class AuthHandlerTests : WeavlyHandlerTests
{
    protected readonly AuthRepository Repository;

    protected AuthHandlerTests()
    {
        Repository = new AuthRepository(new WeavlyRepositoryMock<AuthModule>(TimeProviderMock));

        MessageBusMock
            .InvokeAsync<Result>(
                Arg.Is<LoadConfigurationCommand>(x => x!.Module == "AuthModule"),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Result.Success(
                    new LoadConfigurationResponse(
                        "AuthModule",
                        [
                            ConfigurationItem.Create("Secret") with
                            {
                                StringValue = EncryptionKeyGenerator.GenerateAesKey(256),
                                Category = ConfigCategory.JsonWebToken,
                            },
                            ConfigurationItem.Create("Issuer") with
                            {
                                StringValue = "Weavly",
                                Category = ConfigCategory.JsonWebToken,
                            },
                            ConfigurationItem.Create("Audience") with
                            {
                                StringValue = "Weavly",
                                Category = ConfigCategory.JsonWebToken,
                            },
                        ]
                    )
                )
            );
    }
}
