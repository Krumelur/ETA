/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ETA.Shared"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ETA.Shared
{
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary>
	public class ViewModelLocator
	{
		/// <summary>
		/// Path to local database. Must be set by the platforms.
		/// </summary>
		public static string DatabasePath { get; set; }

		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator ()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			// Support classes.
			SimpleIoc.Default.Register<IEtaWebApi, EtaWebApi>();
			SimpleIoc.Default.Register<ILogger, DebugLogger>();
			SimpleIoc.Default.Register<IStorage, DatabaseStorage>();
			// Factory to create ISupplyData items.
			Func<ISupplyData> supplyDataCreator = () => new SupplyData();
			SimpleIoc.Default.Register<Func<ISupplyData>>(() => supplyDataCreator);
			SimpleIoc.Default.Register<EtaManager>();

			var dbStorage = (DatabaseStorage)SimpleIoc.Default.GetInstance<IStorage>();
			dbStorage.DatabasePath = ViewModelLocator.DatabasePath;

			// View Models.
			SimpleIoc.Default.Register<SuppliesViewModel> ();
		}

		

		public SuppliesViewModel Supplies
		{
			get {
				return ServiceLocator.Current.GetInstance<SuppliesViewModel> ();
			}
		}

		public static void Cleanup ()
		{
			// TODO Clear the ViewModels
		}
	}
}