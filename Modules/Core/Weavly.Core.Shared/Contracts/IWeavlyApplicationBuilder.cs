namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyApplicationBuilder
{
    IEnumerable<IWeavlyModule> Modules { get; }

    IWeavlyApplicationBuilder AddModule<T>()
        where T : IWeavlyModule;

    void Build();
}
