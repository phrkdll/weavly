using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Weavly.Core.Shared.Contracts;
using Wolverine;

namespace Weavly.Core.Shared.Implementation.Endpoints;

public abstract class EndpointBase<TRequest, TResponse>(IMessageBus bus) : IWeavlyEndpoint<TRequest>
    where TRequest : IWeavlyCommand
    where TResponse : class
{
    private string[] authorizationPolicies = [];
    private bool authorize;

    public virtual async Task<IResult> HandleAsync(TRequest request, CancellationToken ct = default)
    {
        var result = await bus.InvokeAsync<Result>(request, ct);

        if (result is not ValidationFailure validationFailure)
        {
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        }

        var errors = validationFailure
            .Results.OfType<ValidationResult>()
            .ToDictionary(x => x.ErrorMessage!, x => x.MemberNames.ToArray());

        return Results.ValidationProblem(errors);
    }

    public void MapEndpoint(WebApplication app)
    {
        var builder = Map(app);

        if (!this.authorize)
        {
            return;
        }

        builder.RequireAuthorization(this.authorizationPolicies);
        builder.Produces<Success<TResponse>>();
    }

    protected abstract RouteHandlerBuilder Map(WebApplication app);

    protected void Authorize(params string[] policies)
    {
        this.authorize = true;
        this.authorizationPolicies = policies;
    }
}
