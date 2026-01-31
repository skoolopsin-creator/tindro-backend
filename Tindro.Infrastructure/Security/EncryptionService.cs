using Tindro.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Tindro.Infrastructure.Security;

public class EncryptionService : IEncryptionService
{
    private readonly string _key;
    private readonly string _iv;

    public EncryptionService(string key, string iv)
    {
        _key = key ?? throw new ArgumentNullException(nameof(key));
        _iv = iv ?? throw new ArgumentNullException(nameof(iv));
    }

    public string Encrypt(string plaintext)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(_key.PadRight(32).Substring(0, 32));
            aes.IV = Encoding.UTF8.GetBytes(_iv.PadRight(16).Substring(0, 16));

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plaintext);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
    }

    public string Decrypt(string ciphertext)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(_key.PadRight(32).Substring(0, 32));
            aes.IV = Encoding.UTF8.GetBytes(_iv.PadRight(16).Substring(0, 16));

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                using (var ms = new MemoryStream(Convert.FromBase64String(ciphertext)))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
