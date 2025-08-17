using System.Security.Cryptography;
using System.Text;

namespace ProjectTemplate.Utils;

public static class Password
{
    // Deriva 32 bytes (256 bits) con 10k iteraciones; ajustable
    public static string Hash(string password, string salt, int iterations = 10_000, int size = 32)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(size);
        return Convert.ToBase64String(hash);
    }

    public static string NewSalt(int size = 16)
    {
        var salt = new byte[size];
        RandomNumberGenerator.Fill(salt);
        return Convert.ToBase64String(salt);
    }

    public static bool Verify(string password, string salt, string expectedHash, int iterations = 10_000, int size = 32)
    {
        var actual = Hash(password, salt, iterations, size);
        return CryptographicOperations.FixedTimeEquals(Convert.FromBase64String(actual), Convert.FromBase64String(expectedHash));
    }
}
