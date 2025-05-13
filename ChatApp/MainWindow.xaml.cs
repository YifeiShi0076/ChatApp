using System;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using StackExchange.Redis;

namespace RedisChatApp
{
	public partial class MainWindow : Window
	{
		private ConnectionMultiplexer redis;
		private ISubscriber subscriber;
		private string chatChannel;

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			string ip = IpTextBox.Text.Trim();
			string port = PortTextBox.Text.Trim();
			chatChannel = ChannelTextBox.Text.Trim();

			if (string.IsNullOrWhiteSpace(ip) || string.IsNullOrWhiteSpace(port) || string.IsNullOrWhiteSpace(chatChannel))
			{
				MessageBox.Show("请输入 IP、端口 和 频道。");
				return;
			}

			try
			{
				redis = await ConnectionMultiplexer.ConnectAsync($"{ip}:{port}");
				subscriber = redis.GetSubscriber();

				await subscriber.SubscribeAsync(chatChannel, (channel, message) =>
				{
					Dispatcher.Invoke(() => AddChatMessage(message, isSelf: false));
				});

				ClientListBox.Items.Add($"连接成功 - 频道: {chatChannel}");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Redis服务器连接失败：" + ex.Message);
			}
		}

		private async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			if (subscriber == null || string.IsNullOrWhiteSpace(chatChannel))
			{
				MessageBox.Show("请先连接 Redis 并指定频道。");
				return;
			}

			string message = MessageTextBox.Text.Trim();
			if (string.IsNullOrWhiteSpace(message))
				return;

			await subscriber.PublishAsync(chatChannel, message);
			AddChatMessage(message, isSelf: true);
			MessageTextBox.Clear();
		}

		private void AddChatMessage(string message, bool isSelf)
		{
			var paragraph = new Paragraph(new Run(message))
			{
				TextAlignment = isSelf ? TextAlignment.Right : TextAlignment.Left
			};

			ChatTextBox.Document.Blocks.Add(paragraph);
			ChatTextBox.ScrollToEnd();
		}
	}
}