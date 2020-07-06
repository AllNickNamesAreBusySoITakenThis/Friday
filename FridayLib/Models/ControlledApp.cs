using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
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
        private int parentId;
        private string name = "";
        private string sourceDirectory = "";
        private string description = "";
        private string releaseDirectory = "";
        private string documentDirectopry = "";
        private string mainFileName="";
        //private string mainFileVersion = "";
        //private string mainFileHash = "";
        private bool upToDate = false;
        private PPOReestrStatus status = PPOReestrStatus.NotTested;
        //private string mainFileDate;
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
        private string userCategories = "Нет";
        [NonSerialized]
        private ControlledProject parent;
        private bool isInReestr = false;
        private int id;
        //private string mainFileReleaseVersion = "";
        //private string mainFileReleaseHash = "";
        //private string mainFileReleaseDate = "";
        private bool blocked;
        private string workingStatus="";
        private string reestrDirectory = "";
        private ControlledFile currentFile=new ControlledFile();
        private ControlledFile reestrFile = new ControlledFile();
        private ControlledFile releaseFile = new ControlledFile();
        private string subdext = "Нет";
        private string ide = "Visual Studio 2017";
        private string propagation = "Копирование";
        private string licenseType = "Нет";
        private bool selected = false;
        #region Properties

        //---------------------------------------------------------------------------------------------------------Общие данные
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
        //---------------------------------------------------------------------------------------------------------Расположение файлов
        /// <summary>
        /// Директория-источник
        /// </summary>
        [Category("Dir"), Description("Директория, где расположена последняя собранная версия"), DisplayName("Рабочая директория")]
        public string SourceDirectory
        {
            get { return sourceDirectory; }
            set
            {
                sourceDirectory = value;
                CurrentFile = new ControlledFile(Path.Combine(SourceDirectory, MainFileName));
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
                ReleaseFile = new ControlledFile(Path.Combine(ReleaseDirectory, MainFileName));
                OnPropertyChanged("ReleaseDirectory");
            }
        }
        /// <summary>
        /// Директория с документацией по проекту
        /// </summary>
        [Category("Dir"), Description("Директория документации приложения"), DisplayName("Директория документации")]
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
        /// Директория реестра
        /// </summary>
        [Category("Dir"), Description("Директория с версией, прошедшей в реестр"), DisplayName("Директория реестра")]
        public string ReestrDirectory
        {
            get { return reestrDirectory; }
            set
            {
                reestrDirectory = value;
                ReestrFile = new ControlledFile(Path.Combine(ReestrDirectory, MainFileName));
                OnPropertyChanged("ReestrDirectory");
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
                CurrentFile = new ControlledFile(Path.Combine(SourceDirectory, MainFileName));
                ReestrFile = new ControlledFile(Path.Combine(ReestrDirectory, MainFileName));
                ReleaseFile = new ControlledFile(Path.Combine(ReleaseDirectory, MainFileName));
                OnPropertyChanged("MainFileName");
            }
        }
        /// <summary>
        /// файл в истончике
        /// </summary>
        [Category("Dir"), Description("Информация о рабочем файле"), DisplayName("Рабочий файл"), TypeConverter(typeof(ExpandableObjectConverter))]
        [NotMapped]
        public ControlledFile CurrentFile
        {
            get { return currentFile; }
            private set
            {
                currentFile = value;
                OnPropertyChanged("CurrentFile");
            }
        }
        /// <summary>
        /// Исполняемый файл в релизе
        /// </summary>
        [Category("Dir"), Description("Информация о файле в релизе"), DisplayName("Релизный файл")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [NotMapped]
        public ControlledFile ReleaseFile
        {
            get { return releaseFile; }
            private set
            {
                releaseFile = value;
                OnPropertyChanged("ReleaseFile");
            }
        }
        /// <summary>
        /// Исполняемый файл в реестре
        /// </summary>
        [Category("Dir"), Description("Информация о файле в реестре"), DisplayName("Файл в реестре")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [NotMapped]
        public ControlledFile ReestrFile
        {
            get { return reestrFile; }
            private set
            {
                reestrFile = value;
                OnPropertyChanged("ReestrFile");
            }
        }
        //---------------------------------------------------------------------------------------------------------Формуляр        
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
        [Category("Formular"), Description("Совместимые версии ОС"), DisplayName("Совместимые ОС")]
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
        //--------------------------------------------------------------------------------------------------------------------
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
        //--------------------------------------------------------------------------------------------------------------------
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
        /// Используемые расширения СУБД
        /// </summary>
        [Category("Formular"), Description("Используемые расширения СУБД"), DisplayName("Расширения СУБД")]
        public string SUBDExt
        {
            get { return subdext; }
            set
            {
                subdext = value;
                OnPropertyChanged("SUBDExt");
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
        //--------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Используемые среды проектирования 
        /// </summary>
        [Category("Formular"), Description("Используемые среды проектирования "), DisplayName("Используемые среды проектирования ")]
        public string IDE
        {
            get { return ide; }
            set
            {
                ide = value;
                OnPropertyChanged("IDE");
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
        //--------------------------------------------------------------------------------------------------------------------        
        /// <summary>
        /// Средства распространения
        /// </summary>
        [Category("Formular"), Description("Средства распространения"), DisplayName("Средства распространения")]
        public string Propagation
        {
            get { return propagation; }
            set
            {
                propagation = value;
                OnPropertyChanged("Propagation");
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
        /// Тип лицензии
        /// </summary>
        [Category("Formular"), Description("Тип лицензии"), DisplayName("Лицензия")]
        public string LicenseType
        {
            get { return licenseType; }
            set
            {
                licenseType = value;
                OnPropertyChanged("LicenseType");
            }
        }
        //---------------------------------------------------------------------------------------------------------Вспомогательные данные
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
        public int ParentId
        {
            get { return parentId; }
            set
            {
                parentId = value;
                OnPropertyChanged("ParentId");
            }
        }
        /// <summary>
        /// Проект выбран
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            }
        }
        //---------------------------------------------------------------------------------------------------------Флаги
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
        //---------------------------------------------------------------------------------------------------------
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
        #endregion


        //Основные функции для приложений
        //1. Актуализация релиза
        //2. Отправка в папку реестра
        //3. Подготовка заявки
        //4. Подготовка описания принятых решений
        //5. Контроль изменений от версии к версии - TODO
        //6. Запись всех данных о приложении в общую ведомость реестра - TODO
        //7. Предоставление информации о себе для общего отчета - TODO
        //8. Запись данных о приложении в БД
        //9. Удаление данных о приложении из БД
        //10. Изменение данных о приложении в БД

        #region Methods
        //-------------------------------------------------------1--------------------------------------------------------------
        /// <summary>
        /// Актуализировать релиз
        /// </summary>
        public async void ActualizeRelease()
        {
            await CopyToFolderAsync(SourceDirectory, ReleaseDirectory);
            await UpdateFileInfoAsync();
        }
        //-------------------------------------------------------2--------------------------------------------------------------
        /// <summary>
        /// Актуализировать реестр
        /// </summary>
        public async void ActualizeReestr()
        {
            await CopyToFolderAsync(ReleaseDirectory, ReestrDirectory);
            await UpdateFileInfoAsync();
        }
        //-------------------------------------------------------3--------------------------------------------------------------
        /// <summary>
        /// Подготовить заявку
        /// </summary>
        public async void PrepareRequest()
        {
            await Task.Run(() => Word_Module.DocumentCreation.CreateRequest(this));
        }
        //-------------------------------------------------------4--------------------------------------------------------------
        /// <summary>
        /// Подготовить описание принятых решений
        /// </summary>
        public async void PrepareFormular()
        {
            await Task.Run(() => Word_Module.DocumentCreation.CreateFormular(this));
        }
        //-------------------------------------------------------9--------------------------------------------------------------
        /// <summary>
        /// Удалить приложение
        /// </summary>
        public async void Remove()
        {
            await DatabaseClass.DeleteApp(this);
        }
        //-------------------------------------------------------10--------------------------------------------------------------
        /// <summary>
        /// Обновить данные о приложении в БД
        /// </summary>
        public async void Update()
        {
            if(await CheckEquals())
                await DatabaseClass.UpdateApp(this);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Обновить данные по основному исполняемому файлу в рабочей директории и в релизе
        /// </summary>
        /// <returns></returns>
        public async Task UpdateFileInfoAsync()
        {
            Blocked = true;
            WorkingStatus = "Проверка актуальности приложения";
            await Task.Run(() =>
            {
                CurrentFile.GetFileData();
                ReleaseFile.GetFileData();
                ReestrFile.GetFileData();
            });
            UpToDate = (CurrentFile.Hash.Equals(ReleaseFile.Hash) && CurrentFile.Version.Equals(ReleaseFile.Version));
            IsInReestr = (ReleaseFile.Hash.Equals(ReestrFile.Hash) && ReleaseFile.Version.Equals(ReestrFile.Version));
            WorkingStatus = "";
            Blocked = false;
        }
        /// <summary>
        /// Обновить данные по основному исполняемому файлу в рабочей директории и в релизе
        /// </summary>
        /// <returns></returns>
        public void UpdateMainFileInfo()
        {
            CurrentFile.GetFileData();
            ReleaseFile.GetFileData();
            ReestrFile.GetFileData();
            UpToDate = (CurrentFile.Hash.Equals(ReleaseFile.Hash) && CurrentFile.Version.Equals(ReleaseFile.Version));
            IsInReestr = (ReleaseFile.Hash.Equals(ReestrFile.Hash) && ReleaseFile.Version.Equals(ReestrFile.Version));
        }
        /// <summary>
        /// Скопировать все требуемые файлы из каталога в каталог
        /// </summary>
        /// <param name="source">Каталог-источник</param>
        /// <param name="dest">Католог-цель</param>
        /// <param name="st">Флаг старта рекурсии</param>
        /// <returns></returns>
        public async Task CopyToFolderAsync(string source, string dest, bool st = true)
        {
            Blocked = true;
            WorkingStatus = "Копирование файлов в релиз";
            try
            {
                foreach (var dir in Directory.GetDirectories(source))
                {
                    await CopyToFolderAsync(dir, Path.Combine(dest, new DirectoryInfo(dir).Name), false);
                }
                foreach (var file in Directory.GetFiles(source))
                {
                    if (Service.GetListFromString(ServiceLib.Configuration.Configuration.Get("AllowedExtentions").ToString()).Contains(new FileInfo(file).Extension) && Service.FilterXMLFiles(file))
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
            if (st)
            {
                WorkingStatus = "";
                Blocked = false;
            }
        }
        /// <summary>
        /// Скопировать все требуемые файлы из каталога в каталог
        /// </summary>
        /// <param name="source">Каталог-источник</param>
        /// <param name="dest">Католог-цель</param>
        /// <param name="st">Флаг старта рекурсии</param>
        public void CopyToFolder(string source, string dest, bool st = true)
        {
            Blocked = true;
            WorkingStatus = "Копирование файлов в релиз";
            try
            {
                foreach (var dir in Directory.GetDirectories(source))
                {
                    CopyToFolder(dir, Path.Combine(dest, new DirectoryInfo(dir).Name), false);
                }
                foreach (var file in Directory.GetFiles(source))
                {
                    if (Service.GetListFromString(ServiceLib.Configuration.Configuration.Get("AllowedExtentions").ToString()).Contains(new FileInfo(file).Extension) && Service.FilterXMLFiles(file))
                    {
                        if (!Directory.Exists(dest))
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
        /// <summary>
        /// Проверка уникальности данных по приложению
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
                CurrentFile = this.CurrentFile,
                ReleaseFile = this.ReleaseFile,
                ReestrFile = this.ReestrFile,
                Name = this.Name,
                OtherSoft = this.OtherSoft,
                Parent = this.Parent,
                Platform = this.Platform,
                ReleaseDirectory = this.ReleaseDirectory,
                Report = this.Report,
                SourceDirectory = this.SourceDirectory,
                Status = this.Status,
                SUBD = this.SUBD,
                UpToDate = this.UpToDate,
                UserCategories = this.UserCategories
            };
        }
        #endregion
    }
}
