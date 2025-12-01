using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Configuration.Implementation;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Shared.Implementation.Results;

namespace Weavly.Configuration.Tests;

public class CreateConfigurationCommandHandlerTests
{
    private static CreateConfigurationCommand TestCommand =>
        CreateConfigurationCommand.Create<CreateConfigurationCommandHandlerTests>("Test", 10, "TestCategory");

    private readonly ILogger<CreateConfigurationCommandHandler> loggerMock = Substitute.For<
        ILogger<CreateConfigurationCommandHandler>
    >();

    private readonly ConfigurationDbContext dbContextMock;

    private readonly CreateConfigurationCommandHandler sut;

    public CreateConfigurationCommandHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseStronglyTypeConverters();

        dbContextMock = Create.MockedDbContextFor<TestConfigurationDbContext>(dbContextOptions.Options);

        sut = new CreateConfigurationCommandHandler(dbContextMock, loggerMock);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturn_SuccessInstance_ForValidConfigurations()
    {
        var result = await this.sut.ExecuteAsync(TestCommand, CancellationToken.None);

        result.ShouldBeOfType<Success<ConfigurationId>>();

        loggerMock
            .ReceivedWithAnyArgs(1)
            .LogInformation("Received {MessageType} message", nameof(CreateConfigurationCommand));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturn_FailureInstance_ForDuplicateConfigurations()
    {
        await this.sut.ExecuteAsync(TestCommand, CancellationToken.None);

        var result = await this.sut.ExecuteAsync(TestCommand, CancellationToken.None);

        result.ShouldBeOfType<Failure>();

        loggerMock
            .ReceivedWithAnyArgs(2)
            .LogInformation("Received {MessageType} message", nameof(CreateConfigurationCommand));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturn_FailureInstance_ForNullRequest()
    {
        var result = await this.sut.ExecuteAsync(null!, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
