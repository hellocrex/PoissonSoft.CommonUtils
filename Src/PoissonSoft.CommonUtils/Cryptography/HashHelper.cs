using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PoissonSoft.CommonUtils.Cryptography
{
    /// <summary>
    /// Hash helper methods
    /// </summary>
    public static class HashingHelper
    {
        /// <summary>
        /// Get sha256 from string 
        /// </summary>
        /// <param name="str">Source string</param>
        /// <param name="takeBytes">Take first N bytes</param>
        /// <returns></returns>
        public static string GetSha256(string str, int takeBytes = int.MaxValue)
        {
            using var crypt = new SHA256Managed();
            var hashBytes = crypt.ComputeHash(Encoding.UTF8.GetBytes(str)).Take(takeBytes);
            return hashBytes.ToHex();
        }

        /// <summary>
        /// Get sha512 from string 
        /// </summary>
        /// <param name="str">Source string</param>
        /// <param name="takeBytes">Take first N bytes</param>
        /// <returns></returns>
        public static string GetSha512(string str, int takeBytes = int.MaxValue)
        {
            using var crypt = new SHA512Managed();
            var hashBytes = crypt.ComputeHash(Encoding.UTF8.GetBytes(str)).Take(takeBytes);
            return hashBytes.ToHex();
        }
    }
}
