using Shouldly;
using Weavly.Core.Implementation;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Core.Tests.Implementation;

public sealed class TemplateServiceTests
{
    private readonly TemplateService sut = new();

    [Fact]
    public async Task RenderAsync_ProperlyRenders_TemplateWithMatchingModel()
    {
        const string template = "This is {{Word}}!";

        var result = await this.sut.RenderAsync(template, new { Word = "Cheesecake" });

        result.ShouldBeOfType<Success<string>>().Data.ShouldBe("This is Cheesecake!");
    }

    [Fact]
    public async Task RenderAsync_Fails_WhenTemplate_IsMalformed()
    {
        const string template = "This is {{Word}!";

        var result = await this.sut.RenderAsync(template, new { Word = "Cheesecake" });

        result.ShouldBeOfType<Failure>();
    }
}
