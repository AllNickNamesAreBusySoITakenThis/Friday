using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FridayLib;

namespace FridayConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MainClass.ErrorInLibrary += MainClass_ErrorInLibrary;
            //ControlledProject project = new ControlledProject()
            //{
            //    Id = 0,
            //    Category = PPOCategories.Sevice,
            //    DocumentDirectory = @"\\192.168.77.228\ппо\РЕЛИЗ\C#\Создание списка исходных текстов",
            //    ReleaseDirectory = @"\\192.168.77.228\ппо\РЕЛИЗ\C#\Создание списка исходных текстов",
            //    WorkingDirectory = @"C:\Users\Иван Козлов\Desktop\SourceTextListing",
            //    Name = "Создание списка исходных текстов",
            //    Task = PPOTasks.OS_Addons,
            //    AllAppsAreInReestr = false,
            //    AllApрsAreUpToDate = false,
            //    Apps = new System.Collections.ObjectModel.ObservableCollection<ControlledApp>()
            //};

            //ControlledApp app = new ControlledApp()
            //{
            //    Id = 0,
            //    Name = "SourceTextListing",
            //    AuthorizationType = "Нет",
            //    Description = "Создание списка исходных текстов приложений",
            //    MainFileName = "SourceTextListing.exe",
            //    SourceDirectory = @"C:\Users\Иван Козлов\Desktop\SourceTextListing\SourceTextListing\bin\Release",
            //    ReleaseDirectory = @"\\192.168.77.228\ппо\РЕЛИЗ\C#\Создание списка исходных текстов\Приложение",
            //    DocumentDirectory = @"\\192.168.77.228\ппо\РЕЛИЗ\C#\Создание списка исходных текстов\Документация",
            //    BuildingComponents = ".Net Framework 4.5.2 – реализация возможности использования C#",
            //    CompatibleOSs = "Windows 7, 10",
            //    CompatibleScadas = "iFix 5.8, 5.9, 6.0",
            //    CompatibleSZI = "Kaspersky Endpoint Security 11",
            //    DataStoringMechanism = "Текстовый файл в общедоступном каталоге",
            //    FunctionalComponents = ".Net Framework 4.5.2 – реализация возможности использования C#",
            //    IdentificationType = "Нет",
            //    Installer = "Нет",
            //    IsInReestr = false,
            //    LocalData = "Нет",
            //    OtherSoft = "Нет",
            //    Platform = ".Net Framework 4.5.2",
            //    SUBD = "отсутствует",
            //    Report = "отсутствует",
            //    Status = PPOReestrStatus.NotTested
            //};
            //app.UpdateMainFileInfo();
            //project.Apps.Add(app);
            //app.Parent = project;

            //DatabaseClass.AddProject(project);
            //DatabaseClass.AddApp(app);

            var projects = DatabaseClass.GetProjects().Result;
            for (int i = 0; i < projects.Count; i++)
            {
                projects[i] = DatabaseClass.GetAppsForProject(projects[i]).Result;
            }
            foreach (var prj in projects)
            {
                foreach (var app in prj.Apps)
                {
                    //app.UpdateMainFileInfo();
                    //app.CopyToFolderAsync(app.SourceDirectory, app.ReleaseDirectory);
                    //Console.WriteLine("Begin preparing formular");
                    //FridayLib.Word_Module.DocumentCreation.CreateFormular(app);
                    //Console.WriteLine("{0} - {1}", prj.Name, app.Name);
                    //app.UpdateMainFileInfo();
                    //DatabaseClass.UpdateApp(app);
                    GoogleScriptsClass.AddDataToSheet(app);
                    
                }
                //GoogleScriptsClass.UpdateSheetsData(prj.Apps);
                //Console.WriteLine("Запуск формирования листинга для проекта {0}",prj.Name);
                //FridayLib.Text_Module.Listing.CreateListing(prj);
                //Console.WriteLine("Окончено формирования листинга для проекта {0}", prj.Name);
                //Console.WriteLine("Запуск формирования ведомости исходных текстов для проекта {0}", prj.Name);
                //SourceTextCreation.CreateSourceTextList(prj);
                //Console.WriteLine("Окончено формирования ведомости исходных текстов для проекта {0}", prj.Name);
            }

            //
            Console.Read();
        }

        private static void MainClass_ErrorInLibrary(string message)
        {
            Console.WriteLine(message);
        }
    }
}
