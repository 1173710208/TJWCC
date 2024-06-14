using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace TJWCC.Code
{
    public class AESUtil
    {
        /// <summary>
        /// 有密码的AES加密 
        /// </summary>
        /// <param name="text">加密字符数组</param>
        /// <param name="password">加密的密码</param>
        /// <returns>被加密后的字符串</returns>
        public static string Encrypt(byte[] toEncrypt, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = new Byte[toEncrypt.Length];
            Array.Copy(toEncrypt, toEncryptArray, toEncrypt.Length);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.IV= keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="text"></param>
        /// <param name="password"></param>
        /// <returns>HexString</returns>
        public static string Decrypt(byte[] toDecrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            //byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            byte[] toDecryptArray = new Byte[toDecrypt.Length];
            Array.Copy(toDecrypt, toDecryptArray, toDecrypt.Length);


            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            

            try
            {
                
                byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                return BytesToHexString(resultArray);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} 可能是密码错误！！！", e.Message);
            }
            return "";
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="Data">被加密的明文</param>
        /// <param name="Key">密钥</param>
        /// <param name="Vector">向量</param>
        /// <returns>密文</returns>
        public static Byte[] AESEncrypt(Byte[] Data, String Key, String Vector)
        {
            Byte[] bKey = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            //如果Vector为空，则vector等于key
            if (Vector.Length!=0)
            {
                Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);
            }
            else
            {
                Array.Copy(bKey, bVector, bVector.Length);
            }

            Byte[] Cryptograph = null; // 加密后的密文

            Rijndael Aes = Rijndael.Create();
            Aes.Key = bKey;
            Aes.IV = bVector;
            Aes.Mode = CipherMode.ECB;
            Aes.Padding = PaddingMode.PKCS7;
            try
            {
                // 开辟一块内存流
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                     Aes.CreateEncryptor(bKey, bVector),
                     CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        Encryptor.Write(Data, 0, Data.Length);
                        Encryptor.FlushFinalBlock();

                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }

            return Cryptograph;
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="Data">被解密的密文</param>
        /// <param name="Key">密钥</param>
        /// <param name="Vector">向量</param>
        /// <returns>明文</returns>
        public static Byte[] AESDecrypt(Byte[] Data, String Key, String Vector)
        {
            Byte[] bKey = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            //如果Vector为空，则vector等于key
            if (Vector.Length != 0)
            {
                Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);
            }
            else
            {
                Array.Copy(bKey, bVector, bVector.Length);
            }

            Byte[] original = null; // 解密后的明文

            Rijndael Aes = Rijndael.Create();
            Aes.Key = bKey;
            Aes.IV = bVector;
            Aes.Mode = CipherMode.ECB;
            Aes.Padding = PaddingMode.PKCS7;
            try
            {
                // 开辟一块内存流，存储密文
                using (MemoryStream Memory = new MemoryStream(Data))
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Decryptor = new CryptoStream(Memory,
                    Aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                    {
                        // 明文存储区
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            Byte[] Buffer = new Byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }
                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                original = null;
            }

            return original;
        }

        /// <summary>
        /// 将16进制的字符串转为byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 将byte[]转为16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        /// <summary>
        /// 将16进制字符串转为字符串
        /// </summary>
        /// <param name="hs"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string HexStringToString(string hs, Encoding encode)
        {
            string strTemp = "";
            byte[] b = new byte[hs.Length / 2];
            for (int i = 0; i < hs.Length / 2; i++)
            {
                strTemp = hs.Substring(i * 2, 2);
                b[i] = Convert.ToByte(strTemp, 16);
            }
            //按照指定编码将字节数组变为字符串
            return encode.GetString(b);
        }

        /// <summary>
        /// 将字符串转为16进制字符，允许中文
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string StringToHexString(string s, Encoding encode)
        {
            byte[] b = encode.GetBytes(s);      //按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)  //逐字节变为16进制字符
            {
                result += Convert.ToString(b[i], 16);
            }
            return result;
        }
        //如：
        //注意，一个中文转为utf-8占三个字节，英文占一个字节
        //System.Console.WriteLine(StringToHexString("中华人民共和国", 
        //System.Text.Encoding.UTF8));
        //或使用
        //BitConverter.ToString(Encoding.UTF8.GetBytes("中华人民共和国"))
        //返回结果为XX-XX-XX
        //然后再去掉”-“

        //数字和字节之间互转
        //int num = 12345;
        //byte[] bytes = BitConverter.GetBytes(num);//将int32转换为字节数组
        //num=BitConverter.ToInt32(bytes,0);//将字节数组内容再转成int32类型
        //字符串和字节之间互转
        //string str1= Encoding.Default.GetString(Byte[] bytes);
        //byte[] bytes = Encoding.Default.GetBytes(string str1);

        /// <summary>
        /// 对字节数组数据的各字节算术累加，得到2位16进制的字符串,得到NB水表通讯协议的校验码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetCheckCode(Byte[] data)
        {
            byte[] checkcode = new byte[1];
            for (int i = 0; i < data.Length; i++)
            {
                checkcode[0] += data[i];
            }
            return BitConverter.ToString(checkcode);
        }
        /// <summary>
        /// byte转换为BCD码
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte ConvertBCD(byte b)
        {
            //高四位  
            byte b1 = (byte)(b / 10);
            //低四位  
            byte b2 = (byte)(b % 10);
            return (byte)((b1 << 4) | b2);
        }

        /// <summary>  
        /// 将BCD一字节数据转换到byte 十进制数据  
        /// </summary>  
        /// <param name="b" />字节数  
        /// <returns>返回转换后的BCD码</returns>  
        public static byte ConvertBCDToInt(byte b)
        {
            //高四位  
            byte b1 = (byte)((b >> 4) & 0xF);
            //低四位  
            byte b2 = (byte)(b & 0xF);

            return (byte)(b1 * 10 + b2);
        }

        /// <summary>
        /// 将16进制字符串转为INT
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static int HexStringToInt(string hexString)
        {
            return Int32.Parse(hexString, System.Globalization.NumberStyles.HexNumber); 
        }

        /// <summary>
        /// 将16进制字符串转为浮点数
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static float HexStringToFloat(string hexString)
        {
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] floatVals = BitConverter.GetBytes(num);
            return BitConverter.ToSingle(floatVals, 0); 
        }

    }
}