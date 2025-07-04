using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Weavly.Core.Implementation;
using Weavly.Core.Persistence.Interceptors;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core;

public sealed class CoreModule : WeavlyModule
{
    public override void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSingleton<ITimeProvider, DefaultTimeProvider>();

        builder.Services.AddScoped<ISaveChangesInterceptor, TimestampMetaEntityInterceptor>();

        base.Configure(builder);
    }
}
