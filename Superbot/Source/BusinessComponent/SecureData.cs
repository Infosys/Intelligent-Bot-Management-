/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.CompilerServices;
//using DataAccess = Infosys.WEM.Infrastructure.SecurityCore.Data;
//using DataEntity = Infosys.WEM.Infrastructure.SecurityCore.Data.Entity;
//using BusinessEntity = Infosys.WEM.Infrastructure.SecurityCore.Business.Entity;
//using Translator = Infosys.WEM.Infrastructure.SecurityCore.Translators;
using System.Net;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class SecureData
    {
        const string pass = "IAP2GO_SEC!URE";
        const string keyText = "aWFw";
        const string sharedSecretPrivate = "aWFwU2lnbmF0dXJl"; //iapSignature
        static string secureK = DateTime.Now.Ticks.ToString();

        public static string Secure(string textToSecure, string passCode)
        {
            if (string.IsNullOrEmpty(textToSecure.Trim()))
                return "";
            int indexK = 0;
            string cipherTxt = "";

            if (passCode.Equals(pass))
            {
                cipherTxt = SecureData.EncryptData(textToSecure, secureK, out indexK);
                cipherTxt = cipherTxt + keyText + Base64Encode(indexK.ToString() + indexK.ToString().Length.ToString());
            }
            return cipherTxt;
        }

        public static string UnSecure(string secureText, string passCode)
        {

            if (string.IsNullOrEmpty(secureText.Trim()))
                return "";
            string plainText = "";

            if (passCode.Equals(pass))
            {
                string[] secureTextSplit = secureText.Trim().Split(new string[] { keyText }, StringSplitOptions.RemoveEmptyEntries);
                secureText = secureTextSplit[0] + Base64Decode(secureTextSplit[1]);

                int indexK = int.Parse(secureText[secureText.Length - 1].ToString());
                int cipherLength = secureText.Length - indexK - 1;
                int keyLength = indexK;

                if (cipherLength > 1)
                {
                    plainText = SecureData.DecryptData(secureText.Substring(0, cipherLength), Convert.ToInt16(secureText.Substring(cipherLength, keyLength)));
                }
                else
                    plainText = secureText;
            }
            return plainText;
        }

        public static string DecryptData(string cipherText, int keyIndex)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            byte[] sharedSecretBytes = new byte[cipherTextBytes.Length - keyIndex];
            byte[] actualCipherText = new byte[keyIndex];

            for (int i = keyIndex, j = 0; i < cipherTextBytes.Length; i++, j++)
                sharedSecretBytes[j] = cipherTextBytes[i];
            for (int i = 0; i < keyIndex; i++)
                actualCipherText[i] = cipherTextBytes[i];

            string sharedSecret = Encoding.Unicode.GetString(sharedSecretBytes);
            cipherText = Convert.ToBase64String(actualCipherText);

            // Declare the AesManaged object 
            // used to decrypt the data. 
            AesManaged aesAlg = null;


            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt 
                PasswordDeriveBytes key = new PasswordDeriveBytes(sharedSecret,
                    null, "SHA256", 1000);

                // Create a AesManaged object 
                // with the specified key and IV. 
                aesAlg = new AesManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform. 
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for decryption.                 
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string. 
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the AesManaged object. 
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        public static string EncryptData(string plainText, string sharedSecret, out int keyIndex)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return 
            AesManaged aesAlg = null;              // AesManaged object used to encrypt the data. 

            try
            {
                // generate the key from the shared secret and the salt 
                PasswordDeriveBytes key = new PasswordDeriveBytes(sharedSecret
                    , null, "SHA256", 1000);

                // Create a AesManaged object 
                // with the specified key and IV. 
                aesAlg = new AesManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

                // Create a decrytor to perform the stream transform. 
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }

                    byte[] cryptedText = msEncrypt.ToArray();
                    byte[] sharedSecretBytes = Encoding.Unicode.GetBytes(sharedSecret);
                    byte[] cipherText = new byte[cryptedText.Length + sharedSecretBytes.Length];
                    keyIndex = cryptedText.Length;

                    for (int i = 0; i < cryptedText.Length; i++)
                        cipherText[i] = cryptedText[i];
                    for (int j = cryptedText.Length, k = 0; j < (cryptedText.Length + sharedSecretBytes.Length);
                        j++, k++)
                        cipherText[j] = sharedSecretBytes[k];

                    outStr = Convert.ToBase64String(cipherText);
                }
            }
            finally
            {
                // Clear the AesManaged object. 
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream. 
            return outStr;
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

    }
}
