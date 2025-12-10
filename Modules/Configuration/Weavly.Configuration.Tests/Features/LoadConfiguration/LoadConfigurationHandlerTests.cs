using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Configuration.Features.LoadConfiguration;
using Weavly.Configuration.Models;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Configuration.Tests.Features.LoadConfiguration;

public class LoadConfigurationHandlerTests : ConfigurationHandlerTests
{
    private readonly ILogger<LoadConfigurationHandler> loggerMock = Substitute.For<ILogger<LoadConfigurationHandler>>();

    private readonly LoadConfigurationHandler sut;

    public LoadConfigurationHandlerTests()
    {
        dbContextMock.Configurations.AddRange(
            new AppConfiguration
            {
                Id = new ConfigurationId(),
                Module = "ExistingModule",
                Category = "Default",
                Name = "FeatureEnabled",
                BoolValue = true,
            },
            new AppConfiguration
            {
                Id = new ConfigurationId(),
                Module = "ExistingModule",
                Category = "Default",
                Name = "Endpoint",
                StringValue = "TestValue",
            },
            new AppConfiguration
            {
                Id = new ConfigurationId(),
                Module = "ExistingModule",
                Category = "Default",
                Name = "MaxItems",
                IntValue = 42,
            },
            new AppConfiguration
            {
                Id = new ConfigurationId(),
                Module = "ExistingModule",
                Category = "Default",
                Name = "PiValue",
                DoubleValue = 3.14,
            }
        );
        dbContextMock.SaveChanges();

        sut = new LoadConfigurationHandler(dbContextMock, loggerMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_ForNullRequest()
    {
        var result = await this.sut.HandleAsync(null!, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Value cannot be null. (Parameter 'command')");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_WhenNoConfigurationFound()
    {
        var command = LoadConfigurationCommand.Create<LoadConfigurationHandlerTests>();

        var result = await this.sut.HandleAsync(command, CancellationToken.None);

        result.ShouldBeOfType<Failure>();
        result.Message.ShouldBe("Could not find configuration");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_WithConfigurationResponse()
    {
        var command = LoadConfigurationCommand.Create("ExistingModule");

        var result = await this.sut.HandleAsync(command, CancellationToken.None);

        var data = result
            .ShouldBeOfType<Success<LoadConfigurationResponse>>()
            .Data.ShouldBeOfType<LoadConfigurationResponse>();

        data.Module.ShouldBe("ExistingModule");
        data.Items.Count().ShouldBe(4);

        var category = data.Items.Select(x => x.Category).Distinct().SingleOrDefault();
        category.ShouldBe("Default");

        data.Items.ShouldContain(i => i.Name == "FeatureEnabled" && i.AsBool());
        data.Items.ShouldContain(i => i.Name == "Endpoint" && i.AsString() == "TestValue");
        data.Items.ShouldContain(i => i.Name == "MaxItems" && i.AsInt() == 42);
        data.Items.ShouldContain(i => i.Name == "PiValue" && i.AsDouble().Equals(3.14));
    }
}
