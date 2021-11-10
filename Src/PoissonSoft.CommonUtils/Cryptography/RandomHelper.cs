using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PoissonSoft.CommonUtils.Cryptography
{
    /// <summary>
    /// Random methods
    /// </summary>
    public class RandomHelper
    {
        static readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

        /// <summary>
        /// Return random hex
        /// </summary>
        /// <param name="length">Number of bytes (chars = bytes * 2)</param>
        /// <returns></returns>
        public static string GetRandomHex(int length)
        {
            var bytes = new byte[length];
            random.GetBytes(bytes);
            return bytes.ToHex();
        }
    }
}
