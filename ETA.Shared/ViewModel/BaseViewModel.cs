using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace ETA.Shared
{
	public class BaseViewModel : ViewModelBase
	{
		public BaseViewModel (EtaManager manager, IUIService uiService) : base()
		{
			// TODO: Read config from...well "Config" :-)
			manager.Config = new EtaConfig("192.168.178.35", 8080);
			this.Manager = manager;
			this.uiService = uiService;
		}

		protected CancellationTokenSource cts;

		protected EtaManager Manager
		{
			get;
		}

		IUIService uiService;

		protected Task ShowMessageAsync(string msg, string confirm)
		{
			return this.uiService.ShowMessageAsync(msg, confirm);
		}

		protected bool IsBusy
		{
			get
			{
				return this.isBusy;
			}
			set
			{
				if (value != this.isBusy)
				{
					this.isBusy = value;
					this.uiService.ShowBusyIndicatorAsync("Test1", "Abbrechen", this.cts);
				}
			}
		}
		bool isBusy;
	}
}