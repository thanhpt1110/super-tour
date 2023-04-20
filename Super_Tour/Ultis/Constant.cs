using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Super_Tour.Ultis
{
    internal class Constant
    {
        public static string convertPassToMD5(string s)
        {
            StringBuilder sb = new StringBuilder();

            // Initialize a MD5 hash object
            using (MD5 md5 = MD5.Create())
            {
                // Compute the hash of the given string
                byte[] hashValue = md5.ComputeHash(Encoding.UTF8.GetBytes(s));

                // Convert the byte array to string format
                foreach (byte b in hashValue)
                {
                    sb.Append($"{b:X2}");
                }
            }

            return sb.ToString();
        }
    }
}
