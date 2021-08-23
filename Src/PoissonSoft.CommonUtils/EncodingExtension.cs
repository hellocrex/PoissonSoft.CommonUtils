using System;
using System.Collections.Generic;
using System.Text;

namespace PoissonSoft.CommonUtils
{
    /// <summary>
    /// Encoding extensions
    /// </summary>
    public static class EncodingExtension
    {
        /// <summary>
        /// Convert bytes to hex
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHex(this IEnumerable<byte> bytes)
        {
            string hex = string.Empty;
            foreach (byte theByte in bytes)
            {
                hex += theByte.ToString("x2");
            }
            return hex;
        }
    }
}
