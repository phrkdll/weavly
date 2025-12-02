using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Weavly.Core.Implementation;
using Weavly.Core.Persistence;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Core.Tests.Implementation;

public class ExtensionsTests
{
    private readonly IHostApplicationBuilder builder = Substitute.For<IHostApplicationBuilder>();
    private readonly WebApplicationBuilder slimBuilder = WebApplication.CreateSlimBuilder();

    [Fact]
    public async Task AddWeavly_Returns_WeavlyApplicationBuilder()
    {
        builder.AddWeavly().ShouldBeOfType<WeavlyApplicationBuilder>();
    }

    [Fact]
    public async Task UseWeavly_Throws_WhenAddWeavly_WasNotCalled()
    {
        var app = slimBuilder.Build();

        Should.Throw<InvalidOperationException>(app.UseWeavly);
    }

    [Fact]
    public async Task UseWeavly_CallsUse_OnRegisteredModules()
    {
        var weavlyBuilder = slimBuilder.AddWeavly().AddModule<TestModule>();

        weavlyBuilder.Build();
        var app = slimBuilder.Build();

        app.UseWeavly();
        var module = weavlyBuilder.Modules.First().ShouldBeOfType<TestModule>();
        module.UseCalled.ShouldBeTrue();
    }

    public class TestModule : WeavlyModule
    {
        public bool UseCalled { get; private set; }

        public override void Configure(IHostApplicationBuilder builder)
        {
            builder.AddWeavlyModuleDbContext<TestModule, TestDbContext>();

            base.Configure(builder);
        }

        public override void Use(WebApplication app)
        {
            this.UseCalled = true;

            base.Use(app);
        }

        public override Type[] DbContextTypes => [typeof(TestDbContext)];
    }

    public abstract class TestDbContext : CoreDbContext { }

    public class InMemoryTestDbContext : TestDbContext
    {
        public override DatabaseFacade Database => null!;
    }
}
