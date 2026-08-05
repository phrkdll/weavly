namespace Weavly.Core.Shared.Contracts;

public interface IUserContext<out TUserId>
    where TUserId : struct
{
    public TUserId UserId { get; }
}
