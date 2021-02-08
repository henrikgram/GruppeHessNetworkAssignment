using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment.Network
{
    /// <summary>
    /// Use this static class to hash passwords.
    /// </summary>
    public class MD5Manager
    {
        /// <summary>
        /// Hash the input string (password) as a byte array.
        /// </summary>
        /// <param name="password">The input string. What to hash.</param>
        /// <returns>Returns the hashed password.</returns>
        public static byte[] EncodePassword(string password)
        {
            //Create a byte array from recieved data.
            byte[] tmpData = Encoding.ASCII.GetBytes(password);

            //Compute hash based on recieved data.
            byte[] hashedPassword = new MD5CryptoServiceProvider().ComputeHash(tmpData);

            //Return the hashed value.
            return hashedPassword;
        }

        /// <summary>
        /// Method can be used to convert byte arrays to strings.
        /// </summary>
        /// <param name="input">The byte array you want to convert to string.</param>
        /// <returns>Returns the byte array as a string.</returns>
        public static string ByteArrayToString(byte[] input)
        {
            //For creating a string.
            StringBuilder output = new StringBuilder(input.Length);

            //Turn the array into a string.
            for (int i = 0; i < input.Length; i++)
            {
                output.Append(input[i].ToString("X2"));
            }

            //Return the new string.
            return output.ToString();
        }
    }
}
