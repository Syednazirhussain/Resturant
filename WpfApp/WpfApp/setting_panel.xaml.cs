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

        private string name, email, status, type, password, confirm_passowrd;

        public setting_panel(string value)
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
            this.Title = "Users";
            login_type.Content = String.Format("Authenticated as {0}", this.user_type);
        }

        private void get_users(object sender, RoutedEventArgs e)
        {
            dgv_users.ItemsSource = this.getAllUsers().DefaultView;
            dgv_users.AutoGenerateColumns = true;
            dgv_users.CanUserAddRows = false;
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
        private void txt_searchByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string name = txt_searchByName.Text.ToString();
            string query;
            if (name.Length > 0)
            {
                query = "select * from users where name like '"+name+"%'";
            }
            else
            {
                query = "select * from users";
            }

            DataTable t1 = new DataTable();
            con = Models.DBConfiguration.DBCON();
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

            dgv_users.ItemsSource = t1.DefaultView;
            dgv_users.AutoGenerateColumns = true;
            dgv_users.CanUserAddRows = false;
        }

        private void renderUserType()
        {
            MySqlConnection con = Models.DBConfiguration.DBCON();
            string query = "select * from user_roles";
            try
            {
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds, "user_roles");
                cb_userType.ItemsSource = ds.Tables[0].DefaultView;
                cb_userType.DisplayMemberPath = ds.Tables[0].Columns["name"].ToString();
                cb_userType.SelectedValuePath = ds.Tables[0].Columns["code"].ToString();
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

        private void btn_addUser_Click(object sender, RoutedEventArgs e)
        {
            bool isError = true;
            int errorCount = 0;
            int user_status_id;

            this.name = txt_username.Text.ToString();
            this.email = txt_email.Text.ToString();
            this.type = cb_userType.SelectedValue.ToString();
            this.password = txt_password.Password;
            this.confirm_passowrd = txt_comfirmpassword.Password;

            if(rbtn_active.IsChecked == true)
            {
                this.status = "Active";
            }
            else
            {
                this.status = "De-Active";
            }

            if(this.validateUser(isError, errorCount).Equals(false))
            {
                this.hideError();
                if(this.status.Equals("Active"))
                {
                    user_status_id = 1;
                }
                else
                {
                    user_status_id = 0;
                }
                MySqlConnection con = Models.DBConfiguration.DBCON();
                string query = "insert into users (name,email,password,user_role_code,user_status_id) values ('"+this.name+ "','" + this.email + "','" + this.password + "','" + this.type + "','" + user_status_id + "' )";
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = query;
                    cmd.Connection = con;

                    int result = cmd.ExecuteNonQuery();

                    if(result > 0)
                    {
                        this.nullInputField();
                        MessageBox.Show("Record has been inserted");
                    }
                    else
                    {
                        MessageBox.Show("There is some problem while inserting records");
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

        private void btn_main_menu_Click(object sender, RoutedEventArgs e)
        {
            main_panel main_Panel = new main_panel(this.user_type);
            main_Panel.Owner = this;
            this.Hide();
            main_Panel.Show();
        }



        private void nullInputField()
        {
            txt_username.Text = "";
            txt_email.Text = "";
            txt_password.Password = "";
            txt_comfirmpassword.Password = "";
        }
        
        private bool validateUser(bool isError,int errorCount)
        {
            this.hideError();
            if (!IsValidEmail(this.email))
            {
                errorCount++;
                lbl_email.Content = "Please enter valid email";
            }

            if(this.name.Length == 0)
            {
                errorCount++;
                lbl_name.Content = "Name is required";
            }
            
            if (this.password.Length == 0)
            {
                errorCount++;
                lbl_password.Content = "Password is required";
            }


            if (this.confirm_passowrd.Length == 0)
            {
                errorCount++;
                lbl_confirmpassword.Content = "Confirm password is required";
            }


            if (!this.password.Equals(this.confirm_passowrd))
            {
                errorCount++;
                lbl_confirmpassword.Content = "Password and confirm password does not match";
            }

            if(errorCount > 0)
            {
                isError = true;
            }
            else
            {
                isError = false;
            }

            return isError;
        }

        private void hideError()
        {
            lbl_name.Content = "";
            lbl_email.Content = "";
            lbl_password.Content = "";
            lbl_confirmpassword.Content = "";
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void add_new_user(object sender, RoutedEventArgs e)
        {
            this.renderUserType();
            rbtn_active.IsChecked = true;
            cb_userType.SelectedIndex = 0;
        }
    }
}
