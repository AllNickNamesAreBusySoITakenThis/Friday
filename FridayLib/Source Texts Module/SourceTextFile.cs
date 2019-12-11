using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using NLog;

namespace FridayLib
{

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
        private CFileData fileData;
        private Logger logger = LogManager.GetCurrentClassLogger();
        static private Logger sLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Полное имя файла
        /// </summary>
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; OnPropertyChanged("FullName"); }
        }
        /// <summary>
        /// Короткое имя программы
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
        /// Описание файла
        /// </summary>
        public string Description
        {
            get { return descr; }
            set { descr = value; OnPropertyChanged("Description"); }
        }
        /// <summary>
        /// Собственник файла
        /// </summary>
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
        /// Общие данные по файлу
        /// </summary>
        public CFileData FileData
        {
            get { return fileData; }
            set
            {
                fileData = value;
                OnPropertyChanged("FileData");
            }
        }

        public SourceTextFile()
        {

        }
        public SourceTextFile(FileInfo fileInfo, string addName)
        {
            FullName = addName == "" ? fileInfo.Name : string.Format("{0}\\{1}", addName, fileInfo.Name);
            Name = fileInfo.Name;
            Description = "";            
            Size = fileInfo.Length / 1024 < 1 ? string.Format("{0} B", fileInfo.Length) : string.Format("{0} kB", fileInfo.Length / 1024);
            Owner = "АО \"НПО \"Спецэлектромеханика\"";
            FileData = new CFileData(fileInfo.FullName);
        }
        /// <summary>
        /// Перевести данные в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}||{1}||{2}||{3}||{4}||{5}||{6}||{7}", FullName, Description, FileData.Date.ToString("dd.MM.yyyy HH:mm:ss"), FileData.Version, FileData.Hash, Size, Name, Owner);
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
                    FileData = new CFileData() { Date = Convert.ToDateTime(sSource[2]), Version = sSource[3], Hash = sSource[4] },  
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
    }
}
