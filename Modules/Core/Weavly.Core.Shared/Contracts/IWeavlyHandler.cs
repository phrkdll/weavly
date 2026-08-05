namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyHandler<in TCommand> : IWeavlyHandler
    where TCommand : IWeavlyCommand
{
    Task<Result> HandleAsync(TCommand command, CancellationToken ct = default);
}

public interface IWeavlyHandler;
