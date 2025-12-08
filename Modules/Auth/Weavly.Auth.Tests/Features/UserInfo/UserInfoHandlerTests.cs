using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;
using Weavly.Auth.Features.UserInfo;
using Weavly.Auth.Models;
using Weavly.Auth.Persistence;
using Weavly.Auth.Shared.Features.UserInfo;
using Weavly.Auth.Shared.Identifiers;
using Weavly.Core.Shared.Contracts;
using Weavly.Core.Shared.Implementation;

namespace Weavly.Auth.Tests.Features.UserInfo;

public sealed class UserInfoHandlerTests
{
    private readonly IUserContext<AppUserId> userContextMock = Substitute.For<IUserContext<AppUserId>>();

    private readonly AuthDbContext dbContextMock;

    private readonly UserInfoHandler sut;

    public UserInfoHandlerTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseStronglyTypeConverters();

        dbContextMock = Create.MockedDbContextFor<TestAuthDbContext>(dbContextOptions.Options);

        sut = new UserInfoHandler(dbContextMock, userContextMock);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_SuccessInstance_ForExistingUser()
    {
        var user = AppUser.Create("admin@test.local", []);
        dbContextMock.Users.AddRange(user);
        dbContextMock.SaveChanges();

        userContextMock.UserId.Returns(user.Id);

        var result = await sut.HandleAsync(new UserInfoCommand(), CancellationToken.None);

        var data = result.ShouldBeOfType<Success<UserInfoResponse>>().Data.ShouldBeOfType<UserInfoResponse>();
        data.Email.ShouldBe(user.Email);
        data.Id.ShouldBe(user.Id);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturn_FailureInstance_ForNonExistingUser()
    {
        userContextMock.UserId.Returns(new AppUserId());

        var result = await sut.HandleAsync(new UserInfoCommand(), CancellationToken.None);

        result.ShouldBeOfType<Failure>();
    }
}
