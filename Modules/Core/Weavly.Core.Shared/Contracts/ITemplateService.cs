namespace Weavly.Core.Shared.Contracts;

public interface ITemplateService
{
    Task<Result> RenderAsync(string template, object model);
}
