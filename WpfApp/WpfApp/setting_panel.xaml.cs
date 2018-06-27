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
using System.Windows.Shapes;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for setting_panel.xaml
    /// </summary>
    public partial class setting_panel : Window
    {
        string user_type;
        public setting_panel(string value)
        {
            InitializeComponent();
            user_type = value;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
