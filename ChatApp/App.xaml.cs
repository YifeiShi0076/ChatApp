﻿using RedisChatApp;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var startWindow = new ConnectWindow();
			startWindow.Show();
		}
	}

}
