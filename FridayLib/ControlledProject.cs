using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{
    public class ControlledProject : INotifyPropertyChanged, ICloneable
    {
        //[NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private int id;
        private string name;
        private string workingDirectory;
        private PPOCategories category = PPOCategories.Other;
        private string releaseDirectory;
        private PPOTasks task = PPOTasks.SCADA_Addons;
        private bool allAppsAreUpToDate;
        private string docDirectory;
        private bool allAppsAreInReestr;
        private ObservableCollection<ControlledApp> apps = new ObservableCollection<ControlledApp>();
        private ObservableCollection<SourceTextFile> sourceTextFiles = new ObservableCollection<SourceTextFile>();


        /// <summary>
        /// Идентификатор проекта
        /// </summary>
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        
        /// <summary>
        /// Имя контролируемого приложения
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        
        /// <summary>
        /// Директория для релиза проекта
        /// </summary>
        public string ReleaseDirectory
        {
            get { return releaseDirectory; }
            set
            {
                releaseDirectory = value;
                OnPropertyChanged("ReleaseDirectory");
            }
        }

        /// <summary>
        /// Рабочая директория проекта
        /// </summary>
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                workingDirectory = value;
                OnPropertyChanged("WorkingDirectory");
            }
        }
                
        /// <summary>
        /// Директория с документацией на проект
        /// </summary>
        public string DocumentDirectory
        {
            get { return docDirectory; }
            set
            {
                docDirectory = value;
                OnPropertyChanged("DocumentDirectory");
            }
        }

        /// <summary>
        /// Категория ППО
        /// </summary>
        public PPOCategories Category
        {
            get { return category; }
            set
            {
                category = value;
                OnPropertyChanged("Category");
            }
        }

        /// <summary>
        /// Задача ППО (по классификации реестра)
        /// </summary>
        public PPOTasks Task
        {
            get { return task; }
            set
            {
                task = value;
                OnPropertyChanged("Task");
            }
        }

        /// <summary>
        /// Все приложения проекта обновлены
        /// </summary>
        public bool AllApрsAreUpToDate
        {
            get { return allAppsAreUpToDate; }
            set
            {
                allAppsAreUpToDate = value;
                OnPropertyChanged("AllApрsAreUpToDate");
            }
        }
        
        /// <summary>
        /// Все приложения проекта прошли в реестр ППО
        /// </summary>
        public bool AllAppsAreInReestr
        {
            get { return allAppsAreInReestr; }
            set
            {
                allAppsAreInReestr = value;
                OnPropertyChanged("AllAppsAreInReestr");
            }
        }

        /// <summary>
        /// Коллекция приложений данного проекта
        /// </summary>
        public ObservableCollection<ControlledApp> Apps
        {
            get { return apps; }
            set
            {
                apps = value;
                OnPropertyChanged("Apps");
            }
        }

        /// <summary>
        /// Коллекция для списка исходных текстов
        /// </summary>
        public ObservableCollection<SourceTextFile> SourceTextFiles
        {
            get { return sourceTextFiles; }
            set
            {
                sourceTextFiles = value;
                OnPropertyChanged("SourceTextFiles");
            }
        }


        public static ControlledProject GetById(int id, IEnumerable<ControlledProject> collection)
        {
            foreach(var prj in collection)
            {
                if (prj.Id == id)
                    return prj;
            }
            return new ControlledProject();
        }

        public void UpdateState()
        {
            AllAppsAreInReestr = true;
            AllApрsAreUpToDate = true;
            for (int i = 0; i < Apps.Count; i++)
            {
                if (!Apps[i].UpToDate)
                    AllApрsAreUpToDate = false;
                if (!Apps[i].IsInReestr)
                    AllAppsAreInReestr = false;
            }
        }

        /// <summary>
        /// Актуализировать все приложения данного проекта
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            try
            {
                AllAppsAreInReestr = true;
                AllApрsAreUpToDate = true;
                for(int i=0;i<Apps.Count;i++)
                {
                    await Apps[i].CopyToFolderAsync(Apps[i].SourceDirectory, Apps[i].ReleaseDirectory);
                    await Apps[i].UpdateMainFileInfoAsync();                    
                }
                UpdateState();
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка обновления проекта {0}: {1}", Name, ex.Message));
            }
        }

        public async Task PrepareDocumentation()
        {
            await System.Threading.Tasks.Task.Run( async () =>
            {
                SaveSourceTextsAsExcel();
                PrepareListing();
                foreach(var app in Apps)
                {
                    await app.PrepareDocumentation();
                }
            });
        }

        private void PrepareListing()
        {
            FridayLib.Text_Module.Listing.CreateListing(this);
        }

        public async Task<bool> CheckEquals()
        {
            try
            {
                foreach(var i in await DatabaseClass.CheckForEqual(this))
                {
                    if(!i.Equals(Id))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка проверки уникальности данных по проекту {0}: {1}", Name, ex.Message));
                return false;
            }
        }

        public void LoadSourceTexts()
        {
            SourceTextFiles = SourceTextCreation.ScanFolder(WorkingDirectory, "", System.IO.Path.Combine(DocumentDirectory, "SourceTexts.txt"));
        }
        public void SaveSourceTextsAsText()
        {
            SourceTextCreation.SaveAsTextFile(SourceTextFiles, System.IO.Path.Combine(DocumentDirectory, "SourceTexts.txt"));
        }

        public void SaveSourceTextsAsExcel()
        {
            SourceTextCreation.SaveAsExcel(SourceTextFiles, System.IO.Path.Combine(DocumentDirectory, "Ведомость исходных текстов.xlsx"));
        }

        public object Clone()
        {
            return new ControlledProject
            {
                AllAppsAreInReestr= this.AllAppsAreInReestr,
                AllApрsAreUpToDate = this.AllApрsAreUpToDate,
                Apps=this.Apps,
                Category=this.Category,
                DocumentDirectory=this.DocumentDirectory,
                Id=this.Id,
                Name=this.Name,
                ReleaseDirectory=this.ReleaseDirectory,
                SourceTextFiles = this.SourceTextFiles,
                Task=this.Task,
                WorkingDirectory=this.WorkingDirectory
            };
        }
    }
}
