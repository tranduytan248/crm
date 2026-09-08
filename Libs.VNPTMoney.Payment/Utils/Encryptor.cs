using System.Security.Cryptography;
using System.Text;
using System;

namespace Libs.VNPTMoney.Payment.Utils
{
    public class Encryptor
    {
        public static string SHA256Hash(string value)
        {
            StringBuilder sb = new StringBuilder();

            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}