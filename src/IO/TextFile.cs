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
        public static void Spit(string filename, string text, bool append = false)
        {
            using(var f = new StreamWriter(filename, append))
            {   
                f.Write(text);
                f.Close();
            }
        }

        /// <summary>
        /// Reads in text from a file. Returns empty list if the file doesn't exist
        /// </summary>
        /// <remarks>Stolen from Clojure</remarks>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string Slurp(string filename)
        {
            if (!File.Exists(filename))
                return "";

            // TODO: Make this function chunky-lazy (read in like 1000 chars at a time, and use an iterator to return them)

            using (var f = new StreamReader(filename))
            {
                var res = f.ReadToEnd();
                f.Close();
                return res;
            }
        }
    }
}
