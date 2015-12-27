using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BigTed;
using EtaShared;

namespace ETA.iOS
{
	public class PlatformServices : IPlatformServices
	{
		CancellationTokenSource cts;

		public void DismissProgressIndicator()
		{
			if (!BTProgressHUD.IsVisible)
			{
				return;
			}
			BTProgressHUD.Dismiss();
		}

		public CancellationToken ShowProgressIndicator(string msg, string cancel)
		{
			if(this.cts == null)
			{
				this.cts = new CancellationTokenSource();
			}

			if (BTProgressHUD.IsVisible)
			{
				return this.cts.Token;
			}

			if (string.IsNullOrWhiteSpace(cancel))
			{
				BTProgressHUD.Show(msg, -1, ProgressHUD.MaskType.Black);
			}
			else
			{
				BTProgressHUD.Show(cancel, () => {
					this.cts.Cancel();
					this.cts = null;
				}, msg, -1, ProgressHUD.MaskType.Black);
			}
			return this.cts.Token;
		}
	}
}
