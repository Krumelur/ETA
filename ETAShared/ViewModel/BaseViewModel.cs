using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace EtaShared
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

		protected EtaManager Manager
		{
			get;
		}

		protected IUIService uiService;

		protected Task ShowMessageAsync(string msg, string confirm)
		{
			return this.uiService.ShowMessageAsync(msg, confirm);
		}

		protected void ShowBusyIndicator(string msg)
		{
			this.ShowBusyIndicator(msg, null);
		}

		protected CancellationToken ShowBusyIndicator(string msg, string cancel)
		{
			return this.uiService.ShowBusyIndicator(msg, cancel);
		}

		protected void HideBusyIndicator()
		{
			this.uiService.HideBusyIndicator();
		}
	}
}