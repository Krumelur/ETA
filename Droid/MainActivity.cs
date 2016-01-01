using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;

namespace ETA.Droid
{
	[Activity (Label = "ETA.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

			// Code for starting up the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start ();
			#endif

			var databasePath = Path.Combine (System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal));
			if (!Directory.Exists (databasePath))
			{
				Directory.CreateDirectory (databasePath);
			}

			Console.WriteLine ($"Android database location: {databasePath}");
			var fullPath = Path.Combine (databasePath, "data.sqlite");

			var formsApp = new App (fullPath, new PlatformSpecific ());
			LoadApplication (formsApp);
		}
	}
}

