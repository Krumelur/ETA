using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BigTed;
using ETA.Shared;

namespace ETA.iOS
{
	public class PlatformServices : IPlatformServices
	{
		bool isVisible;
		TaskCompletionSource<bool> tcs;

		public void DismissProgressIndicator()
		{
			BTProgressHUD.Dismiss();
			// Indicate that task was not cancelled.
			this.tcs.SetResult(false);
			this.isVisible = false;
		}

		public Task<bool> ShowProgressIndicatorAsync(string msg, string cancel)
		{
			if (this.isVisible)
			{
				return Task.FromResult(false);
			}

			this.isVisible = true;
			this.tcs = new TaskCompletionSource<bool>();
			BTProgressHUD.Show(cancel, () => tcs.SetResult(true), msg, -1, ProgressHUD.MaskType.Black);
			return tcs.Task;
		}
	}
}
