using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment.Network
{
    /// <summary>
    /// Use this static class to encode passwords.
    /// </summary>
    public class MD5Manager
    {
        public static bool PasswordsAreEqual { get; set; }


        /// <summary>
        /// Encode and hash the input string.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] EncodePassword(string password)
        {
            //Create a byte array from recieved data.
            byte[] tmpData = Encoding.ASCII.GetBytes(password);

            //Compute hash based on recieved data.
            byte[] hashedPassword = new MD5CryptoServiceProvider().ComputeHash(tmpData);

            return hashedPassword;
        }


        /// <summary>
        /// Method can be used to show byte arrays as strings.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] input)
        {
            StringBuilder output = new StringBuilder(input.Length);

            for (int i = 0; i < input.Length; i++)
            {
                output.Append(input[i].ToString("X2"));
            }

            return output.ToString();
        }
    }
}
