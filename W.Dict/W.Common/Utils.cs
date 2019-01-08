using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace W.Common
{
    public class Utils
    {
        public static long GetCurrentTimeStamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        /// <summary>
        /// Sorted by Keys
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static string NewTXSign(Dictionary<string, object> keyValues, string appKey)
        {
            string[] sortedKeys = keyValues.Keys.OrderBy(x => x, StringComparer.Ordinal).ToArray();
            var joinedParas = string.Join("&", sortedKeys.Select(key => key + "=" + Uri.EscapeDataString(keyValues[key].ToString())));
            joinedParas += "&" + nameof(appKey) + "=" + appKey;
            return Md5String(joinedParas);
        }

        public static string JoinParameters(Dictionary<string, object> keyValues)
        {
            return string.Join("&", keyValues.Select(kv => kv.Key + "=" + kv.Value.ToString()));
        }

        static string Md5String(string content)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(content);
            using (var md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string NewSalt(int length = 8, bool numberOnly = true) => GetRandomString(length, numberOnly);

        public static string NewNonce(int length = 10, bool numberOnly = false) => GetRandomString(length, numberOnly);

        static Random m_rnd = new Random();
        static char GetRandomChar(bool numberOnly = false)
        {
            int[][] charsets = new int[][] {
                new int[] { 48, 58 }, // 0-9
                new int[] { 65, 91 }, // A-Z
                new int[] { 97, 123 } }; // a-z
            short startIndex = 3;
            if (numberOnly)
                startIndex = 1;
            int charsetIndex = m_rnd.Next(0, startIndex);
            return (char)m_rnd.Next(
                charsets[charsetIndex][0],
                charsets[charsetIndex][1]);
        }

        static string GetRandomString(int length, bool numberOnly = false)
        {
            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(GetRandomChar(numberOnly));
            }
            return sb.ToString();
        }
    }
}
