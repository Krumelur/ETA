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
	}
}