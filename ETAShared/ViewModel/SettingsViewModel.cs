using GalaSoft.MvvmLight;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace EtaShared
{
	/// <summary>
	/// Application settings.
	/// </summary>
	public class SettingsViewModel : BaseViewModel
	{
		public const string SettingServerUrl = "server_url";
		public const string SettingStorageWarnLevel = "storage_warn_level";
		public const string SettingStorageCapacity = "storage_capacity";

		public SettingsViewModel(EtaManager manager, IUIService uiService, IStorage storage) : base(manager, uiService)
		{
			var formattedChoices = new List<string>();
			var choices = new List<double>();

			for (int i = 1; i <= 40; i++)
			{
				choices.Add(i * 500);
				formattedChoices.Add($"{i * 500:N0}kg");
            }

			this.StorageChoices = choices;
			this.FormattedStorageChoices = formattedChoices;
			this.StorageCapacitySelectedIndex = 10;
			this.StorageWarningLevelSelectedIndex = 2;
			this.Url = string.Empty;

			this.storage = storage;
		}

		IStorage storage;

		/// <summary>
		/// Initializes the settings from the storage.
		/// </summary>
		/// <returns></returns>
		public async Task InitializeAsync()
		{
			this.ShowBusyIndicator("Lade Konfiguration");
			this.Url = await this.storage.GetConfigValueAsync(SettingServerUrl, string.Empty);
			var value = await this.storage.GetConfigValueAsync(SettingStorageWarnLevel, "1000");
			this.StorageWarnLevel = Convert.ToDouble(value);
			value = await this.storage.GetConfigValueAsync(SettingStorageCapacity, "5000");
			this.StorageCapacity = Convert.ToDouble(value);
			this.HideBusyIndicator();
		}

		/// <summary>
		/// Call this to save the current config. Either via a save button or when leaving the current page.
		/// </summary>
		/// <returns></returns>
		public async Task SaveAsync()
		{
			this.ShowBusyIndicator("Speichere Konfiguration");
			await this.storage.SetConfigValueAsync(SettingStorageWarnLevel, this.StorageChoices[this.StorageWarningLevelSelectedIndex].ToString());
			await this.storage.SetConfigValueAsync(SettingStorageCapacity, this.StorageChoices[this.StorageCapacitySelectedIndex].ToString());
			this.HideBusyIndicator();
		}


		/// <summary>
		/// Maximum capacity of storage.
		/// </summary>
		public double StorageCapacity
		{
			get
			{
				return this.storageCapacity;
			}
			set
			{
				if (value != this.storageCapacity)
				{
					this.storageCapacity = value;
					this.RaisePropertyChanged();
					this.StorageCapacitySelectedIndex = this.StorageChoices.IndexOf(value);
				}
			}
		}
		double storageCapacity;

		/// <summary>
		/// Warning level for storage content.
		/// </summary>
		public double StorageWarnLevel
		{
			get
			{
				return this.storageWarnLevel;
			}
			set
			{
				if (value != this.storageWarnLevel)
				{
					this.storageWarnLevel = value;
					this.RaisePropertyChanged();
					this.StorageWarningLevelSelectedIndex = this.StorageChoices.IndexOf(value);
				}
			}
		}
		double storageWarnLevel;

		/// <summary>
		/// URL or IP of ETA web API.
		/// </summary>
		public string Url
		{
			get
			{
				return this.url;
			}
			set
			{
				if (value != this.url)
				{
					this.url = value;
					this.RaisePropertyChanged();
				}
			}
		}
		string url;

		public IList<string> FormattedStorageChoices { get; }
		IList<double> StorageChoices { get; }


		public int StorageCapacitySelectedIndex
		{
			get
			{
				return this.storageCapacitySelectedIndex;
			}
			set
			{
				if (value != this.storageCapacitySelectedIndex)
				{
					this.storageCapacitySelectedIndex = value;
					this.RaisePropertyChanged();
				}
			}
		}
		int storageCapacitySelectedIndex;

		public int StorageWarningLevelSelectedIndex
		{
			get
			{
				return this.storageWarningLevelSelectedIndex;
			}
			set
			{
				if (value != this.storageWarningLevelSelectedIndex)
				{
					this.storageWarningLevelSelectedIndex = value;
					this.RaisePropertyChanged();
				}
			}
		}
		int storageWarningLevelSelectedIndex;

		public ICommand TestConnectionCommand
		{
			get
			{
				return new RelayCommand(async () => {
					var token = this.ShowBusyIndicator("Teste Verbinding", "Abbrechen");
					var currentConfig = this.Manager.Config;
					try
					{
						string hostName = this.Url.ToLowerInvariant();

						string protocol = "http://";
						if (hostName.StartsWith("http://"))
						{
							protocol = "http://";
							hostName = hostName.Replace("http://", "");
						}
						else if(hostName.StartsWith("https://"))
						{
							protocol = "https://";
							hostName = hostName.Replace("https://", "");
						}

						int port = 8080;
						if (hostName.Contains(":"))
						{
							hostName = hostName.Split(':')[0];
							port = Convert.ToInt32(hostName.Split(':')[1]);
						}
						// Update manager's config.
						this.Manager.Config = new EtaConfig(protocol + hostName, port);

						this.Manager.Logger?.Log($"Trying to connect to: {this.Manager.Config}");

						// Check connection
						var version = await this.Manager.GetApiVersionAsync(token);
						this.HideBusyIndicator();

						if (string.IsNullOrWhiteSpace(version))
						{
							// Reset config.
							this.Manager.Config = currentConfig;
							await this.ShowMessageAsync("Verbindungstest fehlgeschlagen. Bitte �berpr�fen Sie die Adresse und den Port.", "OK");
						}
						else
						{
							// Save config.
							await this.storage.SetConfigValueAsync(SettingServerUrl, this.Manager.Config.ConnectionAddress);
							await this.ShowMessageAsync("Verbindung erfolgreich! Neue Verbindungsdaten gespeichert.", "OK");
						}
					}
					catch (Exception ex)
					{
						this.Manager.Logger?.Log(ex);
					}
					finally
					{
						this.HideBusyIndicator();
					}
				}, () => true);
			}
		}

		public ICommand RefreshWarningLevelCommand
		{
			get
			{	
				return new RelayCommand( () => {
				}, () => true);
			}
		}
		
	}
}