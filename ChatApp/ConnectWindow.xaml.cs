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
	}
}
