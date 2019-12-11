using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Diagnostics;

namespace FridayLib
{
    /// <summary>
    /// Описание контролируемого файла
    /// </summary>

    public class CFile : INotifyPropertyChanged
    {
        public const string ForbEXT = ".pdb;.xml;.vshost;";

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private int id = 0;
        private string projectName = "";
        private string name = "";
        private string sourcePath = "";
        private string releasePath = "";
        private string currentHash = "";
        private string lastHash = "";
        private DateTime date;
        private DateTime rDate;
        private string rVersion="";
        private string version = "";

        /// <summary>
        /// Идентификатор приложения в БД
        /// </summary>
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }
        /// <summary>
        /// Имя проекта
        /// </summary>
        public string ProjectName
        {
            get { return projectName; }
            set
            {
                projectName = value;
                OnPropertyChanged("ProjectName");
            }
        }
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
        /// <summary>
        /// Результат предыдущей проверки
        /// </summary>
        public string ReleaseHash
        {
            get { return lastHash; }
            set
            {
                lastHash = value;
                OnPropertyChanged("LastHash");
            }
        }
        /// <summary>
        /// Результат текущей проверки
        /// </summary>
        public string SourceHash
        {
            get { return currentHash; }
            set
            {
                currentHash = value;
                OnPropertyChanged("CurrentHash");
            }
        }
        public DateTime SourceDate
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }
        public DateTime ReleaseDate
        {
            get { return rDate; }
            set
            {
                rDate = value;
                OnPropertyChanged("ReleaseDate");
            }
        }
        public string ReleaseVersion
        {
            get { return rVersion; }
            set
            {
                rVersion = value;
                OnPropertyChanged("ReleaseVersion");
            }
        }
        public string SourceVersion
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }

        public string FullPath
        {
            get { return Path.Combine(SourcePath, Name); }
        }
        public string FullReleasePath
        {
            get { return Path.Combine(ReleasePath, Name); }
        }

        public async void RefreshProject()
        {
            await Task.Factory.StartNew(() =>
            {
                if (!string.IsNullOrEmpty(SourcePath) & !string.IsNullOrEmpty(ReleasePath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(SourcePath);
                    foreach (var dir in GetDirectories(new DirectoryInfo(SourcePath)))
                    {
                        var tempDir = new DirectoryInfo(dir.FullName.Replace(SourcePath, ReleasePath));
                        if (!tempDir.Exists)
                        {
                            tempDir.Create();
                        }
                    }
                    foreach (var file in GetFiles(directoryInfo))
                    {
                        if(!ForbEXT.Contains(file.Extension))
                            File.Copy(file.FullName, ReleasePath + GetRelativePath(file), true);
                    }
                    GetFileInfo();
                }
            });
           
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

        private List<DirectoryInfo> GetDirectories(DirectoryInfo root)
        {
            List<DirectoryInfo> result = new List<DirectoryInfo>();
            foreach(var dir in root.GetDirectories())
            {
                result.Add(dir);
                result.AddRange(GetDirectories(dir));
            }
            return result;
        }

        string ComputeMD5Checksum(string adress)
        {
            try
            {
                if (File.Exists(adress))
                {
                    using (FileStream fs = System.IO.File.OpenRead(adress))
                    {
                        MD5 md5 = new MD5CryptoServiceProvider();
                        byte[] fileData = new byte[fs.Length];
                        fs.Read(fileData, 0, (int)fs.Length);
                        byte[] checkSum = md5.ComputeHash(fileData);
                        string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                        return result;
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public void GetFileInfo()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(FullPath);
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(FullPath);
                SourceVersion = myFileVersionInfo.FileVersion;
                SourceDate = fileInfo.LastWriteTime;
                SourceHash = ComputeMD5Checksum(FullPath);
                fileInfo = new FileInfo(FullReleasePath);
                myFileVersionInfo = FileVersionInfo.GetVersionInfo(FullReleasePath);
                ReleaseVersion = myFileVersionInfo.FileVersion;
                ReleaseHash = ComputeMD5Checksum(FullReleasePath);
                ReleaseDate = fileInfo.LastWriteTime;
            }
            catch
            {

            }
                        
        }

    }
}
