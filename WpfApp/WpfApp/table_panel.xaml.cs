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

        private int floorId;

        private int IsEditFloor;

        public table_panel(string value)
        {
            InitializeComponent();
            user_type = value;

            this.IsEditFloor = 0;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void btn_deleteFloor_Click(object sender, RoutedEventArgs e)
        {
            if (txt_floorName.Text.ToString().Length == 0)
            {
                MessageBox.Show("Nothing to delete");
            }
            else
            {

                if (MessageBox.Show("Are you sure ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    btn_delete.Visibility = Visibility.Hidden;
                }
                else
                {
                    string query = "delete from floors where id = '" + this.floorId + "'";
                    try
                    {
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = query;
                        cmd.Connection = con;

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            this.undoFloorUpdateOperation();
                            MessageBox.Show("Record has been deleted");
                            this.updateGridView();
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
        }

        private void btn_addFloor_Click(object sender, RoutedEventArgs e)
        {
            MySqlConnection con = Models.DBConfiguration.DBCON();
            if (this.IsEditFloor == 0)
            {
                if (txt_floorName.Text.ToString().Length == 0)
                {
                    MessageBox.Show("Please provide floor name");
                }
                else
                {
                    string query = "insert into floors (name) values ('" + txt_floorName.Text.ToString() + "')";
                
                    try
                    {
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = query;
                        cmd.Connection = con;

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            txt_floorName.Text = "";
                            MessageBox.Show("Record has been inserted");
                            this.updateGridView();
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
            else
            {
                string query = "update floors set name = '" + txt_floorName.Text.ToString() + "' where id = '"+this.floorId+"'";
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = query;
                    cmd.Connection = con;

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        this.undoFloorUpdateOperation();
                        MessageBox.Show("Record has been updated");
                        this.updateGridView();
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

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.undoFloorUpdateOperation();
        }

        private void undoFloorUpdateOperation()
        {
            IsEditFloor = 0;
            txt_floorName.Text = "";
            btn_delete.Visibility = Visibility.Hidden;
            btn_cancel.Visibility = Visibility.Hidden;
            btn_addFloor.Content = "ADD";
        }

        private void floorname_TextChanged(object sender, RoutedEventArgs e)
        {
            string name =  txt_searchFloor.Text.ToString();

            string query;
            if (name.Length > 0)
            {
                query = "select * from floors where name like '" + name + "%'";
            }
            else
            {
                query = "select * from floors";
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



            t1.Columns["name"].ColumnName = "Floor Names";

            DataView dv = new DataView(t1);
            DataTable dt = dv.ToTable(true, "Floor Names");
            dgv_floors.ItemsSource = dt.DefaultView;
            dgv_floors.CanUserAddRows = false;
            

        }


        private void updateGridView()
        {
            DataTable floorTable = this.getFloors();

            floorTable.Columns["name"].ColumnName = "Floor Names";
            floorTable.Columns["id"].ColumnName = "ID";

            DataView dv = new DataView(floorTable);
            DataTable dt = dv.ToTable(true, "id","Floor Names");

          
            dgv_floors.ItemsSource = dt.DefaultView;
            dgv_floors.CanUserAddRows = false;
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
            btn_delete.Visibility = Visibility.Hidden;
            btn_cancel.Visibility = Visibility.Hidden;
            this.updateGridView();
        }

        private void dgv_floors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataRowView selected_row = dataGrid.SelectedItem as DataRowView;

            if(selected_row != null)
            {
                this.IsEditFloor = 1;
                btn_addFloor.Content = "UPDATE";
                btn_delete.Visibility = Visibility.Visible;
                btn_cancel.Visibility = Visibility.Visible;
                txt_floorName.Text = selected_row["Floor Names"].ToString();
                this.floorId = int.Parse(selected_row["ID"].ToString());
            }
        }
    }
}
