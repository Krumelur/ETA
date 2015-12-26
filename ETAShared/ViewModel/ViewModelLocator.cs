/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:EtaShared"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace EtaShared
{
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary>
	public class ViewModelLocator
	{
		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator (string databasePath, IUIService uiService, IPlatformServices platformServices, INavigationService navigationService)
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			// Support classes.
			SimpleIoc.Default.Register<IUIService>(() => uiService);
			SimpleIoc.Default.Register<IPlatformServices>(() => platformServices);
			SimpleIoc.Default.Register<INavigationService>(() => navigationService);
			SimpleIoc.Default.Register<IEtaWebApi, EtaWebApi>();
			SimpleIoc.Default.Register<ILogger, DebugLogger>();
			SimpleIoc.Default.Register<IStorage, DatabaseStorage>();
			// Factory to create ISupplyData items.
			Func<ISupplyData> supplyDataCreator = () => new SupplyData();
			SimpleIoc.Default.Register<Func<ISupplyData>>(() => supplyDataCreator);
			SimpleIoc.Default.Register<EtaManager>();

			var dbStorage = (DatabaseStorage)SimpleIoc.Default.GetInstance<IStorage>();
			dbStorage.DatabasePath = databasePath;

			// View Models.
			SimpleIoc.Default.Register<SuppliesViewModel> ();
			SimpleIoc.Default.Register<SettingsViewModel>();
		}

		

		public SuppliesViewModel Supplies
		{
			get {
				return ServiceLocator.Current.GetInstance<SuppliesViewModel> ();
			}
		}

		public SettingsViewModel Settings
		{
			get
			{
				return ServiceLocator.Current.GetInstance<SettingsViewModel>();
			}
		}

		public static void Cleanup ()
		{
			// TODO Clear the ViewModels
		}
	}
}