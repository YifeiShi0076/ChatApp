using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.DBAcess.DB
{
    public static class DbConnection
    {
        private static readonly string ConnString =
            ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

        public static IDbConnection CreateConnection()
        {
            var conn = new MySqlConnection(ConnString);
            conn.Open();
            return conn;
        }

    }
}
