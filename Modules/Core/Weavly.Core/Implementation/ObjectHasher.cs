using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Weavly.Core.Shared.Contracts;

namespace Weavly.Core.Implementation;

public sealed class ObjectHasher : IObjectHasher
{
    public string ComputeHash<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);

        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}