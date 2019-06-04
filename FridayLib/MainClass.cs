using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FridayLib
{

    public class MainClass
    {
        public static event PropertyChangedEventHandler StaticPropertyChanged;
        static void OnStaticPropertyChanged(string name)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }


        static ObservableCollection<CFile> files;
        public static ObservableCollection<CFile> Files
        {
            get { return files; }
            set
            {
                files = value;
                OnStaticPropertyChanged("Files");
            }
        }

        public async static Task FillFiles()
        {
            await Task.Factory.StartNew(() => Files = DatabaseClass.GetFileData());            
        }

        public async static Task CheckAllFiles()
        {
            await Task.Factory.StartNew(() =>
            {
                foreach(var file in Files)
                {
                    file.GetFileInfo();
                }
            });
        }

        public static void UpdateFiles()
        {
            foreach(var file in Files)
            {
                DatabaseClass.UpdateCFile(file);
            }
            GoogleScriptsClass.UpdateSheetsData(Files);
        }

        public static void UpdateFile(CFile file)
        {
            DatabaseClass.UpdateCFile(file);
            //GoogleScriptsClass.UpdateSheetsData(new CFile[] { file });
        }

        public static void AddFile(string name, string sourcePath, string releasePath, string projName)
        {
            CFile temp = new CFile()
            {
                Name =name,
                ProjectName = projName,
                SourcePath = sourcePath,
                ReleasePath = releasePath,
                ID = Files.Count
            };
            temp.GetFileInfo();
            DatabaseClass.AddCFile(temp);
            GoogleScriptsClass.AddDataToSheet(temp);
            Files.Add(temp);
        }

        public static void Refresh()
        {
            foreach(var file in Files)
            {
                file.RefreshProject();
            }
            UpdateFiles();
        }

        public static void Refresh(CFile file)
        {
            file.RefreshProject();
            UpdateFile(file);
        }
    }
}
