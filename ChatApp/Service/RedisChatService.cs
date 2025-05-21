using StackExchange.Redis;
using System.Text.Json;
using ChatApp.Models;

namespace ChatApp.Services
{
	public class RedisChatService
	{
		private ConnectionMultiplexer redis;
		private ISubscriber subscriber;
		private string clientId;
		// 已订阅的频道记录
		private HashSet<string> subscribedChannels = new HashSet<string>();
		public event Action<string, string> OnMessageReceived;

		public RedisChatService(string cilentId)
		{
			this.clientId = cilentId; 
		}

		public async Task<bool> ConnectAsync(string ip, string port)
		{
			try
			{
				redis = await ConnectionMultiplexer.ConnectAsync($"{ip}:{port}");
				subscriber = redis.GetSubscriber();
				return true;
			}
			catch
			{
				return false;
			}
		}
		public async Task<string> SubscribeAsync(string channelName)
		{
			if (subscribedChannels.Contains(channelName))
			{
				return "你已经订阅该频道，请勿重复订阅。";
			}

			await subscriber.SubscribeAsync(channelName, (channel, message) =>
			{
				var data = JsonSerializer.Deserialize<ChatMessage>(message);
				if (data?.Sender != clientId)
				{
					OnMessageReceived?.Invoke(data.Sender, data.Message);
				}
			});

			subscribedChannels.Add(channelName);
			return $"连接成功 - 频道: {channelName}";
		}

		public async Task SendMessageAsync(string channelName, string message)
		{
			var msg = new ChatMessage { Sender = clientId, Message = message };
			string json = JsonSerializer.Serialize(msg);
			await subscriber.PublishAsync(channelName, json);
		}
	}
}
