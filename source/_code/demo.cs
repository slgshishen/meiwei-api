using System;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;

namespace ConsoleApplication1
{
    class SimpleStringCypher
    {
        private RijndaelManaged RM;

        public SimpleStringCypher(String secret)
        {
            //secret为密钥 S2
            var keyBytes = PrepareAesKey(secret);

            RM = new System.Security.Cryptography.RijndaelManaged
            {
                Mode = System.Security.Cryptography.CipherMode.ECB,
                Padding = System.Security.Cryptography.PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        public string Encrypt(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext)) return null;
            Byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            ICryptoTransform cTransform = RM.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(plaintextBytes,
                        0, plaintextBytes.Length);

            return URLSafeBase64Reflow(Convert.ToBase64String(resultArray,
                        0, resultArray.Length));
        }

        public string Decrypt(string codedText)
        {
            if (string.IsNullOrEmpty(codedText)) return null;
            Byte[] toDeryptArray = Convert.FromBase64String(
                        AutomaticallyPad(NormalBase64Reflow(codedText)));

            ICryptoTransform cTransform = RM.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toDeryptArray,
                        0, toDeryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        private static string AutomaticallyPad(string base64)
        {
            return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
        }

        private static string URLSafeBase64Reflow(string base64)
        {
            return base64.Replace("=", String.Empty).Replace('+', '-').Replace('/', '_');
        }

        private static string NormalBase64Reflow(string base64)
        {
            return base64.Replace("=", String.Empty).Replace('-', '+').Replace('_', '/');
        }

        private static byte[] PrepareAesKey(string key)
        {
            Byte[] keyBinary = Convert.FromBase64String(
                        AutomaticallyPad(NormalBase64Reflow(key)));
            var keyBytes = new byte[16];
            Array.Copy(keyBinary, keyBytes, Math.Min(keyBytes.Length, keyBinary.Length));
            return keyBytes;
        }
    }
}