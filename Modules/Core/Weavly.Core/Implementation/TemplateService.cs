using Fluid;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Implementation;

internal sealed class TemplateService : ITemplateService
{
    private readonly FluidParser parser = new();

    public async Task<Result> RenderAsync(string template, object model)
    {
        try
        {
            if (!this.parser.TryParse(template, out var fluidTemplate, out var error))
            {
                return Result.Failure(error);
            }

            var context = new TemplateContext(model);
            var renderedTemplate = await fluidTemplate.RenderAsync(context);

            return Result.Success(renderedTemplate);
        }
        catch (Exception e)
        {
            return Result.Failure(e);
        }
    }
}
