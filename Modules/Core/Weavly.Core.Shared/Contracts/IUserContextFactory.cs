namespace Weavly.Core.Shared.Contracts;

public interface IUserContextFactory<out TUserId>
    where TUserId : struct
{
    IUserContext<TUserId> CreateUserContext();
}
