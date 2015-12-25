using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ETA.Shared
{
	public interface IPlatformServices
	{
		/// <summary>
		/// Shows a progress indicator.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="cancel"></param>
		/// <returns>TRUE if user cancels, otherwise FALSE if view gets dismissed via DismissProgressIndicator()</returns>
		Task<bool> ShowProgressIndicatorAsync(string msg, string cancel);

		/// <summary>
		/// Hides the progress indicator with cancelling.
		/// </summary>
		void DismissProgressIndicator();
	}
}
