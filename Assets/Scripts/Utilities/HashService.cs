using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Utilities
{
    public static class HashService
    {
        /// <summary>
        /// This function takes in a string and returns a GUID that is reproduce-able across 
        /// systems
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GenerateHash(string email, string password)
        {
            string input = email + password;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                Array.Resize(ref hashBytes, 16);
                return new Guid(hashBytes).ToString();
            }
        }

        public static string GenerateHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                Array.Resize(ref hashBytes, 16);
                return new Guid(hashBytes).ToString();
            }
        }

        public static string EncodeImage(string input)
        {
            return new NotImplementedException().Message;
        }

        public static byte[] DecodeImage(string input)
        {
            throw new NotImplementedException();
            //return new byte[5];
        }
    }
}

