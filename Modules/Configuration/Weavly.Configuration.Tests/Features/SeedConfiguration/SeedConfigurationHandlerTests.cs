using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Weavly.Configuration.Features.SeedConfiguration;
using Weavly.Configuration.Shared.Features.SeedConfiguration;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Configuration.Tests.Features.SeedConfiguration;

public sealed class SeedConfigurationHandlerTests : ConfigurationHandlerTests
{
    private readonly SeedConfigurationHandler sut;

    public SeedConfigurationHandlerTests()
    {
        sut = new SeedConfigurationHandler(Repository, Substitute.For<ILogger<SeedConfigurationHandler>>());
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateMissingConfiguration_AndPreserveItOnRepeatedSeed()
    {
        var firstResult = await sut.HandleAsync(
            SeedConfigurationCommand.Create<SeedConfigurationHandlerTests>("Name", "original", "Category")
        );
        var first = firstResult.ShouldBeOfType<Success<SeedConfigurationResponse>>().Data;

        var secondResult = await sut.HandleAsync(
            SeedConfigurationCommand.Create<SeedConfigurationHandlerTests>("Name", "replacement", "OtherCategory")
        );
        var second = secondResult.ShouldBeOfType<Success<SeedConfigurationResponse>>().Data;
        var persisted = await Repository.Configurations.FindAsync(x =>
            x.Module == nameof(SeedConfigurationHandlerTests) && x.Name == "Name"
        );

        first.Created.ShouldBeTrue();
        second.Created.ShouldBeFalse();
        second.Id.ShouldBe(first.Id);
        persisted.ShouldNotBeNull();
        persisted.StringValue.ShouldBe("original");
        persisted.Category.ShouldBe("Category");
    }
}
