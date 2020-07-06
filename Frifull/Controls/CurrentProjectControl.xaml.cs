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
    /// Логика взаимодействия для CurrentProjectControl.xaml
    /// </summary>
    public partial class CurrentProjectControl : UserControl, INavigationAware
    {
        public CurrentProjectControl()
        {
            InitializeComponent();
        }

        public void NavigatedFrom(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        {
            
        }

        public void NavigatedTo(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        {
            this.DataContext = e.Parameter as ControlledProject;
        }

        public void NavigatingFrom(NavigatingEventArgs e)
        {
            
        }

        private void AddAppClick(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ControlledProject).AddApp();
        }
    }
}
