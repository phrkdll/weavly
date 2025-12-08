using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Configuration.Features.CreateConfiguration;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared.Features.CreateConfiguration;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Configuration.Tests;

public class CreateConfigurationHandlerTests
{
    private static CreateConfigurationCommand TestCommand =>
        CreateConfigurationCommand.Create<CreateConfigurationHandlerTests>("Test", 10, "TestCategory");

    private readonly ILogger<CreateConfigurationHandler> loggerMock = Substitute.For<
        ILogger<CreateConfigurationHandler>
    >();

    private readonly ConfigurationDbContext dbContextMock;

    private readonly CreateConfigurationHandler sut;

    public CreateConfigurationHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseStronglyTypeConverters();

        dbContextMock = Create.MockedDbContextFor<TestConfigurationDbContext>(dbContextOptions.Options);

        sut = new CreateConfigurationHandler(dbContextMock, loggerMock);
    }

    [Theory]
    [InlineData("TestString"), InlineData(123), InlineData(true), InlineData(45.67)]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_ForValidConfigurations(object value)
    {
        var command = CreateConfigurationCommand.Create<CreateConfigurationHandlerTests>(
            "TestConfig",
            value,
            "TestCategory"
        );
        var result = await this.sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Success<ConfigurationId>>();

        loggerMock
            .ReceivedWithAnyArgs(1)
            .LogInformation("Received {MessageType} message", nameof(CreateConfigurationCommand));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_ForDuplicateConfigurations()
    {
        await this.sut.HandleAsync(TestCommand, CancellationToken.None);

        var result = await this.sut.HandleAsync(TestCommand, CancellationToken.None);

        result.ShouldBeOfType<Failure>();

        loggerMock
            .ReceivedWithAnyArgs(2)
            .LogInformation("Received {MessageType} message", nameof(CreateConfigurationCommand));
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_ForNullRequest()
    {
        var result = await this.sut.HandleAsync(null!, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
