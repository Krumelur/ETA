using GalaSoft.MvvmLight;

namespace ETA.Shared
{
	public class BaseViewModel : ViewModelBase
	{
		public BaseViewModel (EtaManager manager) : base()
		{
			// TODO: Read config from...well "Config" :-)
			manager.Config = new EtaConfig("192.168.178.35", 8080);
			this.Manager = manager;
		}

		protected EtaManager Manager
		{
			get;
		}
	}
}