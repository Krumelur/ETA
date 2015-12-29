using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtaShared
{
	/// <summary>
	/// Messages.
	/// </summary>
	public class MessagesViewModel : BaseViewModel
	{
		public MessagesViewModel(EtaManager manager, IUIService uiService, IStorage storage) : base(manager, uiService, storage)
		{
		}

		public override async Task InitializeAsync ()
		{
			this.ShowBusyIndicator("Initialisierung");
			await base.InitializeAsync();
			this.HideBusyIndicator ();
		}

		public ICommand UpdateMessagesCommand => new RelayCommand(async () => {
			await this.UpdateMessagesAsync();
		});

		public IList<EtaError> Messages
		{
			get
			{ 
				return this.messages;
			}
			set
			{ 
				this.messages = value;
				this.RaisePropertyChanged ();
			}
		}
		IList<EtaError> messages;

		public async Task UpdateMessagesAsync()
		{
			var token = this.ShowBusyIndicator("Aktualisiere Meldungen", "Abbrechen");

			var errors =  await this.Manager.GetErrorsAsync (token);
			if (errors != null)
			{
				this.Messages = errors;
			}

			this.HideBusyIndicator();
		}
	}
}