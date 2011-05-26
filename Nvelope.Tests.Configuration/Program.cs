using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nvelope.Configuration;

namespace Nvelope.Tests.Configuration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Config.Setting("settingA"));
            Console.WriteLine(Config.ConnectionString("connStringA"));

            Console.ReadKey();

        }
    }
}
