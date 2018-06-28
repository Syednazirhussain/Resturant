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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Stallyons Technology";
            login_type.Content = String.Format("Authenticated as {0}",user_type);
            if (user_type.Equals("sales_person"))
            {
                btn_settings.IsEnabled = false;
            }
        }

        
        private void logout_Click_1(object sender, RoutedEventArgs e)
        {

            MainWindow login_window = new MainWindow();
            login_window.Owner = this;
            this.Hide();
            login_window.Show();
            //#FF062693
        }

        private void btn_users_Click(object sender, RoutedEventArgs e)
        {
            setting_panel setting_Panel = new setting_panel(this.user_type);
            setting_Panel.Owner = this;
            this.Hide();
            setting_Panel.Show();
        }

        private void btn_tables_Click(object sender, RoutedEventArgs e)
        {
            table_panel table_Panel = new table_panel(this.user_type);
            table_Panel.Owner = this;
            this.Hide();
            table_Panel.Show();
        }
    }
}
