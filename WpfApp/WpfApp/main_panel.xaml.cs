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
    /// Interaction logic for main_panel.xaml
    /// </summary>
    public partial class main_panel : Window
    {
        string user_type;
        public main_panel(string value)
        {
            InitializeComponent();
            user_type = value;
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {

            this.Hide();
            MainWindow login_window = new MainWindow();
            login_window.Show();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Stallyons Technology";
            txt_userType.Text = String.Format("Authenticated as {0}",user_type);
            if (user_type.Equals("sales_person"))
            {
                btn_setting.IsEnabled = false;
            }
        }

        private void btn_setting_Click(object sender, RoutedEventArgs e)
        {
            new setting_panel(user_type).ShowDialog();
            this.Hide();

        }
    }
}
