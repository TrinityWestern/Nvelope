using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nvelope.IO
{
    public static class Folder
    {
        /// <summary>
        /// Get the part of the path representing the folder
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Directory(string path)
        {
            // If there's no slash in the path, then it's either a filename or a foldername
            if (!path.Contains("\\") && !path.Contains("/"))
            {
                // If it's in the pattern c.c, then it's a filename
                if (Regex.IsMatch(path, "[^\\.]+\\.[^\\.]+"))
                    return string.Empty;
                else
                    return path;
            }

            var beforeLastSlash = Regex.Replace(path, "([\\\\/])[^\\\\/]+\\.[^\\\\/]+$", "$1");
            return beforeLastSlash;
        }

        /// <summary>
        /// Get the part of the path representing the filename
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string File(string path)
        {
            return Regex.Replace(path, Directory(path).Replace("\\", "\\\\") + "(.*)$", "$1");
        }

        /// <summary>
        /// Finds the first seperator used in the path - if there is none, return alternate
        /// </summary>
        /// <param name="path"></param>
        /// <param name="alternate"></param>
        /// <returns></returns>
        public static string GetSeperator(string path, string alternate)
        {
            var match = Regex.Match(path, "([\\\\/])");
            if (match.Success)
                return match.Groups[1].Value;
            else
                return alternate;
        }

        /// <summary>
        /// Enforce the use of a given seperator in the path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string UseSeperator(string path, string seperator)
        {
            var alt = seperator == "/" ? "\\" : "/";
            return path.Replace(alt, seperator);
        }

        /// <summary>
        /// Finds the first seperator used in the paths, if there is none, return alternate
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string GetSeperator(IEnumerable<string> paths, string alternate)
        {
            return paths.Select(p => GetSeperator(p, "")).Where(s => !s.IsNullOrEmpty()).FirstOr(alternate);
        }
        
        /// <summary>
        /// Creates a path out of the cleaned-up pieces supplied
        /// </summary>
        /// <param name="pathParts"></param>
        /// <returns></returns>
        public static string ComposePath(params string[] pathParts)
        {
            return ComposePath(pathParts as IEnumerable<string>);
        }

        /// <summary>
        /// Creates a path out of the cleaned-up pieces supplied
        /// </summary>
        /// <param name="pathParts"></param>
        /// <returns></returns>
        public static string ComposePath(IEnumerable<string> pathParts)
        {
            if (!pathParts.Any())
                return "";

            var sep = GetSeperator(pathParts, "\\");
            // Normalize the seperators
            pathParts = pathParts.Select(s => UseSeperator(s, sep));

            var parts = pathParts.Where(s => !s.IsNullOrEmpty());
            return parts.First().ChopEnd(sep)
                .And(parts.Rest().Select(s => s.ChopStart(sep).ChopEnd(sep)))
                .Join(sep);
        }

        /// <summary>
        /// Get sequence of folder names in the path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<string> FolderParts(string path)
        {
            return Directory(path).Tokenize("^[\\\\/]?([^\\\\/]+)[\\\\/]?");
        }

        /// <summary>
        /// Get the file extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Extension(string path)
        {
            return Regex.Replace(File(path), "^.*\\.([^\\.]+)$", "$1");
        }

        /// <summary>
        /// Gets the filename without the extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FileBaseName(string path)
        {   
            return Regex.Replace(File(path), "\\.[^\\.]+$", "");
        }

        /// <summary>
        /// Convert a relative path into one relative to the starting directory
        /// </summary>
        /// <param name="startDir"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string ResolvePath(string startDir, string relativePath)
        {
            // If the "relative path" starts with a drive letter, it's an absoute path
            if (Regex.IsMatch(relativePath, "^[A-Za-z]\\:"))
                return relativePath;

            var filePart = Folder.File(relativePath);

            var relPath = FolderParts(relativePath);

            var seperator = GetSeperator(new string[] { startDir, relativePath }, "\\");

            // Work with the directory backwards, since we'll be messing around with
            // then end of it, which is more efficient to do on the front of lists
            var res = FolderParts(startDir).Reverse();

            while (relPath.Count() > 0)
            {
                if (relPath.First() == "..")
                    res = res.Rest();
                else if (relPath.IsSameAs(res.Reverse())) // remember, res is backwards
                    break; // exit the loop, since all the rest is the same
                else if (relPath.First() != ".")
                    res = relPath.First().And(res);

                relPath = relPath.Rest();
            }

            return Folder.ComposePath(res.Reverse().Join(seperator), filePart);
        }

        /// <summary>
        /// Is this directory top - level (no parents)
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool IsRoot(string dir)
        {
            return FolderParts(dir).Count() == 1;
        }

        /// <summary>
        ///  Get the parent directory
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string Parent(string dir)
        {
            var seperator = GetSeperator(dir, "\\");
            var parts = FolderParts(dir);
            if (parts.Count() < 2)
                return "";

            return parts.Chop().Join(seperator);
        }
    }
}
