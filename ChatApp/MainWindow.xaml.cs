using System;
using System.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using StackExchange.Redis;

namespace RedisChatApp
{
	public partial class MainWindow : Window
	{
		private const string RedisConnectionString = "localhost";
		private const string ChatChannel = "chat_channel";
		private const string StatusChannel = "status_channel";

		private readonly string ClientId = Guid.NewGuid().ToString();
		private ConnectionMultiplexer _redis;
		private ISubscriber _subscriber;

		public MainWindow()
		{
			InitializeComponent();

			// 初始化 Redis 连接并订阅频道
			InitializeRedis();
		}

		private async void InitializeRedis()
		{
			try
			{
				_redis = await ConnectionMultiplexer.ConnectAsync(RedisConnectionString);
				_subscriber = _redis.GetSubscriber();

				// 订阅聊天频道
				await _subscriber.SubscribeAsync(ChatChannel, (channel, message) =>
				{
					var msg = (string)message;
					var parts = msg.Split(new[] { ':' }, 2);
					if (parts.Length == 2)
					{
						var senderId = parts[0];
						var content = parts[1];
						if (senderId != ClientId)
						{
							Dispatcher.Invoke(() =>
							{
								ChatBox.AppendText($"对方：{content}\n");
								ChatBox.ScrollToEnd();
							});
						}
					}
				});

				// 订阅状态频道
				await _subscriber.SubscribeAsync(StatusChannel, (channel, message) =>
				{
					var msg = (string)message;
					var parts = msg.Split(new[] { ':' }, 2);
					if (parts.Length == 2)
					{
						var senderId = parts[0];
						var status = parts[1];
						if (senderId != ClientId)
						{
							Dispatcher.Invoke(() =>
							{
								if (status == "online")
								{
									ChatBox.AppendText($"对方上线\n");
								}
								else if (status == "offline")
								{
									ChatBox.AppendText($"对方下线\n");
								}
								ChatBox.ScrollToEnd();
							});
						}
					}
				});

				// 发布上线通知
				await _subscriber.PublishAsync(StatusChannel, $"{ClientId}:online");
				ChatBox.AppendText("你已上线\n");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Redis 连接失败: {ex.Message}");
			}
		}

		private async void SendMessage_Click(object sender, RoutedEventArgs e)
		{
			var message = InputBox.Text.Trim();
			if (!string.IsNullOrEmpty(message))
			{
				// 发布聊天消息
				await _subscriber.PublishAsync(ChatChannel, $"{ClientId}:{message}");

				// 显示自己的消息
				ChatBox.AppendText($"你：{message}\n");
				ChatBox.ScrollToEnd();
				InputBox.Clear();
			}
		}

		protected override async void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			// 发布下线通知
			if (_subscriber != null)
			{
				await _subscriber.PublishAsync(StatusChannel, $"{ClientId}:offline");
			}

			// 关闭 Redis 连接
			if (_redis != null)
			{
				await _redis.CloseAsync();
				_redis.Dispose();
			}

			Environment.Exit(0); // 退出应用程序
		}
	}
}