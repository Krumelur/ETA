using GalaSoft.MvvmLight;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading;
using System.Collections.Generic;

namespace ETA.Shared
{
	/// <summary>
	/// Application settings.
	/// </summary>
	public class SettingsViewModel : BaseViewModel
	{
		public SettingsViewModel(EtaManager manager, IUIService uiService) : base(manager, uiService)
		{
			var choices = new List<string>();

			for (int i = 1; i <= 40; i++)
			{
				choices.Add($"{i * 500:N0}kg");
            }

			this.StorageChoices = choices;
			this.StorageCapacitySelectedIndex = 10;
			this.StorageWarningLevelSelectedIndex = 2;
			this.Url = string.Empty;
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

		public IList<string> StorageChoices { get; }

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
				this.cts = new CancellationTokenSource();
				return new RelayCommand(async () => {
					this.IsBusy = true;
					try
					{
						var version = await this.Manager.GetApiVersionAsync(this.cts.Token);
						if (string.IsNullOrWhiteSpace(version))
						{
							await this.ShowMessageAsync("Verbindungstest fehlgeschlagen. Bitte überprüfen Sie die Adressen und den Port.", "OK");
						}
						else
						{
							await this.ShowMessageAsync("Verbindung erfolgreich!", "OK");
						}
					}
					finally
					{
						this.IsBusy = false;
					}
				}, () => true);
			}
		}

		public ICommand RefreshWarningLevelCommand
		{
			get
			{
				this.cts = new CancellationTokenSource();
				return new RelayCommand(async () => {
				}, () => true);
			}
		}
		
	}
}