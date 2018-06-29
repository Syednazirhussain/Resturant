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

        private string name, email, username , status, type, password, confirm_passowrd;

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
            this.updateUserGridView(this.getAllUsers());
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

        private void updateUserGridView(DataTable dataTable)
        {
            DataTable floorTable = dataTable;

            floorTable.Columns["id"].ColumnName = "ID";
            floorTable.Columns["name"].ColumnName = "Name";
            floorTable.Columns["email"].ColumnName = "Email";
            floorTable.Columns["user_status_id"].ColumnName = "Status";
            floorTable.Columns["created_at"].ColumnName = "Created At";

            DataView dv = new DataView(floorTable);
            DataTable dt = dv.ToTable(true, "ID", "Name", "Email", "Status", "Created At");

            DataTable dtCloned = dt.Clone();
            dtCloned.Columns["Status"].DataType = typeof(string);
            dtCloned.Columns["Created At"].DataType = typeof(string);

            foreach (DataRow row in dt.Rows)
            {
                dtCloned.ImportRow(row);
            }

            if (dtCloned.Rows.Count > 0)
            {
                DataColumn statusColumn = dtCloned.Columns["Status"];
                DataColumn createdAtColumn = dtCloned.Columns["Created At"];

                foreach (DataRow row in dtCloned.Rows)
                {

                    if(row[statusColumn].Equals("1"))
                    {
                         row[statusColumn] = "Active";
                    }
                    else
                    {
                        row[statusColumn] = "DeActive";
                    }

                    DateTime enteredDate = DateTime.Parse(row[createdAtColumn].ToString());
                    row[createdAtColumn] = enteredDate.ToString("F");

                }
            }

            dgv_users.ItemsSource = dtCloned.DefaultView;
            dgv_users.CanUserAddRows = false;
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

            this.updateUserGridView(t1);
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

        private void btn_main_menu_Click(object sender, RoutedEventArgs e)
        {
            main_panel main_Panel = new main_panel(this.user_type);
            main_Panel.Owner = this;
            this.Hide();
            main_Panel.Show();
        }

        private void btn_addUser_Click(object sender, RoutedEventArgs e)
        {
            bool isError = true;
            int errorCount = 0;
            int user_status_id;

            this.name = txt_name.Text.ToString();
            this.username = txt_username.Text.ToString();
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
                string created_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                MySqlConnection con = Models.DBConfiguration.DBCON();

                String HashPassord = this.encryptPassword(this.password);

                string query = "insert into users (name,username,email,password,user_role_code,user_status_id,created_at) values ('" + this.name+ "','"+this.username+"','" + this.email + "','" + HashPassord + "','" + this.type + "','" + user_status_id + "','"+ created_at + "' )";
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

   

        private String encryptPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            String hash = System.Text.Encoding.ASCII.GetString(data);
            return hash;
        }
        

        private void nullInputField()
        {
            txt_name.Text = "";
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

            if (this.username.Length == 0)
            {
                errorCount++;
                lbl_username.Content = "Username is required";
            }

            if (this.password.Length == 0)
            {
                errorCount++;
                lbl_password.Content = "Password is required";
            }
            else
            {
                if(this.password.Length > 6)
                {
                    errorCount++;
                    lbl_password.Content = "Password must be 1 to 6 character long";
                }
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
