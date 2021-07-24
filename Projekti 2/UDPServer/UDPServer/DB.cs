using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UDPServer
{
    class DB
    {
        private static string connString = "Server=localhost;Port=3307;Database=siguridb;Uid=root;Pwd=;";

        private MySqlConnection connection = new MySqlConnection(connString);

        public void openConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void closeConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }
   
        public MySqlConnection getConnection()
        {
            return connection;
        }
    }
}
