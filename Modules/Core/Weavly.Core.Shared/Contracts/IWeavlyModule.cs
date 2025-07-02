using Microsoft.AspNetCore.Builder;

namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyModule
{
    /// <summary>
    ///     Perform any tasks regarding setup here (i.e. service registration)
    /// </summary>
    /// <param name="builder">
    ///     <see cref="WebApplicationBuilder"/>
    /// </param>
    void Configure(WebApplicationBuilder builder);

    /// <summary>
    ///     Perform any tasks regarding app start here (i.e. endpoint activation)
    /// </summary>
    /// <param name="app">
    ///     <see cref="WebApplication"/>
    /// </param>
    void Use(WebApplication app);

    /// <summary>
    ///     Perform any tasks regarding first time module initialization here
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    ///     Returns the types of DbContexts used within this module
    /// </summary>
    Type[] DbContextTypes { get; }
}
