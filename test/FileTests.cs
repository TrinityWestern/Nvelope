using System.IO;
using NUnit.Framework;
using Nvelope.Exceptions;

namespace Nvelope.Tests
{
     [TestFixture]
    class FileTests
    {
         [Test]
         public void FileExistsException()
         {
             string s = "The quick brown fox jumped over the lazy dog.";
             string FileName = Path.GetTempPath()+"test.txt";

             using (FileStream stream = new FileStream(FileName, FileMode.Create))
             {
                 using (BinaryWriter writer = new BinaryWriter(stream))
                 {
                     writer.Write(s);
                     writer.Close();
                 }
             }

             if (File.Exists(FileName))
             {
                 try {
                     throw new FileAlreadyExistsException(FileName, "File (" + FileName + ") already exists");
                 }
                 catch (FileAlreadyExistsException ex)
                 {
                     Assert.IsTrue(ex.Message.StartsWith("File ("));
                     Assert.IsTrue(ex.Message.EndsWith("test.txt) already exists"));
                 }
             }
             


         }



    }
}
