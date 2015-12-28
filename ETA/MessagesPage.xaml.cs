using EtaShared;
using Xamarin.Forms;

namespace ETA
{
	public partial class MessagesPage : ContentPage
	{
		public MessagesPage ()
		{
			InitializeComponent ();
			this.vm = (MessagesViewModel)this.BindingContext;
		}

		MessagesViewModel vm;

		protected async override void OnAppearing()
		{
			base.OnAppearing();
			await this.vm.InitializeAsync();if (!this.hasUpdated)
			{
				await this.vm.UpdateMessagesAsync ();
				this.hasUpdated = true;
			}
		}
		bool hasUpdated;
	}
}

