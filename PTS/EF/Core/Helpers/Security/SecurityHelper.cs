using System;
using System.Security.Cryptography;
using System.Text;

namespace Core.Helpers.Security
{
    /// <summary>
    /// Security Helper
    /// </summary>
    public static class SecurityHelper
    {

        /// <summary>
        /// Returns Hashed password given a plain password
        /// </summary>
        /// <param name="password">plain text password</param>
        /// <param name="salt">password</param>
        /// <returns></returns>
        public static string HashPassword(string password, ref string salt)
        {
            if (string.IsNullOrEmpty(salt))
            {
                var saltByte = new byte[16];
                var randomData = RandomNumberGenerator.Create();
                randomData.GetBytes(saltByte);
                salt = Convert.ToBase64String(saltByte);
            }
            var passwordByte = Encoding.Unicode.GetBytes(salt + password);
            var hashedBytes = SHA512.Create().ComputeHash(passwordByte);
            var hashedPassword = Convert.ToBase64String(hashedBytes);
            return hashedPassword;
        }

        /// <summary>
        /// Computes Hash 
        /// </summary>
        /// <param name="messageKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ComputeHash(string messageKey, string message)
        {
            var key = Encoding.UTF8.GetBytes(messageKey.ToUpper());
            string hashString;

            using (var hmac = new HMACSHA512(key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                hashString = Convert.ToBase64String(hash);
            }

            return hashString;
        }

    } // class
} // namespace