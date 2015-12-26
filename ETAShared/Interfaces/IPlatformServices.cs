using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EtaShared
{
	public interface IPlatformServices
	{
		/// <summary>
		/// Shows a progress indicator.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="cancel"></param>
		/// <returns>a token</returns>
		CancellationToken ShowProgressIndicator(string msg, string cancel);

		/// <summary>
		/// Hides the progress indicator with cancelling.
		/// </summary>
		void DismissProgressIndicator();
	}
}
