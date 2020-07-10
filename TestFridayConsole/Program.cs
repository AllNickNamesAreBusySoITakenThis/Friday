using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using FridayLib;

namespace TestFridayConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            FridayLib.Service.Init();
            //ControlledApp app = new ControlledApp();
            //app.MainFileName = "AllMessageTransfer.exe";
            //app.ReleaseDirectory = @"\\192.168.77.228\ппо\РЕЛИЗ\C#\Трансфер сообщений на ДМЗ\Приложение";
            //app.SourceDirectory = @"\\192.0.0.227\проекты\C#\Repositories\AllMessageTransfer\AllMessageTransfer\AllMessageTransfer\bin\Release";
            //app.Name = "Трансфер сообщений на сервер ДМЗ";
            //app.Description = "Служба для передачи сообщений с АРМ на сервер ДМЗ";
            ////SourceTextFile textFile = new SourceTextFile(new FileInfo(@"\\192.0.0.227\Public\Козлов\Object.mdb"), "");
            //ControlledProject prj = new ControlledProject()
            //{
            //    ReleaseDirectory = @"\\192.168.77.228\ппо\РЕЛИЗ\C#\Трансфер сообщений на ДМЗ",
            //    WorkingDirectory = @"\\192.0.0.227\проекты\C#\Repositories\AllMessageTransfer",
            //    DocumentDirectory = @"\\192.168.77.228\ппо\РЕЛИЗ\C#\Трансфер сообщений на ДМЗ\Документация",
            //    Name = "Трансфер сообщений на ДМЗ",
            //    Id=1
            //};
            //prj.Apps.Add(app);
            //prj.LoadSourceTexts();
            //prj.SaveProject();
            //GoogleScriptsClass.AddDataToSheet(app);
            //GoogleScriptsClass.UpdateSheetsData(app);
            //var a = MainModel.ReadFromDatabase();
            //Console.Read();
            ///////////////////////////////////////////////////////////////////////////////
            //var projects = DatabaseClass.GetProjects();
            //foreach(var prj in projects)
            //{
            //    prj.Apps = DatabaseClass.GetAppsForProject(prj);
            //    prj.LoadSourceTexts();
            //    prj.SaveProject();
            //}
            ////////////////////////////////////////////////////////////////////////////////////
        }
    }
}
