using Manager.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Manager.Core.Services
{
    public class CryptographyService : ICryptographyService
    {
        private readonly IConfiguration _configuration;

        private readonly byte[] _encryptionBytes = [0x50, 0x08, 0xF1, 0xDD, 0xDE, 0x3C, 0xF2, 0x18, 0x44, 0x74, 0x19, 0x2C, 0x53, 0x49, 0xAB, 0xBC];
        private readonly string _encryptionKey;

        public CryptographyService(IConfiguration configuration)
        {
            _configuration = configuration;

            _encryptionKey = _configuration["EncryptionKey"]!;
        }

        public string Encrypt(string text)
        {
            byte[] encryptionKeyBytes = Convert.FromBase64String(_encryptionKey);
            byte[] textBytes = new UTF8Encoding().GetBytes(text);

            RijndaelManaged rijndaelAlgorithm = new()
            {
                KeySize = 256
            };

            var memoryStream = new MemoryStream();

            var encryptor = new CryptoStream(
                memoryStream,
                rijndaelAlgorithm.CreateEncryptor(encryptionKeyBytes, _encryptionBytes),
                CryptoStreamMode.Write);

            encryptor.Write(textBytes, 0, textBytes.Length);
            encryptor.FlushFinalBlock();

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string text)
        {
            byte[] encryptionKeyBytes = Convert.FromBase64String(_encryptionKey);
            byte[] textBytes = Convert.FromBase64String(text);

            RijndaelManaged rijndaelAlgorithm = new()
            {
                KeySize = 256
            };

            var memoryStream = new MemoryStream();

            var decryptor = new CryptoStream(
                memoryStream,
                rijndaelAlgorithm.CreateDecryptor(encryptionKeyBytes, _encryptionBytes),
                CryptoStreamMode.Write);

            decryptor.Write(textBytes, 0, textBytes.Length);
            decryptor.FlushFinalBlock();

            var utf8 = new UTF8Encoding();

            return utf8.GetString(memoryStream.ToArray());
        }
    }
}
