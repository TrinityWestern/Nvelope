using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope.Exceptions
{
    public class FileAlreadyExistsException : System.IO.IOException
    {
        public String FileName { get; set; }

        public FileAlreadyExistsException(String fileName, String message) : base(message)
        {
            FileName = fileName;
        }
    }
}
