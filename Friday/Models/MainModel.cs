using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FridayLib;

namespace Friday
{  
    public class MainModel
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;
        static void OnStaticPropertyChanged(string name)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }


        static CFile workingFile;
        public static CFile WorkingFile
        {
            get { return workingFile; }
            set
            {
                workingFile = value;
                OnStaticPropertyChanged("WorkingFile");
            }
        }

        internal async static void StartApp()
        {
            await MainClass.FillFiles();
            await MainClass.CheckAllFiles();
        }

        internal static void UpdateAll()
        {
            MainClass.Refresh();
        }

        internal static void UpdateSingle(CFile file)
        {
            MainClass.Refresh(file);
        }

        internal static void AddFile()
        {
            WorkingFile = new CFile();
            AddFileWindow wind = new AddFileWindow();
            wind.Owner = SingleInstanceManager.app.MainWindow;
            if(wind.ShowDialog() == true)
            {
                MainClass.AddFile(WorkingFile.Name,WorkingFile.SourcePath, WorkingFile.ReleasePath, WorkingFile.ProjectName);
            }
            WorkingFile = null;
            GC.Collect();
        }

        internal static void EditFile(object file)
        {
            WorkingFile = (file as CFile);
            AddFileWindow wind = new AddFileWindow();
            wind.Owner = SingleInstanceManager.app.MainWindow;
            if (wind.ShowDialog() == true)
            {
                MainClass.UpdateFile(WorkingFile);
            }
            WorkingFile = null;
            GC.Collect();
        }

        internal static void CheckAll()
        {
            foreach(var file in MainClass.Files)
            {
                file.GetFileInfo();
            }
        }

        internal static void CheckSingle(CFile file)
        {
            file.GetFileInfo();
        }
    }
}
