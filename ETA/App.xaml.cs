using EtaShared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using System.Threading;

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
			this.locator = new ViewModelLocator(databasePath, uiService, platformSpecific, navigationService);
			this.Resources.Add("Locator", this.locator);
			
			var tabbedPage = new TabbedPage();
			
			tabbedPage.Children.Add(this.WrapInNavPage(new SuppliesStatusPage()));
			tabbedPage.Children.Add(this.WrapInNavPage(new StatisticsPage()));
			tabbedPage.Children.Add(this.WrapInNavPage(new MessagesPage()));
			tabbedPage.Children.Add(this.WrapInNavPage(new SettingsPage()));

			this.MainPage = tabbedPage;
		}

		ViewModelLocator locator;

		public async Task<string> RunBackgroundUpdate(CancellationToken token)
		{
			await this.locator.Manager.GetSuppliesAsync (token);
			var errors = await this.locator.Manager.GetErrorsAsync (token);
			if (errors != null && errors.Count > 0)
			{
				return errors [0].Message;
			}
			return null;
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

