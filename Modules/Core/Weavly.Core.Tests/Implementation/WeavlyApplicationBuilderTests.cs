using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Shouldly;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Core.Tests.Implementation;

public sealed class WeavlyApplicationBuilderTests
{
    private readonly IHostApplicationBuilder hostApplicationBuilderMock = Substitute.For<IHostApplicationBuilder>();

    private readonly IServiceCollection serviceCollectionMock = Substitute.For<IServiceCollection>();

    private readonly WeavlyApplicationBuilder sut;

    public WeavlyApplicationBuilderTests()
    {
        hostApplicationBuilderMock.Services.Returns(serviceCollectionMock);
        sut = new WeavlyApplicationBuilder(hostApplicationBuilderMock);
    }

    [Fact]
    public async Task AddModule_ShouldReturn_WithValidModules_AndReturnAnUpdatedInstance()
    {
        var result = sut.AddModule<TestModule>();

        result.Modules.Count().ShouldBe(1);

        result.ShouldBeOfType<WeavlyApplicationBuilder>();
    }

    [Fact]
    public async Task Build_ShouldConfigureModules_AndRegisterEndpoints()
    {
        sut.AddModule<TestModule>().Build();

        var module = sut.Modules.First().ShouldBeOfType<TestModule>();

        module.ConfigureCalled.ShouldBeTrue();
        serviceCollectionMock.ReceivedWithAnyArgs().AddFastEndpoints();
    }

    [Fact]
    public async Task Build_ShouldInvoke_SwaggerDocument_InDevEnvironment()
    {
        hostApplicationBuilderMock.Environment.Returns(
            new HostingEnvironment { EnvironmentName = Environments.Development }
        );

        sut.AddModule<TestModule>().Build();

        serviceCollectionMock.Received().SwaggerDocument();
    }

    private class TestModule : WeavlyModule
    {
        public bool ConfigureCalled { get; private set; }

        public override void Configure(IHostApplicationBuilder builder)
        {
            ConfigureCalled = true;
            base.Configure(builder);
        }
    }

    private class TestEndpoint : Core.Shared.Implementation.Endpoint<object>
    {
        public override void Configure()
        {
            Get("/");
            AllowAnonymous();
        }

        public override Task HandleAsync(object req, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
