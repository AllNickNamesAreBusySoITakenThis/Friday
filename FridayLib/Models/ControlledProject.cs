using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using NLog.LayoutRenderers;
using System.Security.Permissions;
using ServiceLib.Configuration;
using System.Net.Http.Headers;

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
        private int id=0;
        private string name = "Новый проект";
        private string workingDirectory="";        
        private string releaseDirectory = "";        
        private bool allAppsAreUpToDate;
        private string docDirectory = "";
        private bool allAppsAreInReestr;
        private ObservableCollection<ControlledApp> apps = new ObservableCollection<ControlledApp>();
        private ObservableCollection<SourceTextFile> sourceTextFiles = new ObservableCollection<SourceTextFile>();
        private bool blocked=false;
        private string workStatus="";
        private ControlledApp currentApp;
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
        [Category("Общее"), Description("Имя проекта"), DisplayName("Название")]
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
        [Category("Общее"), Description("Директория релиза проекта"), DisplayName("Директория релиза")]
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
        [Category("Общее"), Description("Рабочая директория проекта"), DisplayName("Рабочая директория")]
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
        [Category("Общее"), Description("Директория с документацией на проект"), DisplayName("Директория документации")]
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
        /// Все приложения проекта обновлены
        /// </summary>
        [Category("Статистика"), Description("Все приложения в проекте актуальны"), DisplayName("Проект актуализирован")]
        public bool AllApрsAreUpToDate
        {
            get { return allAppsAreUpToDate; }
            private set
            {
                allAppsAreUpToDate = value;
                OnPropertyChanged("AllApрsAreUpToDate");
            }
        }
        /// <summary>
        /// Все приложения проекта прошли в реестр ППО
        /// </summary>
        [Category("Статистика"), Description("Все приложения проекта находятся в Реестре ППО"), DisplayName("Внесен в Реестр ППО")]
        public bool AllAppsAreInReestr
        {
            get { return allAppsAreInReestr; }
            private set
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
        /// <summary>
        /// Текущее приложение
        /// </summary>
        public ControlledApp CurrentApp
        {
            get { return currentApp; }
            set
            {
                currentApp = value;
                OnPropertyChanged("CurrentApp");
            }
        }
        
        #endregion

        #region Public methods

        #region Static
        public static void CreateProject(ICollection<ControlledProject> collection)
        {
            int newId = 0;
            foreach(var prj in collection)
            {
                if (prj.Id > newId)
                    newId = prj.Id;
            }
            newId++;
            collection.Add(new ControlledProject
            {
                Id = newId,
                Name = "Новый проект",
            });
        }
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
        public BsonDocument ToBsonDocument()
        {
            var doc = new BsonDocument
            {
                {"Id",Id },
                {"Name",Name },
                {"WorkingDirectory",WorkingDirectory },
                {"DocumentDirectory",DocumentDirectory },
                {"ReleaseDirectory",ReleaseDirectory },
                {"Applications",ControlledApp.GetAppArray(Apps) },
                {"SourceFiles",SourceTextFile.GetSourceTextsArray(SourceTextFiles) }
            };
            return doc;
        }
        /// <summary>
        /// Получить проект из документа Bson (MongoDB)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static ControlledProject FromBsonDocument(BsonDocument source)
        {
            try
            {
                var result = new ControlledProject
                {
                    Id = source["Id"].ToInt32(),
                    Name = source["Name"].ToString(),
                    WorkingDirectory = source["WorkingDirectory"].ToString(),
                    DocumentDirectory = source["DocumentDirectory"].ToString(),
                    ReleaseDirectory = source["ReleaseDirectory"].ToString(),
                    Apps = ControlledApp.GetAppsFromBsonArray(source["Applications"].AsBsonArray),
                    SourceTextFiles = SourceTextFile.GetFilesFromBsonArray(source["SourceFiles"].AsBsonArray)
                };
                foreach(var app in result.Apps)
                {
                    app.Parent = result;
                    app.ParentId = result.Id;
                }
                foreach(var st in result.SourceTextFiles)
                {
                    st.Parent = result;
                    st.ParentId = result.Id;
                }
                return result;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка получения данных для проекта : {0}", ex.Message));
                return null;
            }
        }

        public static ObservableCollection<ControlledProject> GetProjects()
        {
            try
            {
                ObservableCollection<ControlledProject> result = new ObservableCollection<ControlledProject>();
                MongoClient client = new MongoClient(Configuration.Get("MongoServer").ToString());
                IMongoDatabase database = client.GetDatabase(Configuration.Get("MongoDatabase").ToString());
                var collection = database.GetCollection<BsonDocument>(Configuration.Get("MongoCollection").ToString());
                var filter = new BsonDocument();
                var data = collection.Find(filter).ToList();
                foreach (var doc in data)
                {
                    result.Add(FromBsonDocument(doc));
                }
                return result;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка получения проектов: {0}", ex.Message));
                return new ObservableCollection<ControlledProject>();
            }
        }
        /// <summary>
        /// Сохранить проект в БД
        /// </summary>
        public async Task SaveProjectAsync()
        {
            try
            {
                MongoClient client = new MongoClient(Configuration.Get("MongoServer").ToString());
                IMongoDatabase database = client.GetDatabase(Configuration.Get("MongoDatabase").ToString());
                var collection = database.GetCollection<BsonDocument>(Configuration.Get("MongoCollection").ToString());
                var filter = Builders<BsonDocument>.Filter.Eq("Id", Id);
                await collection.ReplaceOneAsync(filter, ToBsonDocument(), new ReplaceOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка сохранения проекта {0}: {1}", Name, ex.Message));
            }
        }
        /// <summary>
        /// Сохранить проект в БД
        /// </summary>
        public void SaveProject()
        {
            try
            {
                MongoClient client = new MongoClient(Configuration.Get("MongoServer").ToString());
                IMongoDatabase database = client.GetDatabase(Configuration.Get("MongoDatabase").ToString());
                var collection = database.GetCollection<BsonDocument>(Configuration.Get("MongoCollection").ToString());
                var filter = Builders<BsonDocument>.Filter.Eq("Id", Id);
                collection.ReplaceOne(filter, ToBsonDocument(), new ReplaceOptions { IsUpsert = true });
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка сохранения проекта {0}: {1}", Name, ex.Message));
            }
        }
        /// <summary>
        /// Сохранить в проект в БД (для массового сохранения)
        /// </summary>
        /// <param name="mongoCollection"></param>
        public async static Task SaveProjects(ObservableCollection<ControlledProject> projects)
        {
            try
            {
                MongoClient client = new MongoClient(Configuration.Get("MongoServer").ToString());
                IMongoDatabase database = client.GetDatabase(Configuration.Get("MongoDatabase").ToString());
                var collection = database.GetCollection<BsonDocument>(Configuration.Get("MongoCollection").ToString());
                foreach (var prj in projects)
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("Id", prj.Id);
                    await collection.ReplaceOneAsync(filter, prj.ToBsonDocument(), new ReplaceOptions { IsUpsert = true }); 
                }
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка сохранения проектов: {0}",ex.Message));
            }
        }
        /// <summary>
        /// Добавить приложение
        /// </summary>
        public void AddApp()
        {
            ControlledApp app = ControlledApp.CreateApp(this);
            Apps.Add(app);
        }
        /// <summary>
        /// Удалить приложение
        /// </summary>
        /// <param name="app"></param>
        public void RemoveApp(ControlledApp app)
        {            
            Apps.Remove(app);
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
        public async void UpdateState()
        {
            AllAppsAreInReestr = true;
            AllApрsAreUpToDate = true;
            for (int i = 0; i < Apps.Count; i++)
            {
                await Apps[i].UpdateFileInfoAsync();
                if (!Apps[i].UpToDate)
                    AllApрsAreUpToDate = false;
                if (!Apps[i].IsInReestr)
                    AllAppsAreInReestr = false;
            }
        }              
        /// <summary>
        /// Проверить уникальность проекта в БД асинхронно
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckEqualsAsync(string name)
        {
            try
            {
                MongoClient client = new MongoClient("mongodb://localhost:27017");
                IMongoDatabase database = client.GetDatabase("ProjectsDatabase");
                var collection = database.GetCollection<BsonDocument>("ProjectsData");
                var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
                var data = await collection.Find(filter).ToListAsync();
                return data.Count == 0;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка проверки уникальности данных по проекту {0}: {1}", name, ex.Message));
                return false;
            }
        }
        /// <summary>
        /// Проверить уникальность проекта в БД
        /// </summary>
        /// <returns></returns>
        public static bool CheckEquals(string name)
        {
            try
            {
                MongoClient client = new MongoClient("mongodb://localhost:27017");
                IMongoDatabase database = client.GetDatabase("ProjectsDatabase");
                var collection = database.GetCollection<BsonDocument>("ProjectsData");
                var filter = Builders<BsonDocument>.Filter.Eq("Name", name);
                var data = collection.Find(filter).ToList();
                return data.Count == 0;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка проверки уникальности данных по проекту {0}: {1}", name, ex.Message));
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
                DocumentDirectory = this.DocumentDirectory,
                Id = this.Id,
                Name = this.Name,
                ReleaseDirectory = this.ReleaseDirectory,
                SourceTextFiles = this.SourceTextFiles,
                WorkingDirectory = this.WorkingDirectory
            };
        }        
        #endregion

        #endregion

        #region Private methods
        public int GetNewAppId()
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
            if(!Directory.Exists(destFolderName))
            {
                Directory.CreateDirectory(destFolderName);
            }
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
