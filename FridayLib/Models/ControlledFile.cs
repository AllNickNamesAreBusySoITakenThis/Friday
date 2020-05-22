﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{

    public class ControlledFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string address="";
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged("Address");
            }
        }

        private string hash = "";
        public string Hash
        {
            get { return hash; }
            private set
            {
                hash = value;
                OnPropertyChanged("Hash");
            }
        }
        private string version = "";
        public string Version
        {
            get { return version; }
            private set
            {
                version = value;
                OnPropertyChanged("Version");
            }
        }
        private string changeDate = "";
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