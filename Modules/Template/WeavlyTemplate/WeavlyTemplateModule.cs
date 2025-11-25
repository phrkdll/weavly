using Microsoft.AspNetCore.Builder;

namespace WeavlyTemplate;

public class WeavlyTemplateModule : WeavlyModule
{
    public override void Configure(WebApplicationBuilder builder)
    {
        // Module specific configuration can be added here

        base.Configure(builder);
    }

    public override void Use(WebApplication app)
    {
        // Module specific middleware can be added here

        base.Use(app);
    }

    public override Task InitializeAsync()
    {
        // Module specific initialization can be added here

        return base.InitializeAsync();
    }
}
