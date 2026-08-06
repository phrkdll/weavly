using NSubstitute;
using Weavly.Auth.Persistence;
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
        this.Repository = new AuthRepository(new WeavlyRepositoryMock<AuthModule>(this.TimeProviderMock));

        this.MessageBusMock.InvokeAsync<Result>(
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
                                StringValue = AuthModule.GenerateEncryptionKey(256),
                                Category = AuthModule.ConfigCategory.JsonWebToken,
                            },
                            ConfigurationItem.Create("Issuer") with
                            {
                                StringValue = "Weavly",
                                Category = AuthModule.ConfigCategory.JsonWebToken,
                            },
                            ConfigurationItem.Create("Audience") with
                            {
                                StringValue = "Weavly",
                                Category = AuthModule.ConfigCategory.JsonWebToken,
                            },
                        ]
                    )
                )
            );
    }
}
