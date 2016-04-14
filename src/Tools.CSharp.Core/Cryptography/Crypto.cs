using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tools.CSharp.Cryptography
{
    public sealed class Crypto
    {
        #region private
        private readonly string _password;
        private readonly byte[] _salt;
        //---------------------------------------------------------------------
        private static byte[] _encrypt(byte[] data, SymmetricAlgorithm algorithm)
        {
            byte[] encodeData;
            using(var encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
            {
                using(var memoryStream = new MemoryStream())
                {
                    using(var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    { cryptoStream.Write(data, 0, data.Length); }
                    encodeData = memoryStream.ToArray();
                }
            }
            return encodeData;
        }
        private static byte[] _decrypt(byte[] data, SymmetricAlgorithm algorithm)
        {
            byte[] decodeData;
            using(var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            {
                using(var memoryStream = new MemoryStream())
                {
                    using(var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    { cryptoStream.Write(data, 0, data.Length); }
                    decodeData = memoryStream.ToArray();
                }
            }
            return decodeData;
        }
        //---------------------------------------------------------------------
        private SymmetricAlgorithm _createAlgorithm(string password, byte[] salt)
        {
            var keyDerive = new Rfc2898DeriveBytes(password, salt);

            var algorithm = new RijndaelManaged();
            algorithm.Key = keyDerive.GetBytes(algorithm.KeySize / 8);
            algorithm.IV = keyDerive.GetBytes(algorithm.BlockSize / 8);

            return algorithm;
        }
        #endregion
        public Crypto()
        {
            _password = string.Empty;
            _salt = null;
        }
        public Crypto(string password, string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
            { throw new ArgumentNullException(nameof(password)); }
            if (string.IsNullOrWhiteSpace(salt))
            { throw new ArgumentNullException(nameof(salt)); }

            _password = password;
            _salt = Encoding.Unicode.GetBytes(salt);
        }

        //---------------------------------------------------------------------
        public string Encrypt(string text)
        {
            return Encrypt(text, _password, _salt);
        }
        public string Encrypt(string text, string password, byte[] salt)
        {
            if (string.IsNullOrWhiteSpace(text))
            { throw new ArgumentNullException(nameof(text)); }
            if (string.IsNullOrWhiteSpace(password))
            { throw new ArgumentNullException(nameof(password)); }
            if (salt == null)
            { throw new ArgumentNullException(nameof(salt)); }

            string result;
            var algorithm = _createAlgorithm(password, salt);
            try
            {
                var data = Encoding.Unicode.GetBytes(text);
                var encryptData = _encrypt(data, algorithm);
                result = Convert.ToBase64String(encryptData);
            }
            finally
            { algorithm?.Clear(); }
            return result;
        }
        //---------------------------------------------------------------------
        public string Descrypt(string text)
        {
            return Descrypt(text, _password, _salt);
        }
        public string Descrypt(string text, string password, byte[] salt)
        {
            if (string.IsNullOrWhiteSpace(text))
            { throw new ArgumentNullException(nameof(text)); }
            if (string.IsNullOrWhiteSpace(password))
            { throw new ArgumentNullException(nameof(password)); }
            if (salt == null)
            { throw new ArgumentNullException(nameof(salt)); }

            string result;
            var algorithm = _createAlgorithm(password, salt);
            try
            {
                var data = Convert.FromBase64String(text);
                var decryptData = _decrypt(data, algorithm);
                result = Encoding.Unicode.GetString(decryptData);
            }
            finally
            { algorithm?.Clear(); }
            return result;
        }
        //---------------------------------------------------------------------
    }
}
