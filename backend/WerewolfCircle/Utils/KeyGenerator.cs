using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WerewolfCircle.Utils
{
    internal static class KeyGenerator
    {
        private static readonly char[] s_chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GetUniqueKey(int length)
        {
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(s_chars[RandomNumberGenerator.GetInt32(0, s_chars.Length)]);
            }

            return result.ToString();
        }
    }
}
