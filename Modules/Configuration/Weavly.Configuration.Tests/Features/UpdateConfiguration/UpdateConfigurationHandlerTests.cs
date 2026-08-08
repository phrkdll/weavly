using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NSubstitute;
using Shouldly;
using Weavly.Configuration.Features.UpdateConfiguration;
using Weavly.Configuration.Models;
using Weavly.Configuration.Shared.Events;
using Weavly.Configuration.Shared.Features.UpdateConfiguration;
using Weavly.Configuration.Shared.Identifiers;
using Weavly.Configuration.Shared.Models;
using Weavly.Core.Shared.Implementation;
using Weavly.Core.Shared.Models;
using Wolverine;

namespace Weavly.Configuration.Tests.Features.UpdateConfiguration;

public sealed class UpdateConfigurationHandlerTests : ConfigurationHandlerTests
{
    private readonly ConfigurationId configurationId = new(ObjectId.GenerateNewId().ToString());

    private readonly UpdateConfigurationHandler sut;

    private readonly IMessageBus messageBus = Substitute.For<IMessageBus>();
    private readonly ILogger<UpdateConfigurationHandler> logger = Substitute.For<ILogger<UpdateConfigurationHandler>>();

    private readonly UpdateConfigurationCommandValidator updateConfigurationCommandValidator = new(
        Substitute.For<ILogger<UpdateConfigurationCommandValidator>>()
    );

    public UpdateConfigurationHandlerTests()
    {
        this.sut = new UpdateConfigurationHandler(
            this.Repository,
            this.updateConfigurationCommandValidator,
            this.logger,
            this.messageBus
        );
    }

    [Fact]
    public async Task HandleAsync_ReturnsValidationFailure_WhenCommandContainsNonScalarValues()
    {
        var command = new UpdateConfigurationCommand(this.configurationId, true, 1);

        var result = await sut.HandleAsync(command);

        result.ShouldBeOfType<ValidationFailure>();
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_ConfigurationDoesNotExist()
    {
        var command = new UpdateConfigurationCommand(this.configurationId, false);

        var result = await sut.HandleAsync(command);

        result.ShouldBeOfType<Failure>();

        await this.messageBus.DidNotReceive().PublishAsync(Arg.Any<ConfigurationChangedEvent>());
    }

    [Fact]
    public async Task HandleAsync_ShouldPublishConfigurationChangedEvent_AfterConfigurationWasUpdated()
    {
        await this.Repository.Configurations.InsertAsync(
            new AppConfiguration { ValueType = ConfigurationValueType.Bool, BoolValue = false }
        );
        var configId = this.Repository.Configurations.Query().First().Id;

        var command = new UpdateConfigurationCommand(configId, true);

        var result = await sut.HandleAsync(command);

        result.ShouldBeOfType<Success<EmptyResponse>>();

        await this.messageBus.Received().PublishAsync(Arg.Any<ConfigurationChangedEvent>());
    }

    [Fact]
    public async Task HandleAsync_ReturnsFailure_WhenValueTypeDoesNotMatch()
    {
        await this.Repository.Configurations.InsertAsync(
            new AppConfiguration { ValueType = ConfigurationValueType.Int, IntValue = 1 }
        );
        var configId = this.Repository.Configurations.Query().First().Id;

        var command = new UpdateConfigurationCommand(configId, true);

        var result = await sut.HandleAsync(command);

        result.ShouldBeOfType<Failure>();

        await this.messageBus.DidNotReceive().PublishAsync(Arg.Any<ConfigurationChangedEvent>());
    }
}
