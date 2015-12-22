using ETA.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace ETA
{

	public partial class App : Application
	{
		public App ()
		{
			this.InitializeComponent();

			var suppliesPage = new SuppliesStatusPage();

			var tabbedPage = new TabbedPage();
			tabbedPage.Children.Add(new NavigationPage(suppliesPage) { Title = suppliesPage.Title, Icon = suppliesPage.Icon });

			this.MainPage = tabbedPage;
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

