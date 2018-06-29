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

        private string user_role;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Stallyons Technology";
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private String encryptPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            String hash = System.Text.Encoding.ASCII.GetString(data);
            return hash;
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

            if(this.checkField(txt_username.Text.ToString(), txt_password.Password.ToString()))
            {
                String password = this.encryptPassword(txt_password.Password.ToString());

                string query = "select * from users where username = '" + txt_username.Text + "' and password = '" + password + "' limit 1";

                MySqlConnection con = Models.DBConfiguration.DBCON();
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {

                        dr.Read();
                        this.user_role = dr.GetString("user_role_code");

                        main_panel main_Panel = new main_panel(this.user_role);
                        main_Panel.Owner = this;
                        this.Hide();
                        main_Panel.Show();

                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password");
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
            }

        }


        private bool checkField(string username , string password)
        {
            if(username.Length == 0 && password.Length == 0)
            {
                MessageBox.Show("Please enter username and password");
                return false;
            }
            else
            {
                if(username.Length == 0)
                {
                    MessageBox.Show("Please enter username");
                    return false;
                }
                else if(password.Length == 0)
                {
                    MessageBox.Show("Please enter password");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


    }


}
