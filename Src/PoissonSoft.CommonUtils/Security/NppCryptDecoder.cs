using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using PoissonSoft.CommonUtils.Cryptography;

namespace PoissonSoft.CommonUtils.Security
{
    /// <summary>
    /// Utility for decrypting files encrypted by the NppCrypt plugin for Notepad++
    /// </summary>
    public static class NppCryptDecoder
    {
        /// <summary>
        /// Get the contents of the encrypted file as text 
        /// </summary>
        /// <param name="fileName">Name of the file to be read</param>
        /// <param name="password">Password to decrypt the file</param>
        /// <param name="encoding">File encoding (UTF8 by default)</param>
        /// <returns></returns>
        public static string ReadAllFileAsText(string fileName, string password, Encoding encoding = null)
        {
            return (encoding ?? Encoding.UTF8).GetString(ReadAllFileAsBinary(fileName, password));
        }

        /// <summary>
        /// Get the contents of the encrypted file as byte array
        /// </summary>
        /// <param name="fileName">Name of the file to be read</param>
        /// <param name="password">Password to decrypt the file</param>
        /// <returns></returns>
        public static byte[] ReadAllFileAsBinary(string fileName, string password)
        {
            if (!File.Exists(fileName)) throw new FileNotFoundException("Encrypted file does not found", fileName);

            var encryptedContent = ParseEncryptedFileContent(fileName);
            var cipherParams = ParseCipherParams(encryptedContent.NppCryptXml);

            return Decrypt(cipherParams, encryptedContent.EncryptedData, password);
        }

        private static (string NppCryptXml, byte[] EncryptedData) ParseEncryptedFileContent(string fileName)
        {
            string encryptedContent;
            try
            {
                encryptedContent = File.ReadAllText(fileName);
            }
            catch (Exception e)
            {
                throw new Exception($"Can not read encrypted file '{fileName}'", e);
            }

            // ReSharper disable once StringLiteralTypo
            const string nppCryptCloseTag = "</nppcrypt>";

            var p = encryptedContent.IndexOf(nppCryptCloseTag, StringComparison.InvariantCultureIgnoreCase);
            if (p <= 0) throw new Exception($"NppCrypt-header does not found in file '{fileName}'. " +
                                            $"Maybe this file is not encrypted or it was encrypted by some other means, not NppCrypt.");
            p += nppCryptCloseTag.Length;

            var nppCryptXml = encryptedContent.Substring(0, p);
            var dataStr = encryptedContent.Substring(p);

            byte[] encryptedData;
            try
            {
                encryptedData = Convert.FromBase64String(dataStr);
            }
            catch (Exception e)
            {
                throw new Exception("Can not decode encrypted data from Base64-string. Maybe the file would have been corrupted.", e);
            }

            return (nppCryptXml, encryptedData);

        }

        private static byte[] Decrypt(NppCryptCipherParams cipher, byte[] encryptedData, string password)
        {
            if (cipher.Encryption.Cipher != "rijndael")
                throw new Exception($"Cipher algorithm \"{cipher.Encryption.Cipher}\" is not supported now. " +
                                    "Only \"rijndael\" algorithm might be used in the current version");

            if (cipher.Encryption.Mode != CipherMode.CBC)
                throw new Exception($"Cipher algorithm mode \"{cipher.Encryption.Mode}\" is not supported now. " +
                                    $"Only \"{nameof(CipherMode.CBC)}\" mode might be used in the current version");


            if (cipher.Key.Algorithm != "scrypt")
                throw new Exception($"Key hashing algorithm \"{cipher.Key.Algorithm}\" is not supported now. " +
                                    "Only \"scrypt\" algorithm might be used in the current version");

            if (!cipher.Key.N.HasValue)
                throw new Exception($"Parameter '{nameof(cipher.Key.N)}' of key hashing algorithm is not set");
            if (!cipher.Key.R.HasValue)
                throw new Exception($"Parameter '{nameof(cipher.Key.R)}' of key hashing algorithm is not set");
            if (!cipher.Key.P.HasValue)
                throw new Exception($"Parameter '{nameof(cipher.Key.P)}' of key hashing algorithm is not set");
            if (cipher.Key.Salt == null)
                throw new Exception($"Parameter '{nameof(cipher.Key.Salt)}' of key hashing algorithm is not set");

            var key = ScryptEncoder.CryptoScrypt(Encoding.UTF8.GetBytes(password), cipher.Key.Salt,
                cipher.Key.N.Value, cipher.Key.R.Value, cipher.Key.P.Value, cipher.Encryption.KeyLength ?? 32);

            using Rijndael rijAlg = Rijndael.Create();
            rijAlg.Key = key;
            rijAlg.IV = cipher.Iv.Value;
            rijAlg.Mode = cipher.Encryption.Mode;

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, rijAlg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(encryptedData, 0, encryptedData.Length);
            cs.Close();
            var res = ms.ToArray();
            ms.Close();
            return res;
        }

