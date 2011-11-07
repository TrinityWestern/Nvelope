using System.Text;

namespace Nvelope
{
    public static class BinaryExtensions
    {
        /// <summary>
        /// Print an array of bytes to a hexidecimal string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] values)
        {
            char[] lookup = new char[] { '0', '1', '2', '3', '4', '5', '6',
                '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            int curByte = 0, curChar = 0, len = values.Length;
            char[] res = new char[len * 2];
            byte cur;
            while (curByte < len)
            {
                cur = values[curByte++];
                res[curChar++] = lookup[cur / 0x10]; // Get the character for the first nibble
                res[curChar++] = lookup[cur % 0x10]; // Get the character for the second nibble
            }

            return new string(res, 0, res.Length);

        }

        /// <summary>
        /// Interpret an array of bytes as a UTF32 string
        /// </summary>
        public static string ToUtf8String(this byte[] values)
        {
            return Encoding.UTF8.GetString(values);
        }
    }
}
