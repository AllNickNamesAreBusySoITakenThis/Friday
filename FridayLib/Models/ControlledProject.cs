using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace FridayLib
{
    public class ControlledProject : INotifyPropertyChanged, ICloneable
    {
        
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        } 
        #endregion

        #region Private variables
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
        private bool blocked=false;
        private string workStatus="";
        #endregion

        #region Public properties

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
        /// <summary>
        /// Возможность управления проектом ограничена
        /// </summary>
        public bool Blocked
        {
            get { return blocked; }
            set
            {
                blocked = value;
                OnPropertyChanged("Blocked");
            }
        }
        /// <summary>
        /// Статус обработки проекта
        /// </summary>
        public string WorkStatus
        {
            get { return workStatus; }
            set
            {
                workStatus = value;
                OnPropertyChanged("WorkStatus");
            }
        }
        #endregion

        #region Public methods

        #region Static
        /// <summary>
        /// Получить проект по его Id в коллекции
        /// </summary>
        /// <param name="id"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static ControlledProject GetById(int id, IEnumerable<ControlledProject> collection)
        {
            foreach (var prj in collection)
            {
                if (prj.Id == id)
                    return prj;
            }
            return new ControlledProject();
        }
        #endregion

        #region Not static
        /// <summary>
        /// Сохранить проект в БД
        /// </summary>
        public void SaveProject()
        {
            try
            {
                using (var pc = new ProjectContext())
                {
                    var curPrjs = pc.Projects.Where(i => i.Id == Id);
                    var curPrj = curPrjs.First();
                    if(curPrj!=null)
                    {
                        curPrj = this;
                    }
                    pc.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка сохранения проекта {0}: {1}", Name, ex.Message));
            }
        }
        /// <summary>
        /// Загрузить из БД данные по проекту
        /// </summary>
        public void LoadProject()
        {
            try
            {
                using (var pc = new ProjectContext())
                {
                    var curPrjs = pc.Projects.Where(i => i.Id == Id);
                    var curPrj = curPrjs.First();
                    if (curPrj != null)
                    {
                        this.Apps = curPrj.Apps;
                        Name = curPrj.Name;
                        ReleaseDirectory = curPrj.ReleaseDirectory;
                        WorkingDirectory = curPrj.WorkingDirectory;
                        DocumentDirectory = curPrj.DocumentDirectory;
                        Category = curPrj.Category;
                        Task = curPrj.Task;
                        SourceTextFiles = curPrj.SourceTextFiles;
                    }
                }
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка сохранения проекта {0}: {1}", Name, ex.Message));
            }
        }
        /// <summary>
        /// Добавить приложение
        /// </summary>
        public void AddApp()
        {
            ControlledApp app = new ControlledApp() { Name = "Новое приложение", Parent = this, ParentId=this.Id };
            Apps.Add(app);
            using (var pc = new ProjectContext())
            {
                pc.Applications.Add(app);
                pc.SaveChanges();
            }
        }
        /// <summary>
        /// Удалить приложение
        /// </summary>
        /// <param name="app"></param>
        public void RemoveApp(ControlledApp app)
        {            
            Apps.Remove(app);
            using (var pc = new ProjectContext())
            {
                pc.Applications.Remove(app);
                pc.SaveChanges();
            }
        }

        /// <summary>
        /// Актуализировать релизы всех приложений данного проекта
        /// </summary>
        /// <returns></returns>
        public void ActualizeRelease()
        {
            try
            {
                Blocked = true;
                WorkStatus = "Актуализация релизов проекта";
                AllAppsAreInReestr = true;
                AllApрsAreUpToDate = true;
                for (int i = 0; i < Apps.Count; i++)
                {
                    Apps[i].ActualizeRelease();                    
                }
                UpdateState();
                WorkStatus = "";
                Blocked = false;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка обновления проекта {0}: {1}", Name, ex.Message));
            }
        }
        /// <summary>
        /// Актуализировать реестр всех приложений данного проекта
        /// </summary>
        /// <returns></returns>
        public void ActualizeReestr()
        {
            try
            {
                Blocked = true;
                WorkStatus = "Актуализация реестра проекта";
                AllAppsAreInReestr = true;
                AllApрsAreUpToDate = true;
                for (int i = 0; i < Apps.Count; i++)
                {
                    Apps[i].ActualizeReestr();
                }
                UpdateState();
                WorkStatus = "";
                Blocked = false;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка обновления проекта {0}: {1}", Name, ex.Message));
            }
        }
        /// <summary>
        /// Подготовить набор для реестра
        /// </summary>
        /// <param name="folderName">Директория для сбора данных</param>
        public async void PrepareReestrPackeje(string folderName)
        {
            try
            {
                //Актуализируем документацию
                await PrepareDocumentation();
                //Готовим структуру для формата разработки
                Directory.CreateDirectory(Path.Combine(folderName, "Формат разработки"));
                CopyDataForReestr(WorkingDirectory, Path.Combine(folderName, "Формат разработки"));
                //Копируем документацию
                DirectoryInfo di = new DirectoryInfo(DocumentDirectory);
                Directory.CreateDirectory(Path.Combine(folderName, "Документация"));
                foreach (var file in di.GetFiles())
                {
                    if (file.Extension != ".csv")
                    {
                        File.Copy(file.FullName, Path.Combine(folderName, "Документация", file.Name));
                    }
                }
                //Дальнейшие действия зависят от того, сколько приложений в проекте
                if (Apps.Count == 1)
                {
                    Directory.CreateDirectory(Path.Combine(folderName, "Скомпилированная версия"));
                    await Apps[0].CopyToFolderAsync(Apps[0].SourceDirectory, Path.Combine(folderName, "Скомпилированная версия"));
                    DirectoryInfo adi = new DirectoryInfo(Apps[0].DocumentDirectory);
                    foreach (var file in adi.GetFiles())
                    {
                        if (file.Extension != ".csv")
                        {
                            File.Copy(file.FullName, Path.Combine(folderName, "Документация", file.Name));
                        }
                    }
                }
                else
                {
                    foreach (var app in Apps)
                    {
                        Directory.CreateDirectory(Path.Combine(folderName, app.Name, "Скомпилированная версия"));
                        await app.CopyToFolderAsync(app.SourceDirectory, Path.Combine(folderName, "Скомпилированная версия"));
                        Directory.CreateDirectory(Path.Combine(folderName, app.Name, "Документация"));
                        DirectoryInfo adi = new DirectoryInfo(app.DocumentDirectory);
                        foreach (var file in adi.GetFiles())
                        {
                            if (file.Extension != ".csv")
                            {
                                File.Copy(file.FullName, Path.Combine(folderName, app.Name, "Документация", file.Name));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка формирования пакета для реестра {0}: {1}", Name, ex.Message));
            }
        }
        /// <summary>
        /// Подготовить листинг проекта
        /// </summary>
        private void PrepareListing()
        {
            WorkStatus = "Подготовка листинга";
            Text_Module.Listing.CreateListing(this);
            WorkStatus = "";
        }
        /// <summary>
        /// Подготовить документацию по проекту
        /// </summary>
        /// <returns></returns>
        public async Task PrepareDocumentation()
        {
            Blocked = true;
            WorkStatus = "Подготовка документации по проекту";
            await System.Threading.Tasks.Task.Run(() =>
            {
                LoadSourceTexts();
                SaveSourceTextsAsExcel();
                PrepareListing();
                foreach (var app in Apps)
                {
                    app.PrepareFormular();
                    app.PrepareRequest();
                }
            });
            WorkStatus = "";
            Blocked = false;
        }
        /// <summary>
        /// Загрузить данные по списку исходных текстов
        /// </summary>
        public void LoadSourceTexts()
        {
            WorkStatus = "Загрузка данных по исходным текстам";
            SourceTextFiles = SourceTextCreation.ScanFolder(WorkingDirectory, "", System.IO.Path.Combine(DocumentDirectory, "SourceTexts.txt"),
                System.IO.Path.Combine(DocumentDirectory, "Ведомость исходных текстов.xlsx"));
        }
        /// <summary>
        /// Сохранить исходные тексты в текстовом файле
        /// </summary>
        public void SaveSourceTextsAsText()
        {
            SourceTextCreation.SaveAsTextFile(SourceTextFiles, System.IO.Path.Combine(DocumentDirectory, "SourceTexts.txt"));
        }
        /// <summary>
        /// Свормировать ведомость исходных текстов
        /// </summary>
        public void SaveSourceTextsAsExcel()
        {
            WorkStatus = "Подготовка ведомости исходных текстов";
            SourceTextCreation.SaveAsExcel(SourceTextFiles, System.IO.Path.Combine(DocumentDirectory, "Ведомость исходных текстов.xlsx"));
        }

        /// <summary>
        /// Обновить состояние проекта
        /// </summary>
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
        /// Получить приложения для проекта
        /// </summary>
        public async Task GetApps()
        {
            Apps = new ObservableCollection<ControlledApp>();
            Apps = await DatabaseClass.GetAppsForProject(this);
            UpdateState();
        }
        /// <summary>
        /// Обновить информацию о проекте
        /// </summary>
        public async void UpdateInfo()
        {
            if (await CheckEquals())
            {
                await DatabaseClass.UpdateProject(this);
                foreach (var app in Apps)
                {
                    app.Update();
                } 
            }
        }
        
       
        /// <summary>
        /// Проверить новый/обноленный проект на совпадение с существующими
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckEquals()
        {
            try
            {
                foreach (var i in await DatabaseClass.CheckForEqual(this))
                {
                    if (!i.Equals(Id))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка проверки уникальности данных по проекту {0}: {1}", Name, ex.Message));
                return false;
            }
        }                
        /// <summary>
        /// Клонировать объет проекта
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new ControlledProject
            {
                AllAppsAreInReestr = this.AllAppsAreInReestr,
                AllApрsAreUpToDate = this.AllApрsAreUpToDate,
                Apps = this.Apps,
                Category = this.Category,
                DocumentDirectory = this.DocumentDirectory,
                Id = this.Id,
                Name = this.Name,
                ReleaseDirectory = this.ReleaseDirectory,
                SourceTextFiles = this.SourceTextFiles,
                Task = this.Task,
                WorkingDirectory = this.WorkingDirectory
            };
        }
        
       
       

        #endregion

        #endregion

        #region Private methods
        int GetNewAppId()
        {
            int id = 0;
            foreach(var app in Apps)
            {
                if (app.Id == id)
                    id = app.Id + 1;
            }
            return id;
        }
        /// <summary>
        /// Копировать проект в формате разработки
        /// </summary>
        /// <param name="sourceFolderName">Директория-источник</param>
        /// <param name="destFolderName">Директория-цель</param>
        void CopyDataForReestr(string sourceFolderName, string destFolderName)
        {
            DirectoryInfo di = new DirectoryInfo(sourceFolderName);
            foreach (var folder in di.GetDirectories())
            {
                if (folder.Name.ToUpper() != "OBJ" && folder.Name.ToUpper() != "BIN")
                {
                    CopyDataForReestr(folder.FullName, Path.Combine(destFolderName, folder.Name));
                }
            }
            foreach (var file in di.GetFiles())
            {
                File.Copy(file.FullName, Path.Combine(destFolderName, file.Name));
            }
        }
        #endregion
    }
}
