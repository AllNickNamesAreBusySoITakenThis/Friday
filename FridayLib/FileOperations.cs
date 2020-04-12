using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib
{
    internal class FileOperations
    {
        internal static string GetCheckSumm(string filePath)
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
                Service.OnErrorInLibrary(string.Format("Ошибка при получении хеш-суммы: {0}", ex.Message));
                return "";
            }
        }

        internal static string GetChangeDate(string filePath)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                return fi.LastWriteTime.ToString("dd.MM.yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при получении даты изменения: {0}", ex.Message));
                return "";
            }
        }

        internal static string GetVersion(string filePath)
        {
            try
            {
                if (Executable(filePath))
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(filePath);
                    return fvi.FileVersion;
                }
                else
                    return "";
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при получении версии: {0}", ex.Message));
                return "";
            }
        }
        /// <summary>
        /// Проверить, является ли файл исполняемым
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool Executable(string filePath)
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
