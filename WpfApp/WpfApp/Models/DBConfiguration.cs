using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp.Models
{
    class DBConfiguration
    {
        public static MySqlConnection DBCON()
        {
            MySqlConnection con = new MySqlConnection("server=localhost; user=root; password=''; database=restaurant");
            return con;
        }
    }
}
