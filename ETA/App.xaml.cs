using EtaShared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ETA
{
	public partial class App : Application
	{
		public App (string databasePath, IPlatformSpecific platformSpecific)
		{
			this.InitializeComponent();

			var uiService = new FormsUIService();
			var navigationService = new FormsNavigationService();

			// Make view model locator globally available as a resource.
			this.Resources.Add("Locator", new ViewModelLocator(databasePath, uiService, platformSpecific, navigationService));
			
			var tabbedPage = new TabbedPage();
			
			tabbedPage.Children.Add(this.WrapInNavPage(new SuppliesStatusPage()));
			tabbedPage.Children.Add(this.WrapInNavPage(new StatisticsPage()));
			tabbedPage.Children.Add(this.WrapInNavPage(new MessagesPage()));
			tabbedPage.Children.Add(this.WrapInNavPage(new SettingsPage()));

			this.MainPage = tabbedPage;
		}

		NavigationPage WrapInNavPage(ContentPage page)
		{
			var navBarBackgroundColor = (Color)this.Resources["NavBarBkColor"];
			var navBarTextColor = (Color)this.Resources["NavBarTextColor"];
			return new NavigationPage(page) {
				Title = page.Title,
				Icon = page.Icon,
				BarBackgroundColor = navBarBackgroundColor,
				BarTextColor = navBarTextColor
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

