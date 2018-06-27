using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        MySqlConnection con;
        public setting_panel(string value)
        {
            InitializeComponent();
            user_type = value;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dgv_users.ItemsSource = this.getAllUsers().DefaultView;
            dgv_users.AutoGenerateColumns = true;
            dgv_users.CanUserAddRows = false;

            // check
        }


        private DataTable getAllUsers()
        {
  
            DataTable t1 = new DataTable();
            con = Models.DBConfiguration.DBCON();
            string query = "select * from users";
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = con;
                MySqlDataReader dr = cmd.ExecuteReader();
                if(dr.HasRows)
                {
                    t1.Load(dr);
                }

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

            return t1;
        }
    }
}
