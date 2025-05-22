using ChatApp.Models;
using ChatApp.Service;
using ChatApp.Services;
using RedisChatApp;
using System;
using System.Collections.Generic;
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

namespace ChatApp
{
	/// <summary>
	/// ConnectWindow.xaml 的交互逻辑
	/// </summary>
	public partial class ConnectWindow : Window
	{
		public ConnectWindow()
		{
			InitializeComponent();

            LoadUsers();
        }

		private async void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			string ip = IpTextBox.Text.Trim();
			string port = PortTextBox.Text.Trim();

			if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(port))
			{
				MessageBox.Show("请填写 IP 和端口");
				return;
			}

			string clientId = $"Client-{Guid.NewGuid().ToString().Substring(0, 8)}";
			var redisService = new RedisChatService(clientId);

			bool connected = await redisService.ConnectAsync(ip, port);
			if (!connected)
			{
				MessageBox.Show("连接失败");
				return;
			}

			// 成功连接后跳转到聊天页面（频道等在聊天页面处理）
			var chatWindow = new MainWindow(redisService, clientId);
			chatWindow.Show();
			this.Close();
		}

		//测试
        private void OnTestMySqlClick(object sender, RoutedEventArgs e)
        {
            bool ok = MysqlTester.TestConnection();
            MessageBox.Show(ok ? "连接成功" : "连接失败", "MySQL 测试");
        }

        private readonly UserService _userService = new UserService();


        private void LoadUsers()
        {
            var list = _userService.GetAllUsers().ToList();
            DgUsers.ItemsSource = list;
        }

        /// <summary>
        /// “新增”按钮点击
        /// </summary>
        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            string name = TxtName.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("请输入姓名后再新增。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool success = _userService.AddUser(name);
            if (success)
            {
                MessageBox.Show("新增成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                // 插入成功后清空输入并刷新列表
                TxtName.Clear();
                LoadUsers();
            }

            // 清空输入框，刷新表格
            TxtName.Clear();
            LoadUsers();
        }

        /// <summary>
        /// “删除”按钮点击
        /// </summary>
        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            // 从 DataGrid 拿到当前选中的行
            if (DgUsers.SelectedItem is User u)
            {
                var rst = MessageBox.Show(
                    $"确认删除用户 “{u.Name}”(ID={u.Id})？",
                    "确认删除",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (rst == MessageBoxResult.Yes)
                {
                    bool ok = _userService.RemoveUser(u.Id);
                    if (ok)
                    {
                        MessageBox.Show("删除成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                    }
                    else
                    {
                        MessageBox.Show("删除失败，请检查数据库或日志。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("请先在表格中选择一行再删除。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
