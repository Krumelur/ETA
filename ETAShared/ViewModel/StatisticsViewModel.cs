using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

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
				"7 Tage",
				"2 Wochen",
				"1 Monat",
				"2 Monate",
				"3 Monate",
				"4 Monate",
				"5 Monate",
				"6 Monate",
				"12 Monate",
				"Alles"
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


		/// <summary>
		/// Initializes the settings from the storage.
		/// </summary>
		/// <returns></returns>
		public override async Task InitializeAsync()
		{
			this.ShowBusyIndicator("Aktualisiere Statisiken");
			await base.InitializeAsync();
			this.HideBusyIndicator();
		}
	}
}