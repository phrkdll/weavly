using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Shouldly;
using Testably.Abstractions.Testing;
using Weavly.Core.Persistence;

namespace Weavly.Core.Tests.Persistence;

public sealed class ContextOptionsTests
{
    private readonly IServiceProvider rootProvider = Substitute.For<IServiceProvider>();
    private readonly IServiceScopeFactory scopeFactory = Substitute.For<IServiceScopeFactory>();
    private readonly IServiceScope scope = Substitute.For<IServiceScope>();
    private readonly IServiceProvider scopedProvider = Substitute.For<IServiceProvider>();
    private readonly IFileSystem fileSystemMock = new MockFileSystem();

    public ContextOptionsTests()
    {
        // Root provider must provide IServiceScopeFactory
        rootProvider.GetService(typeof(IServiceScopeFactory)).Returns(scopeFactory);

        // ScopeFactory's CreateScope() IS mockable
        scopeFactory.CreateScope().Returns(scope);

        // Scope exposes the scoped provider
        scope.ServiceProvider.Returns(scopedProvider);

        // EF Core interceptors list must not be null
        scopedProvider.GetService<IEnumerable<ISaveChangesInterceptor>>().Returns([]);
        scopedProvider.GetService<IFileSystem>().Returns(fileSystemMock);
    }

    private static IConfiguration BuildTestConfiguration(Dictionary<string, string?> items) =>
        new ConfigurationBuilder().AddInMemoryCollection(items).Build();

    [Theory]
    [InlineData("ModuleA"), InlineData("ModuleB"), InlineData("ModuleC")]
    public void CreateContextOptions_ShouldSucceed_WithValidOrMissingConfig(string moduleName)
    {
        var config = BuildTestConfiguration(
            new() { ["ModuleA:DatabaseType"] = "sqlite", ["ModuleB:DatabaseType"] = "postgres" }
        );
        scopedProvider.GetService<IConfiguration>().Returns(config);

        ContextOptions.CreateContextOptions(rootProvider, moduleName).ShouldNotBeNull();

        // Verifications
        scopeFactory.Received().CreateScope();
        scopedProvider.Received().GetService<IConfiguration>();
    }

    [Fact]
    public void CreateContextOptions_ShouldThrow_WithUnsupportedDatabaseType()
    {
        var config = BuildTestConfiguration(new() { ["Module:DatabaseType"] = "unsupported" });
        scopedProvider.GetService<IConfiguration>().Returns(config);

        Should.Throw<ArgumentException>(() => ContextOptions.CreateContextOptions(rootProvider, "Module"));
    }

    [Fact]
    public void CreateContextOptions_ShouldUseProvider_IfPresentOnConfig()
    {
        var config = BuildTestConfiguration(new() { ["provider"] = "sqlite" });

        var options = ContextOptions.RetrieveModuleOptions(config, "Module");
        options.DatabaseType.ShouldBe("sqlite");
    }
}
