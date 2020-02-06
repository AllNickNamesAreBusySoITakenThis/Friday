using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        }

        private void MainClass_ErrorInLibrary(string message)
        {
            MessageBox.Show(message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
