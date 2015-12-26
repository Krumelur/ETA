using System.Threading;
using System.Threading.Tasks;

namespace EtaShared
{
	public interface IUIService
	{
		/// <summary>
		/// Shows a message box.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="confirmButton"></param>
		/// <returns></returns>
		Task ShowMessageAsync(string msg, string confirmButton);

		/// <summary>
		/// Shows a busy indicator.
		/// </summary>
		/// <param name="msg">message to display</param>
		/// <param name="cancel">text for cancel button; if NULL, don't show a cancel button</param>
		/// <returns>a token</returns>
		CancellationToken ShowBusyIndicator(string msg, string cancel);

		/// <summary>
		/// Hides the busy indicator.
		/// </summary>
		void HideBusyIndicator();
	}
}
