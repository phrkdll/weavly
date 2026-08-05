using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Configuration.Features.CreateConfiguration;
using Weavly.Configuration.Shared.Features.CreateConfiguration;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Configuration.Tests.Features.CreateConfiguration;

public sealed class CreateConfigurationHandlerTests : ConfigurationHandlerTests
{
    private readonly CreateConfigurationHandler sut;

    public CreateConfigurationHandlerTests()
    {
        this.TimeProviderMock.UtcNow.Returns(DateTime.UtcNow);

        this.sut = new CreateConfigurationHandler(
            this.Repository,
            Substitute.For<ILogger<CreateConfigurationHandler>>()
        );
    }

    private static CreateConfigurationCommand TestCommand =>
        CreateConfigurationCommand.Create<CreateConfigurationHandlerTests>("Test", 10, "TestCategory");

    [Theory]
    [InlineData("TestString")]
    [InlineData(123)]
    [InlineData(true)]
    [InlineData(45.67)]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_ForValidConfigurations(object value)
    {
        var command = CreateConfigurationCommand.Create<CreateConfigurationHandlerTests>(
            "TestConfig",
            value,
            "TestCategory"
        );
        var result = await this.sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Success<ConfigurationId>>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_ForDuplicateConfigurations()
    {
        await this.sut.HandleAsync(TestCommand, CancellationToken.None);

        var result = await this.sut.HandleAsync(TestCommand, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_ForNullRequest()
    {
        var result = await this.sut.HandleAsync(null!, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
