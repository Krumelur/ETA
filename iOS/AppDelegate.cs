using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using UIKit;

namespace ETA.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();

			// Code for starting up the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
			#endif

			var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", "etadata");
			if (!Directory.Exists(databasePath))
			{
				Directory.CreateDirectory(databasePath);
			}

			Console.WriteLine($"iOS database location: {databasePath}");
			var fullPath = Path.Combine(databasePath, "data.sqlite");

			LoadApplication(new App(fullPath, new PlatformSpecific()));

			/*
			UITabBar.Appearance.BarTintColor = UIColor.Black;
			UITabBar.Appearance.SelectedImageTintColor = UIColor.White;
			UINavigationBar.Appearance.BarTintColor = UIColor.Black;
			UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White });
			*/

			return base.FinishedLaunching (app, options);
		}
	}
}

