using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FridayLib;
using FridayLib.Models;

namespace TestFridayConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //ControlledApp app = new ControlledApp();
            //app.MainFileName = "Friful.exe";
            //app.ReleaseDirectory = @"\\192.0.0.227\Public\Козлов\Friday\Frifull\bin\Release";
            //app.SourceDirectory = @"\\192.0.0.227\Public\Козлов\Friday\Frifull\bin\Debug";
            //ControlledProject prj = new ControlledProject()
            //{
            //    ReleaseDirectory = @"\\192.0.0.227\Public\Козлов\Friday\Frifull\bin\Release",
            //    WorkingDirectory= @"\\192.0.0.227\Public\Козлов\Friday",
            //    DocumentDirectory= @"\\192.0.0.227\Public\Козлов\Friday",
            //    Name ="Пятница"                 
            //};
            //prj.Apps.Add(app);
            //MainModel.WriteToDatabse(new List<ControlledProject> { prj });
            var a = MainModel.ReadFromDatabase();
            Console.Read();
        }
    }
}
