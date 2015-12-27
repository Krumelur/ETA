using GalaSoft.MvvmLight;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Views;

namespace EtaShared
{
	/// <summary>
	/// View model to show information about pellets supplies.
	/// </summary>
	public class SuppliesViewModel : BaseViewModel
	{
		public SuppliesViewModel (EtaManager manager, IUIService uiService, IStorage storage, INavigationService navigationService) : base (manager, uiService)
		{
			this.storage = storage;
			this.navigationService = navigationService;
		}

		INavigationService navigationService;
		IStorage storage;

	
		public ICommand UpdateSuppliesInfoCommand
		{
			get
			{
				return new RelayCommand(async () => {
					this.IsExecutingCommand = true;
					var cts = new CancellationTokenSource();
					await this.UpdateSuppliesAsync(cts.Token);
					this.IsExecutingCommand = false;
				}, () => !this.IsExecutingCommand);
			}
		}
		public bool IsExecutingCommand { get; private set; }

		async Task UpdateSuppliesAsync(CancellationToken token)
		{
			// Check if we have a server to talk to.
			bool isServerConfigured = await this.storage.GetConfigValueAsync(SettingsViewModel.SettingServerUrl, null) != null;

			if (!isServerConfigured)
			{
				await this.uiService.ShowMessageAsync("Bitte konfigurieren Sie erst die Verbindung zum ETA Heizkessel in den Einstellungen.", "OK");
				this.navigationService.NavigateTo(NavigationTarget.Settings.ToString());
				return;
			}

			// Get current supplies in storage.
			this.currentSupplies = await this.Manager.GetSuppliesAsync(token);

			// Update the current warning level setting.
			this.suppliesWarningLevel = new NumericUnit(Convert.ToDouble(await this.storage.GetConfigValueAsync(SettingsViewModel.SettingStorageWarnLevel, "1000")), "kg");
			// maxLevel gets represented in the UI by SuppliesFillReferenceValue (=the height of the view).
			this.maxSuppliesLevel = new NumericUnit(Convert.ToDouble(await this.storage.GetConfigValueAsync(SettingsViewModel.SettingStorageCapacity, "5000")), "kg");

			this.SuppliesFillPercentage = this.currentSupplies / this.maxSuppliesLevel;

			this.RaisePropertyChanged(nameof(SuppliesDisplayValue));
		}

		NumericUnit currentSupplies;
		NumericUnit suppliesWarningLevel;
		NumericUnit maxSuppliesLevel;

		/// <summary>
		/// Defines the filling of the supplies storage in percent. Changing this value will also change SuppliesFillAbsoluteValue.
		/// </summary>
		public double SuppliesFillPercentage
		{
			get {
				return this.suppliesFillPercentage;
			}
			set {
				if (value != this.suppliesFillPercentage)
				{
					this.suppliesFillPercentage = value;
					this.RaisePropertyChanged ();
					this.RaisePropertyChanged(nameof(SuppliesFillAbsoluteValue));
				}
			}
		}
		double suppliesFillPercentage;

		/// <summary>
		/// Used by SuppliesFillAbsoluteValue in combination with SuppliesFillPercentage to calculate an absolute value of the supplies which can be used to display some sort of graphical representation in the UI.
		/// Changing this value will also change SuppliesFillAbsoluteValue.
		/// </summary>
		public double SuppliesFillReferenceValue
		{
			get {
				return this.suppliesFillReferenceValue;
			}
			set {
				if (value != this.SuppliesFillReferenceValue)
				{
					this.suppliesFillReferenceValue = value;
					this.RaisePropertyChanged ();
					this.RaisePropertyChanged(nameof(SuppliesFillAbsoluteValue));
				}
			}
		}
		double suppliesFillReferenceValue;

		/// <summary>
		/// Readonly value which represents the absolute amount of supplies based on a maximum of SuppliesFillReferenceValue.
		/// </summary>
		public double SuppliesFillAbsoluteValue
		{
			get {
				return this.SuppliesFillPercentage * this.SuppliesFillReferenceValue;
			}
		}

		public string SuppliesDisplayValue
		{
			get
			{
				return $"{this.currentSupplies.Value}{this.currentSupplies.Unit}";
			}
		}

	}
}