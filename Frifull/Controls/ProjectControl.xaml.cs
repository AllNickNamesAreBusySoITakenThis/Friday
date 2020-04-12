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
    /// Логика взаимодействия для TileControl.xaml
    /// </summary>
    public partial class ProjectControl : UserControl, INavigationAware
    {
        public ProjectControl()
        {
            InitializeComponent();
        }

        public void NavigatedFrom(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        {
            
        }

        public void NavigatedTo(DevExpress.Xpf.WindowsUI.Navigation.NavigationEventArgs e)
        {
            //header.Header = (e.Parameter as ControlledProject).Name;
            //tiles.ItemsSource = (e.Parameter as ControlledProject).Apps;
            this.DataContext = (e.Parameter as ControlledProject);
        }

        public void NavigatingFrom(NavigatingEventArgs e)
        {
            
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
