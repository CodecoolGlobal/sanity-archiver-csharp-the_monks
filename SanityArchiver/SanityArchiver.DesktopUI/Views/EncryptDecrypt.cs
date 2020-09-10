using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SanityArchiver.DesktopUI.Views
{
    public static class EncryptDecrypt
    {

        public static void EncryptFile(string FilePath, string Key) 
        {
            byte[] plainContent = File.ReadAllBytes(FilePath);
            using (var DES = new DESCryptoServiceProvider()) 
            {
                DES.IV = Encoding.UTF8.GetBytes(Key);
                DES.Key = Encoding.UTF8.GetBytes(Key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;

                using (var memStream = new MemoryStream()) 
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateEncryptor(),
                        CryptoStreamMode.Write);
                    cryptoStream.Write(plainContent, 0, plainContent.Length);
                    cryptoStream.FlushFinalBlock();
                    File.WriteAllBytes(FilePath, memStream.ToArray());
                    
                }
            }
        }

        public static void DecryptFile(string FilePath, string Key)
        {
            byte[] encrypted = File.ReadAllBytes(FilePath);
            using (var DES = new DESCryptoServiceProvider())
            {
                DES.IV = Encoding.UTF8.GetBytes(Key);
                DES.Key = Encoding.UTF8.GetBytes(Key);
                DES.Mode = CipherMode.CBC;
                DES.Padding = PaddingMode.PKCS7;

                using (var memStream = new MemoryStream())
                {
                    CryptoStream cryptoStream = new CryptoStream(memStream, DES.CreateDecryptor(),
                        CryptoStreamMode.Write);
                    cryptoStream.Write(encrypted, 0, encrypted.Length);
                    cryptoStream.FlushFinalBlock();
                    File.WriteAllBytes(FilePath, memStream.ToArray());

                }
            }
        }
    }
}
