using System.IO;
using System.Text;

namespace Common.Utils.Extensions
{
    /// <summary>
    /// Extensions to easily transform string to stream.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns stream from string value.
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>Converts string to stream</returns>
        public static Stream ToStream(this string value)
        {
            return ToStream(value, Encoding.UTF8);
        }

        /// <summary>
        /// Returns stream from string value.
        /// </summary>
        /// <param name="value">string value.</param>
        /// <param name="encoding">string encoding.</param>
        /// <returns>Converts string to stream</returns>
        public static Stream ToStream(this string value, Encoding encoding)
        {
            return new MemoryStream(encoding.GetBytes(value ?? string.Empty));
        }
    }
}
