using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridayLib.Text_Module
{
    public class Listing
    {
        public static void CreateListing(ControlledProject project)
        {
            try
            {
                var fData = ScanFolder(project.WorkingDirectory);
                if(fData!=null)
                {
                    File.WriteAllLines(Path.Combine(project.DocumentDirectory, "Листинг.txt"), fData.ToArray());
                }
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка создания листинга проекта {0}: {1}", project.Name, ex.Message));
            }
        }
        internal static List<string> ScanFolder(string folder)
        {
            try
            {
                List<string> result = new List<string>();
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                if (directoryInfo.Name != "bin" & directoryInfo.Name != "obj" & directoryInfo.Name != "Properties" & directoryInfo.Name != ".vs" & directoryInfo.Name.ToUpper() != "PACKEGES")
                {
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        var temp = GetFileData(file);
                        if (temp != null)
                            result.AddRange(temp);
                    }
                    foreach (var dir in directoryInfo.GetDirectories())
                    {
                        var tDirData = ScanFolder(dir.FullName);
                        if (tDirData != null)
                        {
                            result.AddRange(tDirData);
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary( string.Format("Ошибка сканироания папки: {0}\r\n", ex.Message));
                return null;
            }
        }
        internal static List<string> GetFileData(FileInfo fileInfo)
        {
            try
            {
                List<string> result = new List<string>();
                if (fileInfo.Extension == ".cs" | fileInfo.Extension == ".xaml")
                {
                    result.Add("-------------" + fileInfo.Name + "-------------");
                    result.Add("");
                    result.AddRange(File.ReadAllLines(fileInfo.FullName));
                    result.Add("");
                    result.Add("");
                }
                return result;
            }
            catch (Exception ex)
            {
                MainClass.OnErrorInLibrary(string.Format("Ошибка добавления файла в листинг {0}: {1}\r\n", fileInfo.Name, ex.Message));
                return null;
            }
        }
    }
}
