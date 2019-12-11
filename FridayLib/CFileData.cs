using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using NLog;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{

    public class CFileData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private DateTime date= new DateTime();
        private string version="";
        private string hash="";
        private Logger logger = LogManager.GetCurrentClassLogger();
        static private Logger sLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Дата изменения файла
        /// </summary>
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }
        /// <summary>
        /// SHA1 Хеш-сумма файла 
        /// </summary>
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
        public string Version
        {
            get { return version; }
            set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }

        public CFileData(string filepath)
        {
            FileInfo fi = new FileInfo(filepath);
            Date = fi.LastWriteTime;
            Hash = GetHash(filepath);
            Version = GetVersion(filepath);
        }
        public CFileData()
        {

        }
        /// <summary>
        /// Получить SHA1 хеш-сумму файла
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetHash(string filePath)
        {
            try
            {
                string hash = "";
                using (FileStream fs = File.OpenRead(filePath))
                using (var cryptoProvider = new SHA1CryptoServiceProvider())
                {
                    byte[] fileData = new byte[fs.Length];
                    fs.Read(fileData, 0, (int)fs.Length);
                    hash = BitConverter.ToString(cryptoProvider.ComputeHash(fileData)).Replace("-", "");
                }
                return hash;
            }
            catch (Exception ex)
            {
                sLogger.Error("Ошибка получения хеш-суммы файла {0}: {1}", filePath, ex.Message);
                return "-";
            }
        }
        /// <summary>
        /// Получить версию исполняемого файла
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetVersion(string filePath)
        {
            try
            {
                if(Executable(filePath))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(filePath);
                    return fvi.FileVersion;
                }
                else
                {
                    return "-";
                }
            }
            catch
            {
                return "-";
            }
        }
        /// <summary>
        /// Проверить, является ли файл исполняемым
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool Executable(string filePath)
        {
            try
            {
                System.Reflection.AssemblyName testAssembly =
                    System.Reflection.AssemblyName.GetAssemblyName(filePath);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (BadImageFormatException)
            {
                return false;
            }
            catch (FileLoadException)
            {
                return true;
            }
        }
    }
}
