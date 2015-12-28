using System;
using System.Collections.Generic;

using Xamarin.Forms;
using EtaShared;

namespace ETA
{
	public partial class StatisticsPage : ContentPage
	{
		public StatisticsPage ()
		{
			InitializeComponent ();

			this.vm = (StatisticsViewModel)this.BindingContext;
			this.TimeSpanPicker.Unfocused += TimeSpanPicker_Unfocused;
		}

		StatisticsViewModel vm;

		protected async override void OnAppearing ()
		{
			base.OnAppearing ();
			await this.vm.InitializeAsync ();
			if (!this.hasUpdated)
			{
				await this.vm.UpdateStatisticsAsync ();
				this.hasUpdated = true;
			}
		}
		bool hasUpdated;

		private void TimeSpanPicker_Unfocused(object sender, FocusEventArgs e)
		{
			// Now this is hacky, but necessary. The Picker control fires can be data bound to the SelectedItem property.
			// However this property changes before the picker gets dismissed by the user via the "Done" button.
			vm.UpdateStatisticsAsync();
		}
	}
}

