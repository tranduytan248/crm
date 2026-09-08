using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TSFramework.Libs.Crypto;

namespace TSFramework.Libs.Utils
{
    public static class UtilEncrypt
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int KEYSIZE = 256;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DERIVATION_ITERATIONS = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.  
            var saltStringBytes = Generate256BitsOfRandomEntropy(KEYSIZE);
            var ivStringBytes = Generate256BitsOfRandomEntropy(KEYSIZE);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DERIVATION_ITERATIONS))
            {
                var keyBytes = password.GetBytes(KEYSIZE / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = KEYSIZE;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase, out bool isSuccess)
        {
            try
            {
                isSuccess = true;
                // Get the complete stream of bytes that represent:
                // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
                var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
                // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
                var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KEYSIZE / 8).ToArray();
                // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
                var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KEYSIZE / 8).Take(KEYSIZE / 8).ToArray();
                // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
                var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(KEYSIZE / 8 * 2)
                    .Take(cipherTextBytesWithSaltAndIv.Length - KEYSIZE / 8 * 2).ToArray();

                using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DERIVATION_ITERATIONS))
                {
                    var keyBytes = password.GetBytes(KEYSIZE / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = KEYSIZE;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream =
                                       new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount =
                                        cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                isSuccess = false;
                return null;
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy(int keySize)
        {
            var keyLenght = keySize / 8;
            var randomBytes = new byte[keyLenght]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        public static string FromObject(object obj)
        {
            var jsonObject = JsonConvert.SerializeObject(obj);
            if (string.IsNullOrEmpty(jsonObject)) return string.Empty;

            return CalculateMD5Hash(jsonObject);
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            foreach (var t in hash)
                sb.Append(t.ToString("X2"));

            return sb.ToString();
        }

        public static string GenerateHash(string sVal)
        {
            ICryptoService cryptoService = new Pbkdf2Crypto();
            return cryptoService.Compute(sVal);
        }

        public static string GenerateSalt()
        {
            ICryptoService cryptoService = new Pbkdf2Crypto();
            return cryptoService.GenerateSalt(32);
        }

        public static string GenerateCryptoPassword(string password, string salt)
        {
            var algorithm = new SHA256CryptoServiceProvider();//SHA1CryptoServiceProvider(); //or use SHA256.Create();
            var hashData = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password + salt));

            var sb = new StringBuilder();
            foreach (var b in hashData)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static bool IsCompare(string cryptoVal1, string cryptoVals)
        {
            ICryptoService cryptoService = new Pbkdf2Crypto();
            return cryptoService.Compare(cryptoVal1, cryptoVals);
        }
    }
}