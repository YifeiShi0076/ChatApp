using System;
using System.Printing;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using StackExchange.Redis;
using ChatApp.Services;

namespace RedisChatApp
{
	public partial class MainWindow : Window
	{
		private RedisChatService chatService;
		private string currentChannel;
		private string clientId;

		public MainWindow(RedisChatService redisService, string clientId)
		{
			InitializeComponent();
			this.chatService = redisService;
			this.clientId = clientId;

			chatService.OnMessageReceived += HandleIncomingMessage;

		}

		private async void SubscribeButton_Click(object sender, RoutedEventArgs e)
		{
			
			currentChannel = ChannelTextBox.Text.Trim();

			string result = await chatService.SubscribeAsync(currentChannel);
			ClientListBox.Items.Add($"[{clientId}] {result}");
		}

		private async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(currentChannel))
			{
				MessageBox.Show("请先连接频道。");
				return;
			}

			string text = MessageTextBox.Text.Trim();
			if (string.IsNullOrWhiteSpace(text)) return;

			await chatService.SendMessageAsync(currentChannel, text);
			AddChatMessage(text, isSelf: true);
			MessageTextBox.Clear();
		}

		private void HandleIncomingMessage(string senderId, string message)
		{
			AddChatMessage($"[{senderId}]: {message}", isSelf: false);
		}

		private void AddChatMessage(string message, bool isSelf)
		{
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(() => AddChatMessage(message, isSelf));
				return;
			}
			var paragraph = new Paragraph(new Run(message))
			{
				TextAlignment = isSelf ? TextAlignment.Right : TextAlignment.Left
			};
			// if (isSelf)
			//{
			//	// Console.WriteLine($"[{DateTime.Now}] {message}");
			//	Console.WriteLine(paragraph.TextAlignment);
			//}
			string text = string.Empty;
			foreach (var inline in paragraph.Inlines)
			{
				if (inline is Run run) // 检查 Inline 是否是 Run 类型
				{
					text += run.Text; // 累加 Run 的文本内容
				}
			}
			Console.WriteLine(text);
			//ChatTextBox.AppendText(text);
			ChatTextBox.Document.Blocks.Add(paragraph);
			ChatTextBox.ScrollToEnd();
		}
	}
}