using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace EtaShared
{
	/// <summary>
	/// Statistics.
	/// </summary>
	public class StatisticsViewModel : BaseViewModel
	{
		public StatisticsViewModel(EtaManager manager, IUIService uiService, IStorage storage) : base(manager, uiService, storage)
		{
			var formattedChoices = new List<string>
			{
				Localize("7Days"),
				Localize("2Weeks"),
				Localize("1Month"),
				Localize("2Months"),
				Localize("3Months"),
				Localize("4Months"),
				Localize("5Months"),
				Localize("6Months"),
				Localize("12Months"),
				Localize("AllData")
			};

			var now = DateTime.Now;
			this.timeSpanChoices = new List<DateTime>
			{
				now.AddDays(-7),
				now.AddDays(-14),
				now.AddMonths(-1),
				now.AddMonths(-2),
				now.AddMonths(-3),
				now.AddMonths(-4),
				now.AddMonths(-5),
				now.AddMonths(-6),
				now.AddMonths(-12),
				DateTime.MinValue
			};

			this.FormattedTimeSpanChoices = formattedChoices;
			this.TimeSpanSelectedIndex = 2;
		}

		List<DateTime> timeSpanChoices;
		public IList<string> FormattedTimeSpanChoices { get; }

		public ICommand UpdateStatisticsCommand => new RelayCommand(async () => {
			await this.UpdateStatisticsAsync();
		});

		public int TimeSpanSelectedIndex
		{
			get
			{
				return this.timeSpanSelectedIndex;
			}
			set
			{
				if (value != this.timeSpanSelectedIndex)
				{
					this.timeSpanSelectedIndex = value;
					this.RaisePropertyChanged();
				}
			}
		}
		int timeSpanSelectedIndex;

		public string FormattedAveragePerDay
		{
			get
			{
				return this.formattedAveragePerDay;
			}
			set
			{
				if (value != this.formattedAveragePerDay)
				{
					this.formattedAveragePerDay = value;
					this.RaisePropertyChanged();
				}
			}
		}
		string formattedAveragePerDay;

		public string FormattedConsumption
		{
			get
			{
				return this.formattedConsumption;
			}
			set
			{
				if (value != this.formattedConsumption)
				{
					this.formattedConsumption = value;
					this.RaisePropertyChanged();
				}
			}
		}
		string formattedConsumption;

		public string FormattedDateOutOfSupplies
		{
			get
			{
				return this.dateOutOfSuppliesFormatted;
			}
			set
			{
				if (value != this.dateOutOfSuppliesFormatted)
				{
					this.dateOutOfSuppliesFormatted = value;
					this.RaisePropertyChanged();
				}
			}
		}
		string dateOutOfSuppliesFormatted; 


		/// <summary>
		/// Initializes the settings from the storage.
		/// </summary>
		/// <returns></returns>
		public override async Task InitializeAsync()
		{
			this.ShowBusyIndicator(Localize("UpdatingStatistics"));
			await base.InitializeAsync();
			this.HideBusyIndicator();
		}

		public async Task UpdateStatisticsAsync()
		{
			var token = this.ShowBusyIndicator(Localize("UpdatingStatistics"), Localize("Cancel"));

			var start = this.timeSpanChoices[this.TimeSpanSelectedIndex];

			var supplies = await this.Manager.GetSuppliesInRangeAsync(start, DateTime.Now);
			var averageConsumptionPerDay = await this.Manager.GetAverageConsumptionPerDayAsync(supplies);

			if (averageConsumptionPerDay == NumericUnit.Empty)
			{
				this.FormattedAveragePerDay = Localize("NotEnoughData");
			}
			else
			{
				this.FormattedAveragePerDay = $"{averageConsumptionPerDay.Value:N0}{averageConsumptionPerDay.Unit}";
			}

			var consumption = await this.Manager.GetTotalConsumptionAsync(supplies);
			if (consumption == NumericUnit.Empty)
			{
				this.FormattedConsumption = Localize("NotEnoughData");
			}
			else
			{
				this.FormattedConsumption = $"{consumption.Value:N0}{consumption.Unit}";
			} 

			// Get from server.
			var currentSupplies = await this.Manager.GetSuppliesAsync(token);

			if (currentSupplies != NumericUnit.Empty && averageConsumptionPerDay.Value > 0)
			{
				var daysLeft = currentSupplies / averageConsumptionPerDay.Value;
				var outOfSupplies = DateTime.Now.AddDays(daysLeft);
				this.FormattedDateOutOfSupplies = $"{outOfSupplies:D}";
			}
			else
			{
				this.FormattedDateOutOfSupplies = Localize("NoCalculationPossible"); 
			}

			this.HideBusyIndicator();
		}
	}
}