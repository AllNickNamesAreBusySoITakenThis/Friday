using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{
    public class ControlledApp : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string name = "";
        private string sourceDirectory = "";
        private string description = "";
        private string releaseDirectory = "";
        private string documentDirectopry = "";
        private string mainFileName="";
        private string mainFileVersion = "";
        private string mainFileHash = "";
        private bool upToDate = false;
        private PPOReestrStatus status = PPOReestrStatus.NotTested;
        private string mainFileDate;
        private string compatibleOSs = "Windows 7, 10";
        private string compatibleScadas = "iFix 5.8, 5.9, 6.0";
        private string otherSoft = "отсутствует";
        private string compatibleSZI = "Kaspersky Endpoint Security 11";
        private string identificationType = "Не требуется";
        private string installer = "Нет";
        private string report = "Нет";
        private string buildingComponents = ".Net Framework 4.5.2 – реализация возможности использования C#";
        private string functionalComponents = ".Net Framework 4.5.2 – реализация возможности использования C#";
        private string dataStoringMechanism = "Текстовый файл в общедоступном каталоге";
        private string subd = "Нет";
        private string localData = "Настройки приложения";
        private string authorizationType = "Нет";
        private string platform = ".NetFramework v.4.5.2";
        private string userCategories = "нет";
        [NonSerialized]
        private ControlledProject parent;
        private bool isInReestr = false;
        private int id;
        private string mainFileReleaseVersion = "";
        private string mainFileReleaseHash = "";
        private string mainFileReleaseDate = "";
        private bool blocked;
        private string workingStatus;


        /// <summary>
        /// Идентификатор приложения
        /// </summary>
        [Category("Common")]
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }
        [Category("Common"), Description("Имя приложения"), DisplayName("Название")]
        /// <summary>
        /// Имя приложения
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
        /// Описание назначния приложения
        /// </summary>
        [Category("Common"), Description("Описание назначения приложения"), DisplayName("Описание")]
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        /// <summary>
        /// Директория-источник
        /// </summary>
        [Category("Dir"), Description("Директория, где расположена последняя собранная версияя"), DisplayName("Рабочая директория")]
        public string SourceDirectory
        {
            get { return sourceDirectory; }
            set
            {
                sourceDirectory = value;
                OnPropertyChanged("SourceDirectory");
            }
        }
        /// <summary>
        /// Директория для релиза приложения
        /// </summary>
        [Category("Dir"), Description("Директория с релизной версией приложения"), DisplayName("Релиз")]
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
        /// Имя основного исполняемого файла
        /// </summary>
        [Category("Dir"), Description("Имя основного исполняемого файла"), DisplayName("Исполняемый файл")]
        public string MainFileName
        {
            get { return mainFileName; }
            set
            {
                mainFileName = value;
                OnPropertyChanged("MainFileName");
            }
        }
        /// <summary>
        /// Версия основного исполняемого файла
        /// </summary>
        public string MainFileVersion
        {
            get { return mainFileVersion; }
            set
            {
                mainFileVersion = value;
                OnPropertyChanged("MainFileVersion");
            }
        }
        /// <summary>
        /// Контрольная сумма (SHA1) основного исполняемого файла
        /// </summary>
        public string MainFileHash
        {
            get { return mainFileHash; }
            set
            {
                mainFileHash = value;
                OnPropertyChanged("MainFileHash");
            }
        }
        /// <summary>
        /// Дата изменения основного исполняемого файла
        /// </summary>
        public string MainFileDate
        {
            get { return mainFileDate; }
            set
            {
                mainFileDate = value;
                OnPropertyChanged("MainFileDate");
            }
        }
        /// <summary>
        /// Приложение в релизе актуально
        /// </summary>
        public bool UpToDate
        {
            get { return upToDate; }
            set
            {
                upToDate = value;
                OnPropertyChanged("UpToDate");
            }
        }
        /// <summary>
        /// Статус проверки на прохождение в реестр ППО
        /// </summary>
        [Category("Formular"), Description("Статус приложения в реестре ППО"), DisplayName("Статус в реестре")]
        public PPOReestrStatus Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }
        /// <summary>
        /// Платформа
        /// </summary>
        [Category("Formular"), Description("Исполняемая среда каждого из звеньев приложения"), DisplayName("Платформа")]
        public string Platform
        {
            get { return platform; }
            set
            {
                platform = value;
                OnPropertyChanged("Platform");
            }
        }
        /// <summary>
        /// Совместимые ОС
        /// </summary>
        [Category("Formular"), Description("Соваместимые версии ОС"), DisplayName("Совместимые ОС")]
        public string CompatibleOSs
        {
            get { return compatibleOSs; }
            set
            {
                compatibleOSs = value;
                OnPropertyChanged("CompatibleOSs");
            }
        }
        /// <summary>
        /// Совместимые SCADA
        /// </summary>
        [Category("Formular"), Description("Совместимые SCADA с указанием версий"), DisplayName("Совместимые SCADA")]
        public string CompatibleScadas
        {
            get { return compatibleScadas; }
            set
            {
                compatibleScadas = value;
                OnPropertyChanged("CompatibleScadas");
            }
        }
        /// <summary>
        /// Совместимые СЗИ
        /// </summary>
        [Category("Formular"), Description("Совместимые СЗИ с указанием версий"), DisplayName("Совместимые СЗИ")]
        public string CompatibleSZI
        {
            get { return compatibleSZI; }
            set
            {
                compatibleSZI = value;
                OnPropertyChanged("CompatibleSZI");
            }
        }
        /// <summary>
        /// Другие необходимые ПО с указанием версий и назначения
        /// </summary>
        [Category("Formular"), Description("Другие необходимые типы ПО с указанием версий"), DisplayName("Прочее ПО")]
        public string OtherSoft
        {
            get { return otherSoft; }
            set
            {
                otherSoft = value;
                OnPropertyChanged("OtherSoft");
            }
        }
        /// <summary>
        /// Тип идентификации и аутентификации
        /// </summary>
        [Category("Formular"), Description("Тип идентификации и аутентификации"), DisplayName("Тип идентификации")]
        public string IdentificationType
        {
            get { return identificationType; }
            set
            {
                identificationType = value;
                OnPropertyChanged("IdentificationType");
            }
        }
        /// <summary>
        /// Тип авторизации
        /// </summary>
        [Category("Formular"), Description("Тип авторизации"), DisplayName("Авторизация")]
        public string AuthorizationType
        {
            get { return authorizationType; }
            set
            {
                authorizationType = value;
                OnPropertyChanged("AuthorizationType");
            }
        }
        /// <summary>
        /// Состав рзделяемых и локально хранимых данных
        /// </summary>
        [Category("Formular"), Description("Состав разделяемых и локально хранимых данных"), DisplayName("Локальные данные")]
        public string LocalData
        {
            get { return localData; }
            set
            {
                localData = value;
                OnPropertyChanged("LocalData");
            }
        }
        /// <summary>
        /// Используемая СУБД
        /// </summary>
        [Category("Formular"), Description("Используемые СУБД"), DisplayName("СУБД")]
        public string SUBD
        {
            get { return subd; }
            set
            {
                subd = value;
                OnPropertyChanged("SUBD");
            }
        }
        /// <summary>
        /// Механизмы и средства хранения данных
        /// </summary>
        [Category("Formular"), Description("Механизмы и средства хранения локальных данных"), DisplayName("Механизмы хранения данных")]
        public string DataStoringMechanism
        {
            get { return dataStoringMechanism; }
            set
            {
                dataStoringMechanism = value;
                OnPropertyChanged("DataStoringMechanism");
            }
        }
        /// <summary>
        /// Компоненты требуемые для функционирования
        /// </summary>
        [Category("Formular"), Description("Компоненты и платформы, требуемые для функционирования приложения"), DisplayName("Компоненты для функционирования")]
        public string FunctionalComponents
        {
            get { return functionalComponents; }
            set
            {
                functionalComponents = value;
                OnPropertyChanged("FunctionalComponents");
            }
        }
        /// <summary>
        /// Компоненты, требуемые для сборки
        /// </summary>
        [Category("Formular"), Description("Компоненты и платформы, требуемые для сборки приложения"), DisplayName("Компоненты для сборки")]
        public string BuildingComponents
        {
            get { return buildingComponents; }
            set
            {
                buildingComponents = value;
                OnPropertyChanged("BuildingComponents");
            }
        }
        /// <summary>
        /// Средства предоставления отчетности
        /// </summary>
        [Category("Formular"), Description("Средства предоставления отчетности"), DisplayName("Отчетность")]
        public string Report
        {
            get { return report; }
            set
            {
                report = value;
                OnPropertyChanged("Report");
            }
        }
        /// <summary>
        /// Тип установщика
        /// </summary>
        [Category("Formular"), Description("Тип установщика"), DisplayName("Установщик")]
        public string Installer
        {
            get { return installer; }
            set
            {
                installer = value;
                OnPropertyChanged("Installer");
            }
        }
        /// <summary>
        /// Проект, к которому приналдежит приложение
        /// </summary>        
        public ControlledProject Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                OnPropertyChanged("Parent");
            }
        }
        /// <summary>
        /// допущено в реестр
        /// </summary>
        public bool IsInReestr
        {
            get { return isInReestr; }
            set
            {
                isInReestr = value;
                OnPropertyChanged("IsInReestr");
            }
        }
        /// <summary>
        /// Директория с документацией по проекту
        /// </summary>
        [Category("Dir"), Description("Директория документации проекта"), DisplayName("Директория документации")]
        public string DocumentDirectory
        {
            get { return documentDirectopry; }
            set
            {
                documentDirectopry = value;
                OnPropertyChanged("DocumentDirectory");
            }
        }
        /// <summary>
        /// Версия основного исполняемого файла в релизе
        /// </summary>
        public string MainFileReleaseVersion
        {
            get { return mainFileReleaseVersion; }
            set
            {
                mainFileReleaseVersion = value;
                OnPropertyChanged("MainFileReleaseVersion");
            }
        }
        /// <summary>
        /// Контрольная сумма (SHA1) основного исполняемого файла в релизе
        /// </summary>
        public string MainFileReleaseHash
        {
            get { return mainFileReleaseHash; }
            set
            {
                mainFileReleaseHash = value;
                OnPropertyChanged("MainFileReleaseHash");
            }
        }
        /// <summary>
        /// Дата изменения основного исполняемого файла в релизе
        /// </summary>
        public string MainFileReleaseDate
        {
            get { return mainFileReleaseDate; }
            set
            {
                mainFileReleaseDate = value;
                OnPropertyChanged("MainFileReleaseDate");
            }
        }
        /// <summary>
        /// Предполагаемые категори пользователей
        /// </summary>
        [Category("Formular"), Description("Предполагаемые категории пользователей"), DisplayName("Категории пользователей")]
        public string UserCategories
        {
            get { return userCategories; }
            set
            {
                userCategories = value;
                OnPropertyChanged("UserCategories");
            }
        }
        /// <summary>
        /// Управление приложением ограничено
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
        /// Статус обработки приложения
        /// </summary>
        public string WorkingStatus
        {
            get { return workingStatus; }
            set
            {
                workingStatus = value;
                OnPropertyChanged("WorkingStatus");
            }
        }
        /// <summary>
        /// Путь к оснвоному исполняемому файлу приложения 
        /// </summary>
        public string MainFilePath
        {
            get { return Path.Combine(SourceDirectory, MainFileName); }
        }
        /// <summary>
        /// Путь к основному исполнямому файлу в релизе прилоения
        /// </summary>
        public string MainFileReleasePath
        {
            get { return Path.Combine(ReleaseDirectory, MainFileName); }
        }

        

        /// <summary>
        /// Обновить данные по основному исполняемому файлу в рабочей директории и в релизе
        /// </summary>
        /// <returns></returns>
        public async Task UpdateMainFileInfoAsync()
        {
            Blocked = true;
            WorkingStatus = "Проверка актуальности приложения в релизе";
            await Task.Run(() =>
            {
                MainFileDate = FileOperations.GetChangeDate(MainFilePath);
                MainFileHash =  FileOperations.GetCheckSumm(MainFilePath);
                MainFileVersion = FileOperations.GetVersion(MainFilePath);
                MainFileReleaseDate = FileOperations.GetChangeDate(MainFileReleasePath);
                MainFileReleaseHash = FileOperations.GetCheckSumm(MainFileReleasePath);
                MainFileReleaseVersion = FileOperations.GetVersion(MainFileReleasePath);
            });     
            //GoogleScriptsClass.UpdateSheetsData(new List<ControlledApp>() { this});
            UpToDate = (MainFileVersion.Equals(MainFileReleaseVersion) && MainFileHash.Equals(MainFileReleaseHash));
            WorkingStatus = "";
            Blocked = false;
        }

        /// <summary>
        /// Обновить данные по основному исполняемому файлу в рабочей директории и в релизе
        /// </summary>
        /// <returns></returns>
        public void UpdateMainFileInfo()
        {
            MainFileDate = FileOperations.GetChangeDate(MainFilePath);
            MainFileHash = FileOperations.GetCheckSumm(MainFilePath);
            MainFileVersion = FileOperations.GetVersion(MainFilePath);
            MainFileReleaseDate = FileOperations.GetChangeDate(MainFileReleasePath);
            MainFileReleaseHash = FileOperations.GetCheckSumm(MainFileReleasePath);
            MainFileReleaseVersion = FileOperations.GetVersion(MainFileReleasePath);
            UpToDate = (MainFileVersion.Equals(MainFileReleaseVersion) && MainFileHash.Equals(MainFileReleaseHash));
        }

        /// <summary>
        /// Скопировать все требуемые файлы из каталога в каталог
        /// </summary>
        /// <param name="source">Каталог-источник</param>
        /// <param name="dest">Католог-цель</param>
        /// <returns></returns>
        public async Task CopyToFolderAsync(string source, string dest, bool st=true)
        {
            Blocked = true;
            WorkingStatus = "Копирование файлов в релиз";
            try
            {
                foreach(var dir in Directory.GetDirectories(source))
                {
                    await CopyToFolderAsync(dir, Path.Combine(dest, new DirectoryInfo(dir).Name),false);
                }
                foreach(var file in Directory.GetFiles(source))
                {
                    if(Service.GetListFromString(ServiceLib.Configuration.Configuration.Get("AllowedExtentions").ToString()).Contains(new FileInfo(file).Extension))
                    {
                        if (!Directory.Exists(dest))
                            Directory.CreateDirectory(dest);
                        File.Copy(file, Path.Combine(dest, new FileInfo(file).Name), true);
                    }
                }
                IsInReestr = false;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка копирования в релиз: {0}", ex.Message));
            }
            if(st)
            {
                WorkingStatus = "";
                Blocked = false;
            }            
        }
        public void CopyToFolder(string source, string dest, bool st = true)
        {
            Blocked = true;
            WorkingStatus = "Копирование файлов в релиз";
            try
            {
                foreach (var dir in Directory.GetDirectories(source))
                {
                    CopyToFolder(dir, Path.Combine(dest, new DirectoryInfo(dir).Name),false);
                }
                foreach (var file in Directory.GetFiles(source))
                {
                    if (Service.GetListFromString(ServiceLib.Configuration.Configuration.Get("AllowedExtentions").ToString()).Contains(new FileInfo(file).Extension))
                    {
                        if(!Directory.Exists(dest))
                            Directory.CreateDirectory(dest);
                        File.Copy(file, Path.Combine(dest, new FileInfo(file).Name), true);
                    }
                }
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка копирования в релиз: {0}", ex.Message));
            }
            if (st)
            {
                WorkingStatus = "";
                Blocked = false;
            }
        }
        public async Task PrepareDocumentation()
        {
            Blocked = true;
            WorkingStatus = "Подготовка документации";
            await Task.Run(() =>
            {
                PrepareRequest();
                PrepareFormular();
            });
            WorkingStatus = "";
            Blocked = false;
        }
           
        private void PrepareFormular()
        {
            Word_Module.DocumentCreation.CreateFormular(this);
        }

        private void PrepareRequest()
        {
            Word_Module.DocumentCreation.CreateRequest(this);
        }

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
                Service.OnErrorInLibrary(string.Format("Ошибка проверки уникальности данных по приложению {0}: {1}", Name, ex.Message));
                return false;
            }
        }
        public static ControlledApp GetById(int id, ControlledProject prj)
        {
            foreach (var app in prj.Apps)
            {
                if (app.Id == id)
                    return app;
            }
            return new ControlledApp();
        }
        public object Clone()
        {
            return new ControlledApp
            {
                AuthorizationType = this.AuthorizationType,
                BuildingComponents = this.BuildingComponents,
                CompatibleOSs = this.CompatibleOSs,
                CompatibleScadas = this.CompatibleScadas,
                CompatibleSZI = this.CompatibleSZI,
                DataStoringMechanism = this.DataStoringMechanism,
                Description = this.Description,
                DocumentDirectory = this.DocumentDirectory,
                FunctionalComponents = this.FunctionalComponents,
                Id = this.Id,
                IdentificationType = this.IdentificationType,
                Installer = this.Installer,
                IsInReestr = this.IsInReestr,
                LocalData = this.LocalData,
                MainFileDate = this.MainFileDate,
                MainFileHash = this.MainFileHash,
                MainFileName = this.MainFileName,
                MainFileReleaseDate = this.MainFileReleaseDate,
                MainFileReleaseHash = this.MainFileReleaseHash,
                MainFileReleaseVersion = this.MainFileReleaseVersion,
                MainFileVersion = this.MainFileVersion,
                Name = this.Name,
                OtherSoft = this.OtherSoft,
                Parent = this.Parent,
                Platform=this.Platform,
                ReleaseDirectory = this.ReleaseDirectory,
                Report = this.Report,
                SourceDirectory=this.SourceDirectory,
                Status = this.Status,
                SUBD = this.SUBD,
                UpToDate = this.UpToDate,
                UserCategories=this.UserCategories                
            };
        }
        /// <summary>
        /// Обновить данные о приложении в БД
        /// </summary>
        public async void Update()
        {
            await DatabaseClass.UpdateApp(this);
        }
        public async void Remove()
        {
            
        }
    }
}
