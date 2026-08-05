using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Core.Tests;

public class TestModule : WeavlyModule
{
    public bool ConfigureCalled { get; private set; }

    public bool UseCalled { get; private set; }

    public override void Configure(IHostApplicationBuilder builder)
    {
        ConfigureCalled = true;
        base.Configure(builder);
    }

    public override void Use(WebApplication app)
    {
        UseCalled = true;

        base.Use(app);
    }
}
