using System.Security.Cryptography;
using System.Text;

namespace CryptoBackend.Services;

public static class CryptoService
{
    public static string AesEncrypt(string text, string key)
    {
        // Create a buffer from the key (using SHA-256 hash of the key for consistent length)
        byte[] keyHash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        
        // Generate a random initialization vector
        byte[] iv = RandomNumberGenerator.GetBytes(16);
        
        // Create encryptor
        using Aes aes = Aes.Create();
        aes.Key = keyHash;
        aes.IV = iv;
        
        // Encrypt the data
        using ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] encrypted = encryptor.TransformFinalBlock(Encoding.UTF8.GetBytes(text), 0, text.Length);
        
        // Return both the IV and encrypted content (IV is needed for decryption)
        return Convert.ToHexString(iv).ToLower() + ":" + Convert.ToHexString(encrypted).ToLower();
    }

    public static string AesDecrypt(string text, string key)
    {
        // Split the stored result into IV and encrypted text
        string[] parts = text.Split(':');
        if (parts.Length != 2)
        {
            throw new InvalidOperationException("Invalid encrypted text format");
        }
        
        byte[] iv = Convert.FromHexString(parts[0]);
        byte[] encrypted = Convert.FromHexString(parts[1]);
        
        // Create a buffer from the key (using SHA-256 hash of the key for consistent length)
        byte[] keyHash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        
        // Create decryptor
        using Aes aes = Aes.Create();
        aes.Key = keyHash;
        aes.IV = iv;
        
        // Decrypt the data
        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] decrypted = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
        
        return Encoding.UTF8.GetString(decrypted);
    }

    public static string HashSHA256(string text)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(hash).ToLower();
    }

    public static string HashMD5(string text)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(hash).ToLower();
    }

    public static string EncodeBase64(string text)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
    }

    public static string DecodeBase64(string text)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(text));
    }
}