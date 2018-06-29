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

        private int floorId,tableZoneId;

        private int IsEditFloor,isEditFloorZone;

        public table_panel(string value)
        {
            InitializeComponent();
            user_type = value;

            this.IsEditFloor = 0;
            this.isEditFloorZone = 0;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void renderFloor()
        {
            MySqlConnection con = Models.DBConfiguration.DBCON();
            string query = "select * from floors";
            try
            {
                con.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                DataSet ds = new DataSet();
                da.Fill(ds, "floors");

                cb_floor.ItemsSource = ds.Tables[0].DefaultView;
                cb_floor.DisplayMemberPath = ds.Tables[0].Columns["name"].ToString();
                cb_floor.SelectedValuePath = ds.Tables[0].Columns["id"].ToString();
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
                            this.updateFloorGridView(this.selectRecord("select * from floors"));
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
                            this.updateFloorGridView(this.selectRecord("select * from floors"));
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
                        this.updateFloorGridView(this.selectRecord("select * from floors"));
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

        private void updateFloorGridView(DataTable dataTable)
        {
            dataTable.Columns["name"].ColumnName = "Floor Names";
            dataTable.Columns["id"].ColumnName = "ID";

            DataView dv = new DataView(dataTable);
            DataTable dt = dv.ToTable(true, "ID", "Floor Names");

          
            dgv_floors.ItemsSource = dt.DefaultView;
            dgv_floors.CanUserAddRows = false;
        }

        private void dgv_floors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataRowView selected_row = dataGrid.SelectedItem as DataRowView;

            if (selected_row != null)
            {
                this.IsEditFloor = 1;
                btn_addFloor.Content = "UPDATE";
                btn_delete.Visibility = Visibility.Visible;
                btn_cancel.Visibility = Visibility.Visible;
                txt_floorName.Text = selected_row["Floor Names"].ToString();
                this.floorId = int.Parse(selected_row["ID"].ToString());
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = "Tables";
            login_type.Content = String.Format("Authenticated as {0}", this.user_type);
            btn_delete.Visibility = Visibility.Hidden;
            btn_cancel.Visibility = Visibility.Hidden;

            this.updateFloorGridView(this.selectRecord("select * from floors"));


        }

        private DataTable selectRecord(string query)
        {

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

            return t1;
        }

        private void undoTableZoneUpdate()
        {
            txt_zonename.Text = "";
            txt_numoftable.Text = "";
            btn_addTableZone.Content = "ADD";
            btn_tbZoneCancel.Visibility = Visibility.Hidden;
            btn_deleteTableZone.Visibility = Visibility.Hidden;
            cb_floor.SelectedIndex = 0;
        }


        private void updateTableZoneGridView(DataTable dataTable)
        {
            dataTable.Columns["id"].ColumnName = "ID";
            dataTable.Columns["name"].ColumnName = "Floor Name";
            dataTable.Columns["name1"].ColumnName = "Table Zone Name";
            dataTable.Columns["num_tables"].ColumnName = "Number of Table";

            DataView dv = new DataView(dataTable);
            DataTable dt = dv.ToTable(true,"ID", "Floor Name", "Table Zone Name", "Number of Table");

            dgv_tableZone.ItemsSource = dt.DefaultView;
            dgv_tableZone.CanUserAddRows = false;
        }

        private void Grid_TableZoneLoaded(object sender, RoutedEventArgs e)
        {
            this.renderFloor();
            this.updateTableZoneGridView(this.selectRecord("select table_zones.id , floors.name , table_zones.name , table_zones.num_tables from table_zones inner join floors where table_zones.floor_id = floors.id"));
            this.undoTableZoneUpdate();
        }
  

        private void txt_searchTableZone_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btn_addTableZone_Click(object sender, RoutedEventArgs e)
        {
            string floorId = cb_floor.SelectedValue.ToString();
            string zone_name = txt_zonename.Text.ToString();
            string numOfTable = txt_numoftable.Text.ToString();

            MySqlConnection con = Models.DBConfiguration.DBCON();
            if (this.isEditFloorZone == 0)
            {

                string query = "insert into table_zones (name,floor_id,num_tables) values ('" + txt_zonename.Text.ToString() + "',"+int.Parse(floorId)+" , "+int.Parse(numOfTable)+" )";

                    try
                    {
                        con.Open();
                        MySqlCommand cmd = new MySqlCommand();
                        cmd.CommandText = query;
                        cmd.Connection = con;

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            this.undoTableZoneUpdate();
                            MessageBox.Show("Record has been inserted");
                            this.updateTableZoneGridView(this.selectRecord("select table_zones.id , floors.name , table_zones.name , table_zones.num_tables from table_zones inner join floors where table_zones.floor_id = floors.id"));
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
            else
            {

                int floorid = int.Parse(cb_floor.SelectedValue.ToString());
                int numTable = int.Parse(txt_numoftable.Text.ToString());

                string query = "UPDATE `table_zones` SET `floor_id` = '"+floorId+"', `name` = '"+ txt_zonename.Text.ToString() + "', `num_tables` = "+ numTable + " WHERE `table_zones`.`id` = "+this.tableZoneId+" ";
                try
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.CommandText = query;
                    cmd.Connection = con;

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        this.undoTableZoneUpdate();
                        MessageBox.Show("Record has been updated");
                        this.updateTableZoneGridView(this.selectRecord("select table_zones.id , floors.name , table_zones.name , table_zones.num_tables from table_zones inner join floors where table_zones.floor_id = floors.id"));
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

 

        private void btn_deleteTableZone_Click(object sender, RoutedEventArgs e)
        {


        }

        private void dgv_tableZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = (DataGrid)sender;
            DataRowView selected_row = dataGrid.SelectedItem as DataRowView;

            if (selected_row != null)
            {

                this.isEditFloorZone = 1;
                btn_addTableZone.Content = "UPDATE";
                btn_tbZoneCancel.Visibility = Visibility.Visible;
                btn_deleteTableZone.Visibility = Visibility.Visible;
                
                this.tableZoneId = int.Parse(selected_row["ID"].ToString());
                txt_zonename.Text = selected_row["Table Zone Name"].ToString();
                txt_numoftable.Text = selected_row["Number of Table"].ToString();
                cb_floor.SelectedValue = this.selectFloorIdByName(this.tableZoneId);

            }
        }

        private void btn_tbZoneCancel_Click(object sender, RoutedEventArgs e)
        {
            this.undoTableZoneUpdate();
        }

        private int selectFloorIdByName(int tableZoneId)
        {
            string query = "select floor_id from table_zones where id = "+tableZoneId;
            
            con = Models.DBConfiguration.DBCON();

            int floorId = 0;

            try
            {
                con.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandText = query;
                cmd.Connection = con;
                MySqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    floorId = int.Parse(dr["floor_id"].ToString());
                }
                else
                {
                    floorId = 0;
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

            return floorId;

        }

        private void btn_main_menu_Click(object sender, RoutedEventArgs e)
        {
            main_panel main_Panel = new main_panel(this.user_type);
            main_Panel.Owner = this;
            this.Hide();
            main_Panel.Show();
        }
    }
}
