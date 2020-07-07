using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using MongoDB.Bson;
using Google.Apis.Upload;
using System.Collections.ObjectModel;

namespace FridayLib
{
    [Serializable]
    public class SourceTextFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private string fullName = "";
        private string descr = "";
        private string name="";
        private string owner;
        private string size;
        private string version;
        private string hash;
        private string creationDate;
        private string fullPath = "";

        /// <summary>
        /// Полное имя файла
        /// </summary>
        [Category("Общее"), Description("Относительный путь до файла от исходной директории"), DisplayName("Путь к файлу")]
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; OnPropertyChanged("FullName"); }
        }
        /// <summary>
        /// Короткое имя программы
        /// </summary>
        [Category("Общее"), Description("Имя файла"), DisplayName("Имя файла")]
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
        /// Описание файла
        /// </summary>
        [Category("Общее"), Description("Описание содержимого файла"), DisplayName("Описание")]
        public string Description
        {
            get { return descr; }
            set { descr = value; OnPropertyChanged("Description"); }
        }
        /// <summary>
        /// Собственник файла
        /// </summary>
        [Category("Общее"), Description("Создатель файла"), DisplayName("Создатель")]
        public string Owner
        {
            get { return owner; }
            set
            {
                owner = value;
                OnPropertyChanged("Owner");
            }
        }
        /// <summary>
        /// Размер файла
        /// </summary>
        [Category("Общее"), Description("Размер файла"), DisplayName("Размер")]
        public string Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }
        /// <summary>
        /// Хеш-сумма файла
        /// </summary>
        [Category("Общее"), Description("Хеш-сумма файла"), DisplayName("Хеш")]
        public string Hash
        {
            get { return hash; }
            set
            {
                hash = value;
                OnPropertyChanged("Hash");
            }
        }

        /// <summary>
        /// Версия файла
        /// </summary>
        [Category("Общее"), Description("Версия файла (только для исполняемых)"), DisplayName("Версия")]
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }
        /// <summary>
        /// Полный путь к файлу
        /// </summary>
        [Category("Общее"), Description("Полный путь к файлу"), DisplayName("Полный путь")]
        public string FullPath
        {
            get { return fullPath; }
            set
            {
                fullPath = value;
                OnPropertyChanged("FullPath");
            }
        }

        /// <summary>
        /// Дата создания файла
        /// </summary>
        [Category("Общее"), Description("Дата изменения файла"), DisplayName("Дата изменения")]
        public string CreationDate
        {
            get { return creationDate; }
            set
            {
                creationDate = value;
                OnPropertyChanged("CreationDate");
            }
        }
        public int Id { get; set; }
        public ControlledProject Parent { get; set; }
        public int ParentId { get; set; }


        public SourceTextFile()
        {

        }
        public SourceTextFile(FileInfo fileInfo, string addName)
        {
            FullName = addName == "" ? fileInfo.Name : string.Format("{0}\\{1}", addName, fileInfo.Name);
            Name = fileInfo.Name;
            Description = "";
            FullPath = fileInfo.FullName;
            Size = fileInfo.Length / 1024 < 1 ? string.Format("{0} B", fileInfo.Length) : string.Format("{0} kB", fileInfo.Length / 1024);
            Owner = "АО \"НПО \"Спецэлектромеханика\"";
            Hash = FileOperations.GetCheckSumm(fileInfo.FullName);
            CreationDate = FileOperations.GetChangeDate(fileInfo.FullName);
            Version = FileOperations.GetVersion(fileInfo.FullName);
            //FileData = new CFileData(fileInfo.FullName);
        }
        /// <summary>
        /// Перевести данные в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}||{1}||{2}||{3}||{4}||{5}||{6}||{7}", FullName, Description, CreationDate, Version, Hash, Size, Name, Owner);
        }
        /// <summary>
        /// Прочитать данные из строки
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static SourceTextFile FromString(string source)
        {
            try
            {
                var sSource = source.Split(new string[] { "||" }, StringSplitOptions.None);
                return new SourceTextFile
                {
                    FullName = sSource[0],
                    Description = sSource[1],
                    CreationDate = sSource[2], 
                    Version = sSource[3], 
                    Hash = sSource[4],  
                    Size = sSource[5],
                    Name = sSource[6],
                    Owner = sSource[7]
                };
            }
            catch
            {
                return new SourceTextFile();
            }
        }
        public static BsonArray GetSourceTextsArray(ICollection<SourceTextFile> files)
        {
            BsonArray result = new BsonArray();
            foreach(var f in files)
            {
                result.Add(f.ToBsonElement());
            }
            return result;
        }
        public BsonDocument ToBsonElement()
        {
            return new BsonDocument
            {
                {"Name",Name},
                {"Description",Description},
                {"Owner",Owner},                
                {"FullName",FullName},
                {"FullPath",FullPath}
            };
        }
        public static SourceTextFile FromBsonDocument(BsonDocument source)
        {
            try
            {
                var result = new SourceTextFile
                {
                    Name = source["Name"].ToString(),
                    Description = source["Description"].ToString(),
                    Owner = source["Owner"].ToString(),                    
                    FullName = source["FullName"].ToString(),
                    FullPath = source["FullPath"].ToString(),
                };
                result.CreationDate = FileOperations.GetChangeDate(result.FullPath);
                result.Hash = FileOperations.GetCheckSumm(result.FullPath);
                result.Version = FileOperations.GetVersion(result.FullPath);
                var fileInfo = new FileInfo(result.FullPath);
                result.Size = fileInfo.Length / 1024 < 1 ? string.Format("{0} B", fileInfo.Length) : string.Format("{0} kB", fileInfo.Length / 1024);
                return result;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка получения данных о приложении из БД: {0}", ex.Message));
                return null;
            }
        }
        public static ObservableCollection<SourceTextFile> GetFilesFromBsonArray(BsonArray source)
        {
            ObservableCollection<SourceTextFile> result = new ObservableCollection<SourceTextFile>();
            foreach (BsonDocument item in source)
            {
                var res = SourceTextFile.FromBsonDocument(item);
                if (res != null)
                    result.Add(res);
            }
            return result;
        }
    }
}
