using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nvelope.IO
{
    public static class TextFile
    {        
        /// <summary>
        /// Writes a bunch of lines to a text file (overwrites the file if it exists)
        /// </summary>
        /// <remarks>Stolen from Clojure</remarks>
        /// <param name="filename"></param>
        /// <param name="texts"></param>
        public static void Spit(string fileName, string text, bool append = false)
        {
            using (var f = new StreamWriter(fileName, append))
            {   
                f.Write(text);
                f.Close();
            }
        }

        /// <summary>
        /// Reads in text from a file. Returns empty list if the file doesn't exist
        /// </summary>
        /// <remarks>Stolen from Clojure</remarks>
        public static string Slurp(string fileName)
        {
            if (!File.Exists(fileName))
                return "";
            using (var f = new StreamReader(fileName))
            {
                var res = f.ReadToEnd();
                f.Close();
                return res;
            }
        }
    }
}
