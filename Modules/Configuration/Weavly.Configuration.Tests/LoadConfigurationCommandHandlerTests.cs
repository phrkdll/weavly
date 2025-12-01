using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Configuration.Implementation;
using Weavly.Configuration.Models;
using Weavly.Configuration.Persistence;
using Weavly.Configuration.Shared.Features.LoadConfig;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Shared.Implementation.Results;

namespace Weavly.Configuration.Tests;

public class LoadConfigurationCommandHandlerTests
{
    private readonly ILogger<LoadConfigurationCommandHandler> loggerMock = Substitute.For<
        ILogger<LoadConfigurationCommandHandler>
    >();

    private readonly ConfigurationDbContext dbContextMock;

    private readonly LoadConfigurationCommandHandler sut;

    public LoadConfigurationCommandHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseStronglyTypeConverters();

        dbContextMock = Create.MockedDbContextFor<TestConfigurationDbContext>(dbContextOptions.Options);
        dbContextMock.Configurations.AddRange(
            new AppConfiguration
            {
                Id = new ConfigurationId(),
                Module = "ModuleA",
                Category = "Default",
                Name = "FeatureEnabled",
                BoolValue = true,
            },
            new AppConfiguration
            {
                Id = new ConfigurationId(),
                Module = "ModuleA",
                Category = "Default",
                Name = "Endpoint",
                StringValue = "TestValue",
            },
            new AppConfiguration
            {
                Id = new ConfigurationId(),
                Module = "ModuleB",
                Category = "Default",
                IntValue = 42,
            },
            new AppConfiguration
            {
                Id = new ConfigurationId(),
                Module = "ModuleB",
                Category = "Default",
                DoubleValue = 3.14,
            }
        );
        dbContextMock.SaveChanges();

        sut = new LoadConfigurationCommandHandler(dbContextMock, loggerMock);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturn_FailureInstance_ForNullRequest()
    {
        var result = await this.sut.ExecuteAsync(null!, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Value cannot be null. (Parameter 'request')");
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturn_FailureInstance_WhenNoConfigurationFound()
    {
        var command = LoadConfigurationCommand.Create<ModuleC>();

        var result = await this.sut.ExecuteAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Could not find configuration");
    }

    [Theory]
    [InlineData(nameof(ModuleA))]
    [InlineData(nameof(ModuleB))]
    public async Task ExecuteAsync_ShouldReturn_SuccessInstance_WithConfigurationResponse(string moduleName)
    {
        var command = LoadConfigurationCommand.Create(moduleName);

        var result = await this.sut.ExecuteAsync(command, CancellationToken.None);

        var castResult = result.ShouldBeOfType<Success<LoadConfigurationResponse>>();
        var data = castResult.Data.ShouldBeOfType<LoadConfigurationResponse>();

        data.Module.ShouldBe(moduleName);
        data.Items.Count().ShouldBe(2);
    }

    private sealed class ModuleA { };

    private sealed class ModuleB { };

    private sealed class ModuleC { };
}
