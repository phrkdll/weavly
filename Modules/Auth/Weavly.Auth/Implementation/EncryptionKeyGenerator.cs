using System.Security.Cryptography;

namespace Weavly.Auth.Implementation;

internal static class EncryptionKeyGenerator
{
    public static string GenerateAesKey(int keySize)
    {
        using var aes = Aes.Create();

        aes.KeySize = keySize;
        aes.GenerateKey();

        return Convert.ToBase64String(aes.Key);
    }
}
