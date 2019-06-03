using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace FridayLib
{
    /// <summary>
    /// Описание контролируемого файла
    /// </summary>

    public class CFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private string name;
        /// <summary>
        /// Относительное имя файла
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


        private string sourcePath;
        /// <summary>
        /// Путь к источнику разрабатываемого проекта
        /// </summary>
        public string SourcePath
        {
            get { return sourcePath; }
            set
            {
                sourcePath = value;
                OnPropertyChanged("SourcePath");
            }
        }


        private string releasePath;
        /// <summary>
        /// Путь к релизу проекта
        /// </summary>
        public string ReleasePath
        {
            get { return releasePath; }
            set
            {
                releasePath = value;
                OnPropertyChanged("ReleasePath");
            }
        }


        private string lastHash;
        /// <summary>
        /// Результат предыдущей проверки
        /// </summary>
        public string LastHash
        {
            get { return lastHash; }
            set
            {
                lastHash = value;
                OnPropertyChanged("LastHash");
            }
        }


        private string currentHash;
        /// <summary>
        /// Результат текущей проверки
        /// </summary>
        public string CurrentHash
        {
            get { return currentHash; }
            set
            {
                currentHash = value;
                OnPropertyChanged("CurrentHash");
            }
        }

        public string FullPath
        {
            get { return Path.Combine(SourcePath, Name); }
        }

        public void RefreshProject()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(SourcePath);
            foreach(var file in GetFiles(directoryInfo))
            {
                File.Copy(file.FullName, Path.Combine(ReleasePath, GetRelativePath(file)));
            }            
        }

        private string GetRelativePath(FileInfo file)
        {
            return file.FullName.Replace(SourcePath, "");
        }
        private List<FileInfo> GetFiles(DirectoryInfo root)
        {
            List<FileInfo> result = new List<FileInfo>();
            result.AddRange(root.GetFiles());
            foreach(var dir in root.GetDirectories())
            {
                result.AddRange(GetFiles(dir));
            }
            return result;
        }

        public void ComputeMD5Checksum()
        {
            try
            {
                if (File.Exists(FullPath))
                {
                    using (FileStream fs = System.IO.File.OpenRead(FullPath))
                    {
                        MD5 md5 = new MD5CryptoServiceProvider();
                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, (int)fs.Length);
                        byte[] checkSum = md5.ComputeHash(fileData);
                        string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                        CurrentHash = result;
                    }
                }
                else
                {
                    CurrentHash = "";
                }
            }
            catch (Exception ex)
            {
               
            }

        }
    }
}
