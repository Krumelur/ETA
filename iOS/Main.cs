using UIKit;

namespace ETA.iOS
{
	public class Application
	{
		static void Main (string[] args)
		{
			Xamarin.Insights.Initialize (global::ETA.iOS.XamarinInsights.ApiKey);
			UIApplication.Main (args, null, typeof(AppDelegate));
		}
	}
}
