using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Excel = Microsoft.Office.Interop.Excel;

namespace FridayLib
{
    public class SourceTextCreation
    {
        /// <summary>
        /// Список запрещенных к просмотру папок
        /// </summary>
        public static List<string> ForbiddenFolderNames { get; set; } = new List<string>() { "BIN", "OBJ", "PACKAGES", ".GIT", ".VS" };
        /// <summary>
        /// Получить список файлов в проекте
        /// </summary>
        /// <param name="rootDirectoryAddress">Директория проекта</param>
        /// <returns></returns>
        internal static ObservableCollection<SourceTextFile> ScanFolder(string rootDirectoryAddress, string addName = "", string existingTextFileName = "", string excelFileName="")
        {
            try
            {
                ObservableCollection<SourceTextFile> result = new ObservableCollection<SourceTextFile>();
                DirectoryInfo root = new DirectoryInfo(rootDirectoryAddress);
                foreach (var file in root.GetFiles())
                {
                    result.Add(new SourceTextFile(file, addName));
                }
                foreach (var folder in root.GetDirectories())
                {
                    if (!ForbiddenFolderNames.Contains(folder.Name.ToUpper()))
                    {
                        var add = addName == "" ? folder.Name : string.Format("{0}\\{1}", addName, folder.Name);
                        var temp = ScanFolder(folder.FullName, add, "");
                        foreach (var file in temp)
                        {
                            result.Add(file);
                        }
                    }
                }
                if (excelFileName != "")
                {
                    var savedData = GetFromExcel(excelFileName);
                    foreach (var nFile in result)
                    {
                        foreach (var oFile in savedData)
                        {
                            if (nFile.FullName == oFile.FullName && !string.IsNullOrEmpty(oFile.Description))
                            {
                                nFile.Description = oFile.Description;
                                nFile.Owner = oFile.Owner;
                                break;
                            }
                        }
                    }
                }   
                if (existingTextFileName != "")
                {
                    var savedData = GetFromTextFile(existingTextFileName);
                    foreach (var nFile in result)
                    {
                        foreach (var oFile in savedData)
                        {
                            if (nFile.FullName == oFile.FullName && !string.IsNullOrEmpty(oFile.Description))
                            {
                                nFile.Description = oFile.Description;
                                nFile.Owner = oFile.Owner;
                                break;
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при сканировании папки: {0}", ex.Message));
                return new ObservableCollection<SourceTextFile>();
            }
        }
        /// <summary>
        /// Сохранить данные о проекте в собственном формате
        /// </summary>
        /// <param name="collection">Коллекция файлов</param>
        /// <param name="path">Путь для сохранения</param>
        public static void SaveAsTextFile(ObservableCollection<SourceTextFile> collection, string path)
        {
            try
            {
                FileInfo fInfo = new FileInfo(path);
                if(!fInfo.Directory.Exists)
                {
                    Directory.CreateDirectory(fInfo.Directory.FullName);
                }
                List<string> writeData = new List<string>();
                foreach (var file in collection)
                {
                    writeData.Add(file.ToString());
                }
                File.WriteAllLines(path, writeData.ToArray());
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при сохранении файла данных об исходных кодах: {0}", ex.Message));
            }
        }
        /// <summary>
        /// Получить коллекцию файлов из собственного хранилища
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static ObservableCollection<SourceTextFile> GetFromTextFile(string path)
        {
            try
            {
                ObservableCollection<SourceTextFile> result = new ObservableCollection<SourceTextFile>();
                var data = File.ReadAllLines(path);
                foreach (var line in data)
                {
                    result.Add(SourceTextFile.FromString(line));
                }
                return result;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при чтении файла данных об исходных кодах: {0}", ex.Message));
                return new ObservableCollection<SourceTextFile>();
            }
        }
        internal static void SaveAsExcel(ObservableCollection<SourceTextFile> collection, string path)
        {
            try
            {
                FileInfo fInfo = new FileInfo(path);
                if (!fInfo.Directory.Exists)
                {
                    Directory.CreateDirectory(fInfo.Directory.FullName);
                }
                string samplePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Res", @"SampleTable.xlsx");
                Excel.Application excelapp = null;
                Excel.Sheets excelsheets; // переменная-список всх листов текущей книги
                Excel.Worksheet excelworksheet;// переменная определяет рабочий лист текущей книги
                //Excel.Range excelcells;// переменные ячейки экселя
                Excel.Workbooks excelappworkbooks;// Список всех книг приложения Эксель (ну там типо в одном окне работать можно с кучей документов)
                Excel.Workbook excelappworkbook; // переменная текущая книга
                excelapp = new Excel.Application();
                excelapp.Visible = false;
                excelapp.Workbooks.Open(samplePath,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);
                excelappworkbooks = excelapp.Workbooks; // готовим Эксель к работе: получаем список книг, выбираем первую (а в нашем случае и единственную) книгу, получаем список ее листов и выбираем первый 
                excelappworkbook = excelappworkbooks[1];
                excelsheets = excelappworkbook.Worksheets;
                excelworksheet = (Excel.Worksheet)excelsheets.get_Item(1);
                excelworksheet.Name = "Ведомость";
                for (int i = 0; i < collection.Count; i++)
                {
                    excelworksheet.Cells[i + 2, 1] = collection[i].Name;
                    excelworksheet.Cells[i + 2, 2] = collection[i].Description;
                    excelworksheet.Cells[i + 2, 3] = collection[i].FullName;
                    excelworksheet.Cells[i + 2, 4] = collection[i].Size;
                    excelworksheet.Cells[i + 2, 5] = collection[i].Version;
                    excelworksheet.Cells[i + 2, 6] = collection[i].Hash;
                    excelworksheet.Cells[i + 2, 7] = collection[i].Owner;
                }
                excelappworkbook.SaveAs(path);
                excelapp.Quit();
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при создании таблицы исходных кодов: {0}", ex.Message));
            }
        }

        public static ObservableCollection<SourceTextFile> GetFromExcel(string path)
        {
            try
            {
                if(File.Exists(path))
                {
                    ObservableCollection<SourceTextFile> result = new ObservableCollection<SourceTextFile>();
                    Excel.Application excelapp = null;
                    Excel.Sheets excelsheets; // переменная-список всх листов текущей книги
                    Excel.Worksheet excelworksheet;
                    Excel.Workbooks excelappworkbooks;// Список всех книг приложения Эксель (ну там типо в одном окне работать можно с кучей документов)
                    Excel.Workbook excelappworkbook; // переменная текущая книга
                    excelapp = new Excel.Application();
                    excelapp.Visible = false;
                    excelapp.Workbooks.Open(path,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing);
                    excelappworkbooks = excelapp.Workbooks; // готовим Эксель к работе: получаем список книг, выбираем первую (а в нашем случае и единственную) книгу, получаем список ее листов и выбираем первый 
                    excelappworkbook = excelappworkbooks[1];
                    excelsheets = excelappworkbook.Worksheets;
                    excelworksheet = (Excel.Worksheet)excelsheets.get_Item(1);
                    int counter = 2;
                    do
                    {
                        result.Add(new SourceTextFile()
                        {
                            FullName = excelworksheet.Cells[counter, 3].Value,
                            Name = excelworksheet.Cells[counter, 1].Value,
                            Description = excelworksheet.Cells[counter, 2].Value,
                            Owner = excelworksheet.Cells[counter, 7].Value,
                        });
                        counter++;
                    }
                    while (excelworksheet.Cells[counter + 1, 1].Value != null);
                    excelappworkbook.Save();
                    excelapp.Quit();
                    return result;
                }
                return new ObservableCollection<SourceTextFile>();
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при прочтении таблицы исходных кодов: {0}", ex.Message));
                return new ObservableCollection<SourceTextFile>();
            }
        }

        public static void CreateSourceTextList(ControlledProject project)
        {
            try
            {
                project.SourceTextFiles = UpdateSourceTextList(project);
                SaveAsExcel(project.SourceTextFiles, Path.Combine(project.DocumentDirectory, "Ведомость исходных текстов.xlsx"));
                SaveAsTextFile(project.SourceTextFiles, Path.Combine(project.DocumentDirectory, "SourceTexts.csv"));
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при сохранении исходных текстов: {0}", ex.Message));
            }
        }

        public static ObservableCollection<SourceTextFile> UpdateSourceTextList(ControlledProject project)
        {
            try
            {
                ObservableCollection<SourceTextFile> originalCollection = ScanFolder(project.WorkingDirectory);
                ObservableCollection<SourceTextFile> savedCollection = GetFromTextFile(Path.Combine(project.DocumentDirectory, "SourceTexts.csv"));
                foreach(var item in originalCollection)
                {
                    foreach(var sItem in savedCollection)
                    {
                        if(sItem.FullName.Equals(item.FullName))
                        {
                            item.Description = sItem.Description;
                            item.Owner = sItem.Owner;
                            break;
                        }
                    }
                }
                return originalCollection;
            }
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при создании исходных текстов: {0}", ex.Message));
                return new ObservableCollection<SourceTextFile>();
            }
        }
    }
}
