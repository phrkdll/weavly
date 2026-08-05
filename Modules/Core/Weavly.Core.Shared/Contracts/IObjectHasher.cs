namespace Weavly.Core.Shared.Contracts;

public interface IObjectHasher
{
    string ComputeHash<T>(T obj);
}
