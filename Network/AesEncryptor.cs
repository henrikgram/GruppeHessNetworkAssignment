using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GruppeHessNetworkAssignment.Network
{
    public class AesEncryptor
    {
        private static AesEncryptor instance;
        public byte[] Key { get; private set; }

        public static AesEncryptor Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new AesEncryptor();
                }
                return instance;
            }
        }

        public AesEncryptor()
        {
            //used to generate a new key for symmetric encryption
            using (Aes aesGenerator = Aes.Create())
            {
                aesGenerator.GenerateKey();

                Key = aesGenerator.Key;
            }
        }

        public byte[] EncryptString(string plainText, byte[] Key)
        {

            byte[] encrypted;
            byte[] iv;


            // Create an Aes object
            using (Aes aesEncrypt = Aes.Create())
            {


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


                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
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

        public string DecryptStringFromBytes(byte[] cipherText, byte[] Key/*, byte[] IV*/)
        {

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            byte[] encryptedMessage; //= new byte[cipherText.Length-16];

            byte[] iv;// = new byte[16];

            //splits the byte array into the IV and encrypted message
            iv = cipherText.Take(16).ToArray();
            encryptedMessage = cipherText.Skip(16).ToArray();


            // Create an Aes object
            using (Aes aesDecrypt = Aes.Create())
            {
                //aesDecrypt.Key = Key;
                //aesDecrypt.IV = IV;

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
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }



    }

}

