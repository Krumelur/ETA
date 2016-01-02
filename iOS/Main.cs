using UIKit;
using EtaShared;

namespace ETA.iOS
{
	public class Application
	{
		static void Main (string[] args)
		{
			Xamarin.Insights.Initialize (InsightsKey.ApiKey);
			UIApplication.Main (args, null, typeof(AppDelegate));
		}
	}
}
