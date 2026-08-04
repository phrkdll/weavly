using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Weavly.Core.Implementation;

namespace Weavly.Core.Tests.Implementation;

public sealed class ExtensionsTests
{
    private readonly IHostApplicationBuilder builderMock = Substitute.For<IHostApplicationBuilder>();
    private readonly WebApplicationBuilder slimBuilder = WebApplication.CreateSlimBuilder();

    [Fact]
    public void AddWeavly_Returns_WeavlyApplicationBuilder()
    {
        this.builderMock.AddWeavly().ShouldBeOfType<WeavlyApplicationBuilder>();
    }

    [Fact]
    public void UseWeavly_Throws_WhenAddWeavly_WasNotCalled()
    {
        var app = this.slimBuilder.Build();

        Should.Throw<InvalidOperationException>(app.UseWeavly);
    }

    [Fact]
    public void UseWeavly_CallsUse_OnRegisteredModules()
    {
        var weavlyBuilder = this.slimBuilder.AddWeavly().AddModule<TestModule>();

        weavlyBuilder.Build();
        var app = this.slimBuilder.Build();

        app.UseWeavly();
        var module = weavlyBuilder.Modules.First().ShouldBeOfType<TestModule>();
        module.UseCalled.ShouldBeTrue();
    }
}