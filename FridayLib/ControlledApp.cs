using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{

    public class ControlledApp : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string name;
        private string sourceDirectory;
        private string description;
        private string releaseDirectory;
        private string documentDirectopry;
        private string mainFileName;
        private string mainFileVersion;
        private string mainFileHash;
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
        private ControlledProject parent;
        private bool isInReestr = false;
        private int id;
        private string mainFileReleaseVersion;
        private string mainFileReleaseHash;
        private string mainFileReleaseDate;


        /// <summary>
        /// Идентификатор приложения
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
        public string UserCategories
        {
            get { return userCategories; }
            set
            {
                userCategories = value;
                OnPropertyChanged("UserCategories");
            }
        }

        public string MainFilePath
        {
            get { return Path.Combine(SourceDirectory, MainFileName); }
        }
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
            await Task.Run(() =>
            {
                MainFileDate = FileOperations.GetChangeDate(MainFilePath);
                MainFileHash =  FileOperations.GetCheckSumm(MainFilePath);
                MainFileVersion = FileOperations.GetVersion(MainFilePath);
                MainFileReleaseDate = FileOperations.GetChangeDate(MainFileReleasePath);
                MainFileReleaseHash = FileOperations.GetCheckSumm(MainFileReleasePath);
                MainFileReleaseVersion = FileOperations.GetVersion(MainFileReleasePath);
            });           
            UpToDate = (MainFileVersion.Equals(MainFileReleaseVersion) && MainFileHash.Equals(MainFileReleaseHash));
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
        public async Task CopyToFolderAsync(string source, string dest)
        {
            try
            {
                foreach(var dir in Directory.GetDirectories(source))
                {
                    await CopyToFolderAsync(dir, Path.Combine(dest, new DirectoryInfo(dir).Name));
                }
                foreach(var file in Directory.GetFiles(source))
                {
                    if(Service.AllowedFileExtentions.Contains(new FileInfo(file).Extension))
                        File.Copy(file, Path.Combine(dest, new FileInfo(file).Name), true);
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка копирования в релиз: {0}", ex.Message));
            }
        }
        public void CopyToFolder(string source, string dest)
        {
            try
            {
                foreach (var dir in Directory.GetDirectories(source))
                {
                    CopyToFolder(dir, Path.Combine(dest, new DirectoryInfo(dir).Name));
                }
                foreach (var file in Directory.GetFiles(source))
                {
                    if (Service.AllowedFileExtentions.Contains(new FileInfo(file).Extension))
                        File.Copy(file, Path.Combine(dest, new FileInfo(file).Name), true);
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка копирования в релиз: {0}", ex.Message));
            }
        }
    }
}
