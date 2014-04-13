//==============================================================
// ProgReporter
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ProgReporter.Helpers
{
    internal class CriptoService
    {
        private const string Password = "somepassword";
        private const int Iterations = 1024;
        private readonly byte[] salt = Encoding.ASCII.GetBytes("somesalt");

        internal string Encrypt(string plainText)
        {
            var keyBytes = new Rfc2898DeriveBytes(Password, salt, Iterations);
            var alg = new RijndaelManaged {Key = keyBytes.GetBytes(32), IV = keyBytes.GetBytes(16)};
            var encryptStream = new MemoryStream();
            var encrypt = new CryptoStream(encryptStream, alg.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            encrypt.Write(data, 0, data.Length);
            encrypt.FlushFinalBlock();
            encrypt.Close();
            return Convert.ToBase64String(encryptStream.ToArray());
        }

        internal string Decrypt(string cipherText)
        {
            var keyBytes = new Rfc2898DeriveBytes(Password, salt, Iterations);
            var alg = new RijndaelManaged {Key = keyBytes.GetBytes(32), IV = keyBytes.GetBytes(16)};
            var decryptStream = new MemoryStream();
            var decrypt = new CryptoStream(decryptStream, alg.CreateDecryptor(), CryptoStreamMode.Write);
            byte[] data = Convert.FromBase64String(cipherText);
            decrypt.Write(data, 0, data.Length);
            decrypt.Flush();
            decrypt.Close();
            return Encoding.UTF8.GetString(decryptStream.ToArray());
        }
    }
}