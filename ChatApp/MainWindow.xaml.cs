using System;
using System.Printing;
using System.Text.Json;
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
		private string clientId;

		// 添加：已订阅的频道记录
		private HashSet<string> subscribedChannels = new HashSet<string>();

		public MainWindow()
		{
			InitializeComponent();
			clientId = $"Client-{Guid.NewGuid().ToString().Substring(0, 8)}";
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

			if (subscribedChannels.Contains(chatChannel))
			{
				ClientListBox.Items.Add($"[{clientId}] 你已经订阅该频道，请勿重复订阅。");
				return;
			}

			try
			{
				redis ??= await ConnectionMultiplexer.ConnectAsync($"{ip}:{port}");
				subscriber ??= redis.GetSubscriber();

				await subscriber.SubscribeAsync(chatChannel, (channel, message) =>
				{
					try
					{
						var data = JsonSerializer.Deserialize<ChatMessage>(message);
						if (data?.Sender != clientId)
						{
							Dispatcher.Invoke(() =>
							{
								AddChatMessage($"[{data.Sender}]: {data.Message}", isSelf: false);
							});
						}
					}
					catch
					{
						// 忽略格式错误
					}
				});

				// 添加记录
				subscribedChannels.Add(chatChannel);
				ClientListBox.Items.Add($"[{clientId}] 连接成功 - 频道: {chatChannel}");
			}
			catch (Exception ex)
			{
				MessageBox.Show("连接失败：" + ex.Message);
			}
		}

		private async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			if (subscriber == null || string.IsNullOrWhiteSpace(chatChannel))
			{
				MessageBox.Show("请先连接 Redis 并指定频道。");
				return;
			}

			string text = MessageTextBox.Text.Trim();
			if (string.IsNullOrWhiteSpace(text)) return;

			var payload = new ChatMessage
			{
				Sender = clientId,
				Message = text
			};

			string json = JsonSerializer.Serialize(payload);
			await subscriber.PublishAsync(chatChannel, json);

			AddChatMessage(text, isSelf: true);
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

		private class ChatMessage
		{
			public string Sender { get; set; }
			public string Message { get; set; }
		}
	}
}