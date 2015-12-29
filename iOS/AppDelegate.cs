using System;
using System.IO;
using Foundation;
using UIKit;
using System.Threading;

namespace ETA.iOS
{
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			// Code for starting up the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start ();
			#endif

			var databasePath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "..", "Library", "etadata");
			if (!Directory.Exists (databasePath))
			{
				Directory.CreateDirectory (databasePath);
			}

			Console.WriteLine ($"iOS database location: {databasePath}");
			var fullPath = Path.Combine (databasePath, "data.sqlite");

			this.formsApp = new App (fullPath, new PlatformSpecific ());
			LoadApplication (this.formsApp);

			/*
			UITabBar.Appearance.BarTintColor = UIColor.Black;
			UITabBar.Appearance.SelectedImageTintColor = UIColor.White;
			UINavigationBar.Appearance.BarTintColor = UIColor.Black;
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });
			*/

			// Run background fetch maximum once every 12 hours.
			app.SetMinimumBackgroundFetchInterval (TimeSpan.FromHours (15).TotalSeconds);

			var settings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Alert, new NSSet ());
			if (UIApplication.SharedApplication.CurrentUserNotificationSettings != settings)
			{
				UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);
			}

			return base.FinishedLaunching (app, options);
		}

		App formsApp;

		public async override void PerformFetch (UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
		{
			var cts = new CancellationTokenSource ();
			// We have maximum 30 seconds in background mode - cancel early.
			cts.CancelAfter (TimeSpan.FromSeconds (20));
			try
			{
				var errorMsg = await this.formsApp.RunBackgroundUpdate(cts.Token);

				if (errorMsg != null)
				{
					var lastNotification = (string)((NSString)NSUserDefaults.StandardUserDefaults.ValueForKey(new NSString("lastNotification")));
					if (lastNotification != errorMsg)
					{
						var notif = new UILocalNotification
						{
							AlertBody = errorMsg
						};
						UIApplication.SharedApplication.PresentLocalNotificationNow(notif);
					}
					NSUserDefaults.StandardUserDefaults.SetString(errorMsg, "lastNotification");
				}
				completionHandler(UIBackgroundFetchResult.NewData);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to execute in background: " + ex);
				completionHandler(UIBackgroundFetchResult.Failed);
			}
		}
	}
}

