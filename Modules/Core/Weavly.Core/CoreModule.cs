using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Weavly.Core.Implementation;
using Weavly.Core.Persistence.Interceptors;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core;

public sealed class CoreModule<TUserId> : WeavlyModule
    where TUserId : struct
{
    public override void Configure(WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSingleton<ITimeProvider, DefaultTimeProvider>();

        builder.Services.AddSingleton<MetaEntityInterceptor<TUserId>>();

        base.Configure(builder);
    }
}
