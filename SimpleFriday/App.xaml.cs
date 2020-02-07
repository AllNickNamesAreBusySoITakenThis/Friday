using System;
using System.Collections.Generic;
//using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ServiceLib.Configuration;

namespace SimpleFriday
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            FridayLib.MainClass.ErrorInLibrary += MainClass_ErrorInLibrary;
            InitSettings();
        }

        void InitSettings()
        {
            Configuration.Add("Server", SettingType.String, "192.0.0.165\\MYSERVER");
            Configuration.Add("Database", SettingType.String, "TestDataBase");
            Configuration.Add("User", SettingType.String, "ORPO");
            Configuration.Add("Password", SettingType.String, "Bzpa/123456789");
            Configuration.Add("SpreadsheetAddress", SettingType.String, "10dymgee_7SNKLRwf9nS533pJpTMk1tLbndR9BmdO8As");
            Configuration.Add("SpreadsheetId", SettingType.Integer, 1515691245);
            Configuration.Add("AllowedExtentions", SettingType.String, FridayLib.Service.GetStringFromCollecction(new List<string>() { ".dll", ".exe", ".xml", ".txt", ".png", ".jpg", ".jpeg", ".bmp", ".ico", ".config", ".json" }));
            Configuration.Load(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "Friday"));
        }

        private void MainClass_ErrorInLibrary(string message)
        {
            Dispatcher.Invoke(()=>MessageBox.Show(Current.MainWindow,message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error));
        }
    }
}
