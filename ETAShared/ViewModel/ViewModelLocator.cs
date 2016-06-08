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
		public ViewModelLocator (string databasePath, IUIService uiService, IPlatformSpecific platformSpecific, INavigationService navigationService)
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			// Support classes.
			SimpleIoc.Default.Register<IUIService>(() => uiService);
			SimpleIoc.Default.Register<IPlatformSpecific>(() => platformSpecific);
			SimpleIoc.Default.Register<INavigationService>(() => navigationService);
			SimpleIoc.Default.Register<IEtaWebApi, EtaWebApi>();
			SimpleIoc.Default.Register<ILogger, DebugLogger>();
			SimpleIoc.Default.Register<IStorage, AzureStorage>();
			// Factory to create ISupplyData items.
			Func<ISupplyData> supplyDataCreator = () => new SupplyData();
			SimpleIoc.Default.Register<Func<ISupplyData>>(() => supplyDataCreator);
			SimpleIoc.Default.Register<EtaManager>();

			var dbStorage = (AzureStorage)SimpleIoc.Default.GetInstance<IStorage>();
			dbStorage.DatabasePath = databasePath;

			// View Models.
			SimpleIoc.Default.Register<SuppliesViewModel> ();
			SimpleIoc.Default.Register<SettingsViewModel>();
			SimpleIoc.Default.Register<StatisticsViewModel>();
			SimpleIoc.Default.Register<MessagesViewModel>();
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

		public StatisticsViewModel Statistics
		{
			get
			{
				return ServiceLocator.Current.GetInstance<StatisticsViewModel>();
			}
		}

		public MessagesViewModel Messages
		{
			get
			{
				return ServiceLocator.Current.GetInstance<MessagesViewModel>();
			}
		}

		public EtaManager Manager
		{
			get
			{ 
				return ServiceLocator.Current.GetInstance<EtaManager> ();
			}
		}

		public static void Cleanup ()
		{
			// TODO Clear the ViewModels
			SimpleIoc.Default.Reset();
		}
	}
}