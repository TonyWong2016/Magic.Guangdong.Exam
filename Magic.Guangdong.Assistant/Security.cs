using Magic.Guangdong.Assistant.Dto;
using MassTransit;
using NPOI.OpenXmlFormats.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Magic.Guangdong.Assistant
{
    public class Security
    {
        //private readonly byte[] _key = Encoding.UTF8.GetBytes("YourSecretKey32CharactersLong"); // 你的密钥
        //private readonly byte[] _iv = Encoding.UTF8.GetBytes("YourInitializationVector16CharactersLong"); // 你的初始化向量

        public static string Encrypt(string plainText, byte[] _key,byte[] _iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, byte[] _key, byte[] _iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                var cipherBytes = Convert.FromBase64String(cipherText);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static byte[] GenerateKey(int keySizeInBits)
        {
            using (Aes aes = Aes.Create())
            {
                // AES默认支持128, 192, 或 256位密钥长度
                aes.KeySize = keySizeInBits;

                // 生成密钥
                byte[] key = new byte[aes.KeySize / 8];
                RandomNumberGenerator.Fill(key);

                return key;
            }
        }


        #region sh1 和用户中心保持一致
        public static string Sha1(string privatekey, string plainText)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] str01 = Encoding.Default.GetBytes(privatekey + plainText);
            byte[] str02 = sha1.ComputeHash(str01);
            var result = BitConverter.ToString(str02).Replace("-", "").ToLower();
            return result;
        }

        public static string SHA1(string content)
        {
            return SHA1(content, Encoding.UTF8);
        }
        /// <summary>
        /// SHA1 加密
        /// </summary>
        /// <param name="content">需要加密字符串</param>
        /// <param name="encode">指定加密编码</param>
        /// <returns>返回40位小写字符串</returns>
        public static string SHA1(string content, Encoding encode)
        {
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(content);
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "").ToLower();//转小写
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string GenerateMD5Hash(string input)
        {
            // 创建一个MD5哈希实例
            using (var md5 = MD5.Create())
            {
                // 将输入转换为字节数组
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // 计算哈希
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // 将哈希字节数组转换为十六进制字符串形式
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public static string GetMD5HashFromStream(Stream file)
        {
            try
            {
                MD5 md5 = MD5.Create();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromStream() fail, error:" + ex.Message);
            }
        }

        public static async Task<string> GetFileMD5(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file does not exist.", filePath);
            }

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes =await md5.ComputeHashAsync(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static MaskDataDto getMaskData(MaskDataDto dto)
        {
            if (dto.valid)
            {
                //dto.encryptText = Assistant.Security.Encrypt(dto.text, Encoding.UTF8.GetBytes(dto.keyId), Encoding.UTF8.GetBytes(dto.keySecret));

                var textParts = new List<string>()
                    {
                        dto.text.Substring(0, (int)dto.firstPoint),
                        dto.text.Substring((int)dto.lastPoint, dto.text.Length - (int)dto.lastPoint)
                    };

                
            }
            return dto;
        }
    }

    


}
