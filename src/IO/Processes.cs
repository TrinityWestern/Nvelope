using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Nvelope.IO
{
    public static class Processes
    {

        /// <summary>
        /// Execute some external program and return the results
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <param name="command"></param>
        /// <param name="arguments"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string Execute(string command, string arguments, int timeout = 30000, string workingDirectory = "")
        {
            if (workingDirectory.IsNullOrEmpty())
                workingDirectory = Directory.GetCurrentDirectory();

            Process p = new Process();
            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.WorkingDirectory = workingDirectory;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.ErrorDialog = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            p.Start();

            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            var error = p.StandardError.ReadToEnd();
            p.WaitForExit();

            if (p.ExitCode != 0)
                throw new ExternalProcessException("External processed exited with code: " + p.ExitCode + 
                    " and message: " + error + "\n" + output);

            return output;


        }
    }
}