        private static NppCryptCipherParams ParseCipherParams(string nppCryptXml)
        {
            XmlDocument doc;
            try
            {
                doc = new XmlDocument();
                doc.LoadXml(nppCryptXml);
            }
            catch (Exception e)
            {
                throw new Exception("NppCrypt-header parsing exception", e);
            }

            var res = new NppCryptCipherParams
            {
                Encryption = new NppCryptEncryptionParams(),
                Key = new NppCryptKeyParams(),
                Iv = new NppCryptIvParams()
            };


            XmlNode getTag(XmlNode parent, string tagName)
            {
                using (var tagCollection = (parent as XmlElement)?.GetElementsByTagName(tagName))
                {
                    if (tagCollection?.Count != 1)
                        throw new Exception($"NppCrypt-header parsing exception. The \"<{tagName}>\" tag was not found " +
                                            "or is present in several instances");
                    return tagCollection[0];
                }
            }

            string getAttribute(XmlNode parent, string attributeName)
            {
                return parent?.Attributes?[attributeName]?.Value;
            }

            byte[] parseBase64String(string str)
            {
                try
                {
                    return Convert.FromBase64String(str);
                }
                catch
                {
                    return null;
                }
            }

            CipherMode parseMode(string mode)
            {
                var m = mode.Trim().ToUpperInvariant();
                var modes = Enum.GetValues(typeof(CipherMode)).Cast<CipherMode>().ToList();
                foreach (var cipherMode in modes)
                {
                    if (cipherMode.ToString().ToUpperInvariant() == m) return cipherMode;
                }
                throw new Exception($"Unknown cipher mode '{mode}'");
            }


            var encryptionTag = getTag(doc.DocumentElement,"encryption");
            res.Encryption.Cipher = getAttribute(encryptionTag, "cipher");
            if (int.TryParse(getAttribute(encryptionTag, "key-length"), out int keyLen))
                res.Encryption.KeyLength = keyLen;
            res.Encryption.Mode = parseMode(getAttribute(encryptionTag, "mode"));
            res.Encryption.Encoding = getAttribute(encryptionTag, "encoding");

            var keyTag = getTag(doc.DocumentElement, "key");
            res.Key.Algorithm = getAttribute(keyTag, "algorithm");
            if (int.TryParse(getAttribute(keyTag, "N"), out int n))
                res.Key.N = n;
            if (int.TryParse(getAttribute(keyTag, "r"), out int r))
                res.Key.R = r;
            if (int.TryParse(getAttribute(keyTag, "p"), out int p))
                res.Key.P = p;
            res.Key.Salt = parseBase64String(getAttribute(keyTag, "salt"));

            var ivTag = getTag(doc.DocumentElement, "iv");
            res.Iv.Value = parseBase64String(getAttribute(ivTag, "value"));
            res.Iv.Method = getAttribute(ivTag, "method");

            return res;
        }

        private class NppCryptCipherParams
        {
            public NppCryptEncryptionParams Encryption { get; set; }
            public NppCryptKeyParams Key { get; set; }
            public NppCryptIvParams Iv { get; set; }
        }

        private class NppCryptEncryptionParams
        {
            public string Cipher { get; set; }
            public int? KeyLength { get; set; }
            public CipherMode Mode { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Encoding { get; set; }
        }

        private class NppCryptKeyParams
        {
            public string Algorithm { get; set; }

            public int? N { get; set; }
            public int? R { get; set; }
            public int? P { get; set; }
            public byte[] Salt { get; set; }
        }

        private class NppCryptIvParams
        {
            public byte[] Value { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Method { get; set; }
        }
    }
}
