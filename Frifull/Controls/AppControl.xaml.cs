using DevExpress.Xpf.WindowsUI.Navigation;
using FridayLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Frifull
{
    /// <summary>
    /// Логика взаимодействия для AppControl.xaml
    /// </summary>
    public partial class AppControl : UserControl, INavigationAware
    {
        public AppControl()
        {
            InitializeComponent();
        }

        public void NavigatedFrom(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        {
            
        }

        public void NavigatedTo(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        {
            //header.Header = (e.Parameter as ControlledApp).Name;
            this.DataContext = e.Parameter as ControlledApp;
        }

        public void NavigatingFrom(NavigatingEventArgs e)
        {
            
        }

        private void ComboBoxEdit_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
