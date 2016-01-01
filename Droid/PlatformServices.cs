using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EtaShared;
using AndroidHUD;

namespace ETA.Droid
{
	public class PlatformSpecific : IPlatformSpecific
	{
		CancellationTokenSource cts;

		public void DismissProgressIndicator()
		{
			if (AndHUD.Shared.CurrentDialog == null)
			{
				return;
			}
			AndHUD.Shared.Dismiss();
		}

		public CancellationToken ShowProgressIndicator(string msg, string cancel)
		{
			if(this.cts == null)
			{
				this.cts = new CancellationTokenSource();
			}

			if (AndHUD.Shared.CurrentDialog  != null)
			{
				return this.cts.Token;
			}

			if (string.IsNullOrWhiteSpace(cancel))
			{
				AndHUD.Shared.Show(Xamarin.Forms.Forms.Context, msg, -1, MaskType.Black);
			}
			else
			{
				AndHUD.Shared.Show(Xamarin.Forms.Forms.Context, msg, -1, MaskType.Black, default(TimeSpan?), null, true, () => {
					this.cts.Cancel();
					this.cts = null;
				});
			}
			return this.cts.Token;
		}
	}
}
