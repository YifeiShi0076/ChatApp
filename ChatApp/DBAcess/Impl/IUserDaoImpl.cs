using ChatApp.DBAcess.Dao;
using ChatApp.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.DBAcess.DB;

namespace ChatApp.DBAcess.Impl
{
    public class IUserDaoImpl : IUserDao
    {
        public int Insert(string name)
        {
            const string sql = "INSERT INTO users (name) VALUES (@name)";
            using IDbConnection conn = DbConnection.CreateConnection();
            using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
            cmd.Parameters.AddWithValue("@name", name);
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int id)
        {
            const string sql = "DELETE FROM users WHERE id = @id";
            using IDbConnection conn = DbConnection.CreateConnection();
            using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteNonQuery();
        }

        public IEnumerable<User> GetAll()
        {
            const string sql = "SELECT id, name FROM users";
            using IDbConnection conn = DbConnection.CreateConnection();
            using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
            using var reader = cmd.ExecuteReader();
            var list = new List<User>();
            while (reader.Read())
            {
                list.Add(new User
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name")
                });
            }
            return list;
        }
    }
}
