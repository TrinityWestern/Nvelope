using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Nvelope
{
    public static class FileExtensions
    {
        public static string MD5(this FileInfo file)
        {
            using (var stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                var md5 = new MD5CryptoServiceProvider();                
                var bytes = md5.ComputeHash(stream);
                return bytes.ToHexString();
            }
        }
    }
}
