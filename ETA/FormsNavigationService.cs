using System;
using System.Linq;
using EtaShared;
using GalaSoft.MvvmLight.Views;
using Xamarin.Forms;

namespace ETA
{
	public class FormsNavigationService : INavigationService
	{
		public string CurrentPageKey
		{
			get
			{
				return this.currentPage.ToString();
			}
			set
			{
				 Enum.TryParse<NavigationTarget>(value, out this.currentPage);
			}
		}
		NavigationTarget currentPage;

		public void GoBack()
		{
			throw new NotImplementedException();
		}

		public void NavigateTo(string pageKey)
		{
			this.CurrentPageKey = pageKey;
			var tabPage = (TabbedPage)Application.Current.MainPage;
			switch (this.currentPage)
			{
				case NavigationTarget.SuppliesInfo:
					tabPage.SelectedItem = tabPage.Children[0];
					break;

				case NavigationTarget.Statistics:
					tabPage.SelectedItem = tabPage.Children[1];
					break;

				case NavigationTarget.Messages:
					tabPage.SelectedItem = tabPage.Children[2];
					break;

				case NavigationTarget.Settings:
					tabPage.SelectedItem = tabPage.Children[3];
					break;
			}
		}

		public void NavigateTo(string pageKey, object parameter)
		{
			throw new NotImplementedException();
		}
	}
}
