using Xamarin.Forms;
using EtaShared;

namespace ETA
{
	public partial class SettingsPage : ContentPage
	{
		ViewModelLocator locator;

		public SettingsPage()
		{
			this.InitializeComponent();
			this.locator = (ViewModelLocator)Application.Current.Resources["Locator"];
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			await this.locator.Settings.InitializeAsync();
		}

		protected async override void OnDisappearing()
		{
			base.OnDisappearing();
			await this.locator.Settings.SaveAsync();
		}
	}
}
