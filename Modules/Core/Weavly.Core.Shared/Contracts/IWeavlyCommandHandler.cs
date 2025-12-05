namespace Weavly.Core.Shared.Contracts;

public interface IWeavlyCommandHandler<TCommand, TResponse> : IWeavlyCommandHandler
    where TCommand : IWeavlyCommand
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken ct);
}

public interface IWeavlyCommandHandler;
