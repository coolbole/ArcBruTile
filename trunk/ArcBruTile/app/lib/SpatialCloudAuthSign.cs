using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace BrutileArcGIS
{
    public class SpatialCloudAuthSign
    {
        public static string GetMD5Hash(string z, string x, string y, string type, string login, string password)
        {
            string input = z + "/" + x + "/" + y + "." + type + login + password;

            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }        

    }
}
