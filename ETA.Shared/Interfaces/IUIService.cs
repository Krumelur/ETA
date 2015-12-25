using System.Threading;
using System.Threading.Tasks;

namespace ETA.Shared
{
	public interface IUIService
	{
		Task ShowMessageAsync(string msg, string confirmButton);
		Task ShowBusyIndicatorAsync(string msg, string cancel, CancellationTokenSource cts = default(CancellationTokenSource));
		void HideBusyIndicator();
	}
}
