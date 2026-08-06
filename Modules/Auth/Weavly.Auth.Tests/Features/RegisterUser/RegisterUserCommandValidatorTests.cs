using System.Linq.Expressions;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.RegisterUser;
using Weavly.Auth.Seeding;
using Weavly.Auth.Shared.Features.RegisterUser;
using Weavly.Configuration.Shared;
using Weavly.Configuration.Shared.Features.LoadConfiguration;
using Weavly.Core.Shared.Implementation;
using Wolverine;

namespace Weavly.Auth.Tests.Features.RegisterUser;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommand baseCommand = new("test@user.com", "");

    private readonly IMessageBus messageBusMock = Substitute.For<IMessageBus>();
    private readonly RegisterUserCommandValidator sut;

    public RegisterUserCommandValidatorTests()
    {
        this.sut = new RegisterUserCommandValidator(this.messageBusMock);
    }

    [Theory]
    [InlineData("Password", false)]
    [InlineData("Passw0rd", true)]
    public async Task RequireDigitTest(string password, bool expectSuccess)
    {
        this.messageBusMock.InvokeAsync<Result>(Arg.Is(LoadConfigurationCommandPredicate), Arg.Any<CancellationToken>())
            .Returns(MakeConfiguration("RequireDigit", true));

        var result = await this.sut.ValidateAsync(this.baseCommand with { Password = password });
        result.Success.ShouldBe(expectSuccess);
    }

    [Theory]
    [InlineData("password", false)]
    [InlineData("Password", true)]
    public async Task RequireUppercaseTest(string password, bool expectSuccess)
    {
        this.messageBusMock.InvokeAsync<Result>(Arg.Is(LoadConfigurationCommandPredicate), Arg.Any<CancellationToken>())
            .Returns(MakeConfiguration("RequireUppercase", true));

        var result = await this.sut.ValidateAsync(this.baseCommand with { Password = password });
        result.Success.ShouldBe(expectSuccess);
    }

    [Theory]
    [InlineData("PASSWORD", false)]
    [InlineData("Password", true)]
    public async Task RequireLowercaseTest(string password, bool expectSuccess)
    {
        this.messageBusMock.InvokeAsync<Result>(Arg.Is(LoadConfigurationCommandPredicate), Arg.Any<CancellationToken>())
            .Returns(MakeConfiguration("RequireLowercase", true));

        var result = await this.sut.ValidateAsync(this.baseCommand with { Password = password });
        result.Success.ShouldBe(expectSuccess);
    }

    [Theory]
    [InlineData("Password", false)]
    [InlineData("Password0", false)]
    [InlineData("Password!", true)]
    [InlineData("Password@", true)]
    [InlineData("Password#", true)]
    [InlineData("Password$", true)]
    [InlineData("Password%", true)]
    [InlineData("Password^", true)]
    [InlineData("Password&", true)]
    [InlineData("Password*", true)]
    [InlineData("Password_", true)]
    [InlineData("Password-", true)]
    [InlineData("Password.", true)]
    public async Task RequireNonAlphanumericTest(string password, bool expectSuccess)
    {
        this.messageBusMock.InvokeAsync<Result>(Arg.Is(LoadConfigurationCommandPredicate), Arg.Any<CancellationToken>())
            .Returns(MakeConfiguration("RequireNonAlphanumeric", true));

        var result = await this.sut.ValidateAsync(this.baseCommand with { Password = password });
        result.Success.ShouldBe(expectSuccess);
    }

    [Theory]
    [InlineData("Crabs", false)]
    [InlineData("Box", false)]
    [InlineData("Password", true)]
    [InlineData("Beanie", true)]
    public async Task MinimumLengthTest(string password, bool expectSuccess)
    {
        this.messageBusMock.InvokeAsync<Result>(Arg.Is(LoadConfigurationCommandPredicate), Arg.Any<CancellationToken>())
            .Returns(MakeConfiguration("MinimumLength", 6));

        var result = await this.sut.ValidateAsync(this.baseCommand with { Password = password });
        result.Success.ShouldBe(expectSuccess);
    }

    private static Expression<Predicate<LoadConfigurationCommand?>> LoadConfigurationCommandPredicate =>
        x => x!.Module == "AuthModule";

    [Theory]
    [InlineData("Crabs", true)]
    [InlineData("Box", true)]
    [InlineData("Password", false)]
    [InlineData("Beanie", true)]
    public async Task MaximumLengthTest(string password, bool expectSuccess)
    {
        this.messageBusMock.InvokeAsync<Result>(Arg.Is(LoadConfigurationCommandPredicate), Arg.Any<CancellationToken>())
            .Returns(MakeConfiguration("MaximumLength", 6));

        var result = await this.sut.ValidateAsync(this.baseCommand with { Password = password });
        result.Success.ShouldBe(expectSuccess);
    }

    [Theory]
    [InlineData("test@user.com", true)]
    [InlineData("@co.uk", false)]
    [InlineData("@some.gov", false)]
    [InlineData("spartan@.gov", false)]
    [InlineData("jjin@ban.co.jp", true)]
    [InlineData("no", false)]
    public async Task EmailTest(string email, bool expectSuccess)
    {
        this.messageBusMock.InvokeAsync<Result>(Arg.Is(LoadConfigurationCommandPredicate), Arg.Any<CancellationToken>())
            .Returns(MakeConfiguration("NothingSpecial", false));

        var result = await this.sut.ValidateAsync(this.baseCommand with { Email = email });
        result.Success.ShouldBe(expectSuccess);
    }

    private static Result MakeConfiguration(string name, object value)
    {
        var config = ConfigurationItem.Create(name);
        var configWithValue = value switch
        {
            string s => config with { StringValue = s, Category = ConfigCategory.PasswordRules },
            int i => config with { IntValue = i, Category = ConfigCategory.PasswordRules },
            bool b => config with { BoolValue = b, Category = ConfigCategory.PasswordRules },
            double d => config with { DoubleValue = d, Category = ConfigCategory.PasswordRules },
            _ => throw new InvalidOperationException("Unsupported configuration value type"),
        };

        return Result.Success(new LoadConfigurationResponse("AuthModule", [configWithValue]));
    }
}
