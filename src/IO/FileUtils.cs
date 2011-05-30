using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nvelope.IO
{
    public static class FileUtils
    {
        /// <summary>
        /// Makes sure that the folder exists
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static void EnsureFolder(string path)
        {
            var dir = Folder.Directory(path);
            if (!Directory.Exists(dir))
            {
                EnsureFolder(Folder.Parent(dir));
                Directory.CreateDirectory(dir);
            }
        }
    }
}
