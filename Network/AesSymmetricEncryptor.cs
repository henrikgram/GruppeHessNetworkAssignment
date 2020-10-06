using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment.Network
{
    public class AesSymmetricEncryptor
    {
        private static AesSymmetricEncryptor instance;

        public byte[] key = { 207,251,215,218,58,91,196,42,174,212,191,30,155,179,227,104};
     
        public static AesSymmetricEncryptor Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new AesSymmetricEncryptor();
                }
                return instance;
            }
        }


        /// <summary>
        /// Returns a byte array with the enrypted input
        /// </summary>
        /// <param name="input">message to be encrypted</param>
        /// <param name="Key">Key for encrypting message</param>
        /// <returns></returns>
        public byte[] EncryptString(string input, byte[] Key)
        {

            byte[] encrypted;
            byte[] iv;

            // Create an Aes object
            using (Aes aesEncrypt = Aes.Create())
            {
                //aesEncrypt.Key = Key;
                //aesEncrypt.IV = IV;

                //generates a new specific IV for this message
                aesEncrypt.GenerateIV();

                iv = aesEncrypt.IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesEncrypt.CreateEncryptor(Key, iv);

                // Create the streams used for encryption.
                //memory stream for saving it to a string
                using (MemoryStream msEncrypt = new MemoryStream())
                {

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {

                        //stream writer for writing the encryption to a string
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(input);
                        }

                        //make a new empty array with the size of the encrypted message + the length of the IV
                        encrypted = new byte[msEncrypt.ToArray().Length + iv.Length];

                        //saves the encrypted message to an array
                        byte[] tmpMsEncrypt = msEncrypt.ToArray();

                        //Combines the two arrays into one
                        Array.Copy(iv, encrypted, iv.Length);
                        Array.Copy(tmpMsEncrypt, 0, encrypted, iv.Length, tmpMsEncrypt.Length);
                     
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }


        /// <summary>
        /// Returns a decrypted string from an encrypted byte array
        /// </summary>
        /// <param name="input">Encrypted message</param>
        /// <param name="Key">Key for decrypting</param>
        /// <returns></returns>
        public string DecryptStringFromBytes(byte[] input, byte[] Key)
        {

            string decryptedMessage = null;
            byte[] encryptedMessage; 

            byte[] iv;

            //splits the byte array into the IV and encrypted message
            iv = input.Take(16).ToArray();
            encryptedMessage = input.Skip(16).ToArray();

            // Create an Aes object
            using (Aes aesDecrypt = Aes.Create())
            {
                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesDecrypt.CreateDecryptor(Key, iv);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encryptedMessage))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            decryptedMessage = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedMessage;
        }
    }

}

