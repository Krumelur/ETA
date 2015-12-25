using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ETA.Shared;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms;

namespace ETA
{
	internal class FormsUIService : IUIService
	{
		public FormsUIService()
		{
		}

		public async Task ShowBusyIndicatorAsync(string msg, string cancel, CancellationTokenSource cts = default(CancellationTokenSource))
		{
			Debug.Assert(!Application.Current.MainPage.IsBusy);

			Application.Current.MainPage.IsBusy = true;
			var platform = SimpleIoc.Default.GetInstance<IPlatformServices>();
			bool cancelled = await platform.ShowProgressIndicatorAsync(msg, cancel);
			if (cancelled)
			{
				cts.Cancel();
			}
		}

		public void HideBusyIndicator()
		{
			Debug.Assert(Application.Current.MainPage.IsBusy);

			Application.Current.MainPage.IsBusy = false;
			var platform = SimpleIoc.Default.GetInstance<IPlatformServices>();
			platform.DismissProgressIndicator();
		}

		public Task ShowMessageAsync(string msg, string confirmButton)
		{
			return Application.Current.MainPage.DisplayAlert(string.Empty, msg, confirmButton, null);
		}
	}
}