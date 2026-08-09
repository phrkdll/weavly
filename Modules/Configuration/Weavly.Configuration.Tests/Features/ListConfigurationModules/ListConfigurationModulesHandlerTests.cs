using Shouldly;
using Weavly.Configuration.Features.ListConfigurationModules;
using Weavly.Configuration.Models;
using Weavly.Configuration.Shared.Features.ListConfigurationModules;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Configuration.Tests.Features.ListConfigurationModules;

public class ListConfigurationModulesHandlerTests : ConfigurationHandlerTests
{
    private readonly ListConfigurationModulesHandler sut;

    public ListConfigurationModulesHandlerTests()
    {
        Repository.Configurations.InsertAsync(
                new AppConfiguration
                {
                    Module = "ExtraModule",
                    Category = "Default",
                    Name = "FeatureEnabled",
                    BoolValue = true,
                }
            )
            .Wait();
        Repository.Configurations.InsertAsync(
                new AppConfiguration
                {
                    Module = "ExtraModule",
                    Category = "Default",
                    Name = "Endpoint",
                    StringValue = "TestValue",
                }
            )
            .Wait();
        Repository.Configurations.InsertAsync(
                new AppConfiguration
                {
                    Module = "CoreModule",
                    Category = "Default",
                    Name = "MaxItems",
                    IntValue = 42,
                }
            )
            .Wait();
        Repository.Configurations.InsertAsync(
                new AppConfiguration
                {
                    Module = "CoreModule",
                    Category = "Default",
                    Name = "PiValue",
                    DoubleValue = 3.14,
                }
            )
            .Wait();

        sut = new ListConfigurationModulesHandler(Repository);
    }

    [Fact]
    public async Task Handler_ShouldReturn_ModuleNames_AsDistinctList()
    {
        var result = await sut.HandleAsync(new ListConfigurationModulesCommand());

        var data = result.ShouldBeOfType<Success<ListConfigurationModulesResponse>>().Data;
        data.Modules.ShouldNotBeEmpty();
        data.Modules.Count().ShouldBe(2);
        data.Modules.ShouldContain("CoreModule");
        data.Modules.ShouldContain("ExtraModule");
    }
}
