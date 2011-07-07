using System;
using System.IO;

namespace Nvelope.Exceptions
{
    [Serializable]
    public class FileAlreadyExistsException : IOException
    {
        public String FileName { get; set; }

        public FileAlreadyExistsException(String fileName, String message) : base(message)
        {
            FileName = fileName;
        }
    }
}
