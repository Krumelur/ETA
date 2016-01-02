using System;
using Xamarin.Forms;
using EtaShared;
using Xamarin.Forms.Xaml;

namespace ETA
{
	public partial class SuppliesStatusPage : ContentPage
	{
		ViewModelLocator locator;

		public SuppliesStatusPage()
		{
			this.InitializeComponent();
			this.locator = (ViewModelLocator)Application.Current.Resources["Locator"];

			// Must manually adjust translation on Android. Seems like the anchor settings are not working the same on Android.
			this.locator.Supplies.PropertyChanged += (sender, e) => {
				var s = this.locator.Supplies;
				var propName = nameof(s.SuppliesFillAbsoluteValue);
				if(Device.OS == TargetPlatform.Android && e.PropertyName == propName)
				{
					this.fillStatusLayout.TranslationY = s.SuppliesFillAbsoluteValue - 50;
				}
			};
		}

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			await this.locator.Supplies.InitializeAsync();

			if((DateTime.Now - lastUpdate).TotalMinutes > 5)
			{
				await this.locator.Supplies.UpdateSuppliesAsync();
				lastUpdate = DateTime.Now;
			}
		}

		static DateTime lastUpdate = DateTime.MinValue;
	}
}
