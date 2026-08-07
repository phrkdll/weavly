using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Testably.Abstractions;
using Weavly.Core.Implementation;
using Weavly.Core.Serialization;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Persistence;

namespace Weavly.Core;

[ExcludeFromCodeCoverage]
public sealed class CoreModule : WeavlyModule
{
    public override void Configure(IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSingleton<ITimeProvider, DefaultTimeProvider>();
        builder.Services.AddSingleton<IFileSystem, RealFileSystem>();

        BsonSerializer.RegisterSerializationProvider(new IdSerializationProvider());
        ConventionRegistry.Register(
            "WeavlyIds",
            new ConventionPack
            {
                new WeavlyIdConvention(),
                new EnumRepresentationConvention(BsonType.String),
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(true),
            },
            _ => true
        );

        builder.Services.AddOptions<MongoDbOptions>().BindConfiguration(nameof(MongoDbOptions));

        builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(
            sp.GetRequiredService<IOptions<MongoDbOptions>>().Value.ConnectionString
        ));
        builder.Services.AddScoped(typeof(IWeavlyRepository<>), typeof(WeavlyRepository<>));

        builder.Services.AddSingleton<IObjectHasher, ObjectHasher>();
        builder.Services.AddScoped<ITemplateService, TemplateService>();

        base.Configure(builder);
    }
}
