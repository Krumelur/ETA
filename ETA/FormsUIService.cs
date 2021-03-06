﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EtaShared;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;

namespace ETA
{
	internal class FormsUIService : IUIService
	{
		public FormsUIService()
		{
		}

		public CancellationToken ShowBusyIndicator(string msg, string cancel)
		{
			Application.Current.MainPage.IsBusy = true;
			var platform = SimpleIoc.Default.GetInstance<IPlatformSpecific>();
			var token = platform.ShowProgressIndicator(msg, cancel);
			return token;
		}

		public void HideBusyIndicator()
		{
			Application.Current.MainPage.IsBusy = false;
			var platform = SimpleIoc.Default.GetInstance<IPlatformSpecific>();
			platform.DismissProgressIndicator();
		}

		public Task ShowMessageAsync(string msg, string confirmButton)
		{
			return Application.Current.MainPage.DisplayAlert(string.Empty, msg, null, confirmButton);
		}

		public Task<bool> ShowMessageAsync(string msg, string confirmButton, string cancelButton)
		{
			return Application.Current.MainPage.DisplayAlert(string.Empty, msg, confirmButton, cancelButton);
		}


	}
}