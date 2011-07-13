using System;
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
                 try{
                     throw new FileAlreadyExistsException(FileName, "File already exists");
                 }
                 catch(Exception ex)
                 {
                     Assert.AreEqual(typeof(FileAlreadyExistsException), ex.GetType());
                     Assert.AreEqual("File already exists", ex.Message);
                 }
             }
             


         }



    }
}
