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
		public SuppliesViewModel (EtaManager manager, IUIService uiService, IStorage storage, INavigationService navigationService) : base (manager, uiService, storage)
		{
			this.navigationService = navigationService;
		}

		INavigationService navigationService;

	
		public ICommand UpdateSuppliesInfoCommand
		{
			get
			{
				return new RelayCommand(async () => {
					this.IsExecutingCommand = true;
					await this.UpdateSuppliesAsync();
					this.IsExecutingCommand = false;
				}, () => !this.IsExecutingCommand);
			}
		}
		public bool IsExecutingCommand { get; private set; }

		public async Task UpdateSuppliesAsync()
		{
			var token = this.ShowBusyIndicator("Aktualisieren", "Abbrechen");

			// Check if we have a server to talk to.
			bool isServerConfigured = await this.storage.GetConfigValueAsync(SettingsViewModel.SettingServerUrl, null) != null;

			if (!isServerConfigured)
			{
				this.HideBusyIndicator();
				await this.uiService.ShowMessageAsync("Bitte konfigurieren Sie erst die Verbindung zum ETA Heizkessel in den Einstellungen.", "OK");
				this.navigationService.NavigateTo(NavigationTarget.Settings.ToString());
				return;
			}

			// Get current supplies in storage.
			this.currentSupplies = await this.Manager.GetSuppliesAsync(token);
			this.suppliesUpdatedOn = DateTime.Now;
			if (this.currentSupplies == NumericUnit.Empty)
			{
				// Failed to retrieve value. Use last known.
				var latestSupply = await this.Manager.GetLatestCachedSupplyAsync ();
				if (latestSupply != null)
				{
					this.currentSupplies = new NumericUnit (latestSupply.Amount, latestSupply.Unit);
					this.suppliesUpdatedOn = latestSupply.TimeStamp;
				}
			}

			// Update the current warning level setting.
			this.suppliesWarningLevel = new NumericUnit(Convert.ToDouble(await this.storage.GetConfigValueAsync(SettingsViewModel.SettingStorageWarnLevel, "1000")), "kg");
			// maxLevel gets represented in the UI by SuppliesFillReferenceValue (=the height of the view).
			this.maxSuppliesLevel = new NumericUnit(Convert.ToDouble(await this.storage.GetConfigValueAsync(SettingsViewModel.SettingStorageCapacity, "5000")), "kg");

			this.SuppliesFillPercentage = this.currentSupplies / this.maxSuppliesLevel;

			this.RaisePropertyChanged(nameof(SuppliesDisplayValue));
			this.RaisePropertyChanged(nameof(IsBelowWarnLevel));
			this.RaisePropertyChanged (nameof (FormattedSuppliesUpdatedOn));
			this.HideBusyIndicator();
		}

		NumericUnit currentSupplies;
		NumericUnit suppliesWarningLevel;
		NumericUnit maxSuppliesLevel;
		DateTime suppliesUpdatedOn;

		public string FormattedSuppliesUpdatedOn => $"{this.suppliesUpdatedOn:D}";

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
					this.RaisePropertyChanged(nameof(IsBelowWarnLevel));
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
					this.RaisePropertyChanged(nameof(IsBelowWarnLevel));
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
				return this.SuppliesFillReferenceValue - this.SuppliesFillPercentage * this.SuppliesFillReferenceValue;
			}
		}

		public bool IsBelowWarnLevel
		{
			get
			{
				return this.currentSupplies <= this.suppliesWarningLevel;
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