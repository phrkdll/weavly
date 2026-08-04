using Weavly.Configuration.Persistence;
using Weavly.Core.Tests.Shared;

namespace Weavly.Configuration.Tests;

public abstract class ConfigurationHandlerTests : WeavlyHandlerTests
{
    protected readonly ConfigurationRepository Repository;

    protected ConfigurationHandlerTests()
    {
        this.Repository =
            new ConfigurationRepository(new WeavlyRepositoryMock<ConfigurationModule>(this.TimeProviderMock));
    }
}