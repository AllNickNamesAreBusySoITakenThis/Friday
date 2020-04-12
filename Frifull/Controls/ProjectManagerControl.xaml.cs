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
    /// Логика взаимодействия для TileManagerControl.xaml
    /// </summary>
    public partial class ProjectManagerControl : UserControl
    {
        public ProjectManagerControl()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty ItemsSourceProperty =
                   DependencyProperty.Register(
                         "ItemsSource",
                          typeof(object),
                          typeof(ProjectManagerControl));

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
    }
}
