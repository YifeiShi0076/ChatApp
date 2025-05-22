using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ChatApp.Service
{
    public class MysqlTester
    {
        public static bool TestConnection()
        {
            // TODO：根据你自己的账号／密码／库名改一下
            string connStr =
                "server=127.0.0.1;" +
                "port=3306;" +
                "user=root;" +
                "password=root;" +
                "database=chatapp;" +
                "SslMode=None;";

            try
            {
                using var conn = new MySqlConnection(connStr);
                conn.Open();                                   // 打开连接

                // 查询一个简单的值，看连通后能否执行 SQL
                using var cmd = new MySqlCommand("SELECT VERSION()", conn);
                string version = cmd.ExecuteScalar()!.ToString();
                Console.WriteLine($"MySQL 版本：{version}");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" 连接失败：{ex.Message}");
                return false;
            }
        }

 //       public static void Main()
 //       {
 //           Console.WriteLine("开始测试 MySQL 连接…");
 //           bool ok = TestConnection();
 //           Console.WriteLine(ok ? "连接成功" : "连接失败");
 //           Console.ReadLine();
 //       }

    }
}
