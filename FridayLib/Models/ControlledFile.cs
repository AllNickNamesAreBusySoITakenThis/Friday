using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{
    [Description("Исполняемый файл"), DisplayName("Исполняемый файл")]
    public class ControlledFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        [NonSerialized]
        private string address="";
        [Description("Адрес исполняемого файла"), DisplayName("Адрес")]
        [NotMapped]
        public string Address
        {
            get { return address; }
            set
            {
                //if (address!=value)
                //{
                    address = value;
                    GetFileData();
                    OnPropertyChanged("Address"); 
                //}
            }
        }
        [NonSerialized]
        private string hash = "";
        [Description("Хеш-сумма файла (SHA1)"), DisplayName("Хеш")]
        [NotMapped]
        public string Hash
        {
            get { return hash; }
            private set
            {
                hash = value;
                OnPropertyChanged("Hash");
            }
        }
        [NonSerialized]
        private string version = "";
        [Description("Версия файла"), DisplayName("Версия")]
        [NotMapped]
        public string Version
        {
            get { return version; }
            private set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }
        [NonSerialized]
        private string changeDate = "";
        [Description("Дата изменения файла"), DisplayName("Дата изменения")]
        [NotMapped]
        public string ChangeDate
        {
            get { return changeDate; }
            private set
            {
                changeDate = value;
                OnPropertyChanged("ChangeDate");
            }
        }

        public ControlledFile(string addr)
        {
            Address = addr;
            GetFileData();
        }
        public ControlledFile()
        {

        }

        public void GetFileData()
        {
            try
            {
                GetChangeDate(Address);
                GetCheckSumm(Address);
                GetVersion(Address);
            }
            catch(Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка получения информации о файле. Имя - {0}. Ошибка - {1}", Address, ex.Message));
            }
        }

        private void GetCheckSumm(string filePath)
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
                Hash = hash;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при получении хеш-суммы: {0}", ex.Message));
                Hash="";
            }
        }

        private void GetChangeDate(string filePath)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                ChangeDate = fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при получении даты изменения: {0}", ex.Message));
                ChangeDate = "";
            }
        }

        private void GetVersion(string filePath)
        {
            try
            {
                if (Executable(filePath))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(filePath);
                    Version = fvi.FileVersion;
                }
                else
                {
                    Version = "";
                }
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при получении версии: {0}", ex.Message));
                Version = "";
            }
        }
        /// <summary>
        /// Проверить, является ли файл исполняемым
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool Executable(string filePath)
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
