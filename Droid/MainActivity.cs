using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.IO;
using Xamarin.Forms.Platform.Android;
using EtaShared;
using Xamarin.Forms;

namespace ETA.Droid
{
	[Activity (Label = "ETA Check", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	{
		public void Remind (DateTime dateTime, string title, string message)
		{

//			Intent alarmIntent = new Intent(Forms.Context, typeof(AlarmReceiver));
//			alarmIntent.PutExtra ("message", message);
//			alarmIntent.PutExtra ("title", title);
//
//			PendingIntent pendingIntent = PendingIntent.GetBroadcast(Forms.Context, 0, alarmIntent, PendingIntentFlags.UpdateCurrent);
//			AlarmManager alarmManager = (AlarmManager) Forms.Context.GetSystemService(Context.AlarmService);
//
//			var delay = dateTime - DateTime.Now;
//			alarmManager.Set(AlarmType.RtcWakeup, SystemClock.ElapsedRealtime () + delay.Milliseconds, pendingIntent);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Insights.Initialize (InsightsKey.ApiKey, this);

			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

			global::Xamarin.Forms.Forms.Init (this, bundle);

			FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;
			FormsAppCompatActivity.TabLayoutResource = Resource.Layout.tabs;

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

			this.Remind(DateTime.Now + TimeSpan.FromSeconds(10), "Hello", "World");
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
			ViewModelLocator.Cleanup();
		}
	}
}

