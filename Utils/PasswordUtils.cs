using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Auth.Utils;

/// <summary>
/// This class is used to encapsulate the password hashing and verification logic.
/// </summary>
public static class PasswordUtils
{
    public static (string Hash, string Salt) HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32
        );
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    /// <summary>
    /// This method verifies a password against a stored hash and salt.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="storedHash">The stored hash.</param>
    /// <param name="storedSalt">The stored salt.</param>
    /// <returns>True if the password is valid, false otherwise.</returns>
    public static bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] salt = Convert.FromBase64String(storedSalt);
        byte[] hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32
        );
        return Convert.ToBase64String(hash) == storedHash;
    }
}