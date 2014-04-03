using System.IO;
using System.Security.Cryptography;

namespace Nvelope
{
    public static class FileExtensions
    {
        public static string MD5(this FileInfo file)
        {
            using (var stream = file.Open(FileMode.Open, FileAccess.Read)) {
                return stream.MD5();
            }
        }
        public static string MD5(this Stream stream)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bytes = md5.ComputeHash(stream);
            return bytes.ToHexString();
        }

        /// <summary>
        /// This method returns fileextension based on the filename
        /// </summary>
        /// <param name="name">file name eg hello.html</param>
        /// <returns>file extension eg .html</returns>
        public static string GetExtension(string name)
        {
            if (name.IndexOf(".") > -1)
            {
                return name.Substring(name.IndexOf(".")).ToLower();
            }
            return "";
        }
    }
}
