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
    /// Interaction logic for table_panel.xaml
    /// </summary>
    public partial class table_panel : Window
    {
        string user_type;
        MySqlConnection con;
        public table_panel(string value)
        {
            InitializeComponent();
            user_type = value;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void btn_addFloor_Click(object sender, RoutedEventArgs e)
        {

        }

        private void floorname_TextChanged(object sender, TextChangedEventArgs e)
        {
                 
        }

        private DataTable getFloors()
        {

            DataTable t1 = new DataTable();
            con = Models.DBConfiguration.DBCON();
            string query = "select * from floors";
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = con;
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
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

        private void btn_main_menu_Click(object sender, RoutedEventArgs e)
        {
            main_panel main_Panel = new main_panel(this.user_type);
            main_Panel.Owner = this;
            this.Hide();
            main_Panel.Show();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Tables";
            login_type.Content = String.Format("Authenticated as {0}",this.user_type);


            DataTable floorTable = this.getFloors();

            floorTable.Columns["name"].ColumnName = "Floor Names";

            DataView dv = new DataView(floorTable);
            DataTable dt = dv.ToTable(true, "Floor Names");
            dgv_floors.ItemsSource = dt.DefaultView;
            dgv_floors.AutoGenerateColumns = true;
            dgv_floors.CanUserAddRows = false;
        }
    }
}
