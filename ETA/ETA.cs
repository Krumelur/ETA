using System;
using ETA.Shared;
using Xamarin.Forms;

namespace ETA
{
	public class App : Application
	{
		public App ()
		{
			EtaManager.InitDefaultDependencies();

			EtaManager.Instance.Config = new EtaConfig("192.168.178.35", 8080);
			var supplies = EtaManager.Instance.GetSuppliesAsync().Result;

			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Label {
							XAlign = TextAlignment.Center,
							Text = supplies.ToString()
						}
					}
				}
			};
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

