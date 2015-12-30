using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System;
using ETA.Resources;

namespace EtaShared
{
	public class BaseViewModel : ViewModelBase
	{
		public BaseViewModel (EtaManager manager, IUIService uiService, IStorage storage) : base()
		{
			this.Manager = manager;
			this.uiService = uiService;
			this.storage = storage;
		}

		protected IStorage storage;

		public async virtual Task InitializeAsync()
		{
			var hostName = await this.storage.GetConfigValueAsync(SettingsViewModel.SettingServerUrl, null);
			this.Manager.Config = new EtaConfig(hostName);
			if (this.Manager.Config.Host.Contains("demo"))
			{
				this.Manager.EnableDemoMode();
			}
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

		protected Task<bool> ShowMessageAsync(string msg, string confirm, string cancel)
		{
			return this.uiService.ShowMessageAsync(msg, confirm, cancel);
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

		protected static string Localize(string constant)
		{
			try
			{
			return Strings.ResourceManager.GetString(constant);
			}
			catch
			{
				return constant;
			}
		}
	}
}