using Xamarin.Forms;
using EtaShared;

namespace ETA
{
	public partial class SuppliesStatusPage : ContentPage
	{
		ViewModelLocator locator;

		public SuppliesStatusPage()
		{
			this.InitializeComponent();
			this.locator = (ViewModelLocator)Application.Current.Resources["Locator"];
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			await this.locator.Supplies.InitializeAsync();
			await this.locator.Supplies.UpdateSuppliesAsync();
		}
	}
}
