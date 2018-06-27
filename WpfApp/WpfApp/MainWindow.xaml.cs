using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using MySql.Data.MySqlClient;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.renderUserType();
            this.Title = "Stallyons Technology";
            cbn_type.SelectedIndex = 0;
        }

        private void renderUserType()
        {
            MySqlConnection con;
            con = this.DBCON();
            string query = "select * from user_roles";
            try
            {
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds, "user_roles");
                cbn_type.ItemsSource = ds.Tables[0].DefaultView;
                cbn_type.DisplayMemberPath = ds.Tables[0].Columns["name"].ToString();
                cbn_type.SelectedValuePath = ds.Tables[0].Columns["code"].ToString();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }
        }
        

        private MySqlConnection DBCON()
        {
            MySqlConnection con = new MySqlConnection("server=localhost; user=root; password=''; database=restaurant");
            return con;
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {

            string type = cbn_type.SelectedValue.ToString();
            string query = "select * from users where email = '" + txt_username.Text + "' and password = '" + txt_password.Password + "' and user_role_code = '" + type + "'  limit 1";

            MySqlConnection con = this.DBCON();
            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand(query, con);
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    new main_panel(type).ShowDialog();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid email or password");
                }
            }
            catch(MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

        }
    }

    public class StringValue
    {
        public StringValue(string s)
        {
            _value = s;
        }
        public string Value { get { return _value; } set { _value = value; } }
        string _value;
    }

    //dgv.ItemsSource = t1.DefaultView;
    //dgv.AutoGenerateColumns = true;
    //dgv.CanUserAddRows = false;
}
