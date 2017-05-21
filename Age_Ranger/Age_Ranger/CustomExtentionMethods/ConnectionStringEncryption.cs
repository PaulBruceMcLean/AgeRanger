using System;
using System.Security.Cryptography;
using System.IO;

namespace Age_Ranger.CustomExtentionMethods
{
    public static class ConnectionStringEncryption
    {
        public static string DecryptConnectionString(this string EncryptedConnection)
        {
            if (string.IsNullOrEmpty(EncryptedConnection))
            {
                return "Input Can not be Null or Empty";
            }
            string UnHashedConnectionString;
            TripleDES Triple_Des = TripleDES.Create();
            Triple_Des.Key = new byte[]{1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12,13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24};
            Triple_Des.IV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(EncryptedConnection)))
            {
                using (CryptoStream cs = new CryptoStream(ms, Triple_Des.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(cs))
                    {
                        UnHashedConnectionString = srDecrypt.ReadToEnd();
                    }
                }
            }
            return UnHashedConnectionString;
        }

        public static string EncryptConnectionString(this string DecryptedConnection)
        {

            if (string.IsNullOrEmpty(DecryptedConnection))
            {
                return "Input Can Not be Null or Empty";
            }
            string EncryptedString;

            TripleDES Triple_Des = TripleDES.Create();
            Triple_Des.Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
            Triple_Des.IV = new byte[] { 8, 7, 6, 5, 4, 3, 2, 1 };
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, Triple_Des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(cs))
                    {
                        swEncrypt.Write(DecryptedConnection);
                    }
                    EncryptedString = string.Concat(Convert.ToBase64String(ms.ToArray()));

                }
            }
            return EncryptedString;
        }
    }
}