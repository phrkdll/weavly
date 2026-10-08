using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Core.Tests.Implementation;

public sealed class ExtensionsTests
{
    private readonly IHostApplicationBuilder builderMock = Substitute.For<IHostApplicationBuilder>();
    private readonly WebApplicationBuilder slimBuilder = WebApplication.CreateSlimBuilder();

    [Fact]
    public void AddWeavly_Returns_WeavlyApplicationBuilder()
    {
        builderMock.AddWeavly().ShouldBeOfType<WeavlyApplicationBuilder>();
    }

    [Fact]
    public void UseWeavly_Throws_WhenAddWeavly_WasNotCalled()
    {
        var app = slimBuilder.Build();

        Should.Throw<InvalidOperationException>(app.UseWeavly);
    }

    [Fact]
    public void UseWeavly_CallsUse_OnRegisteredModules()
    {
        var weavlyBuilder = slimBuilder.AddWeavly().AddModule<TestModule>();

        weavlyBuilder.Build();
        var app = slimBuilder.Build();

        app.UseWeavly();
        var module = weavlyBuilder.Modules.First().ShouldBeOfType<TestModule>();
        module.UseCalled.ShouldBeTrue();
    }

    [Fact]
    public async Task UseWeavly_Propagates_ModuleInitializationFailure()
    {
        var appBuilder = WebApplication.CreateSlimBuilder();
        appBuilder.AddWeavly().AddModule<FailingInitializationModule>().Build();
        var app = appBuilder.Build();
        app.UseWeavly();

        await Should.ThrowAsync<InvalidOperationException>(() => app.StartAsync());
    }

    public sealed class FailingInitializationModule : WeavlyModule
    {
        public override Task InitializeAsync(Wolverine.IMessageBus bus) =>
            Task.FromException(new InvalidOperationException("Initialization failed"));
    }
}
