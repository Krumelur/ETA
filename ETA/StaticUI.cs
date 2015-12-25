using Xamarin.Forms;

namespace ETA
{
	public static class StaticUI
	{
		/// <summary>
		/// Background color for percentage scale. Alpha values in XAML are broken in Forms 2.0. See https://bugzilla.xamarin.com/show_bug.cgi?id=37177
		/// #D5D18D
		/// </summary>
		public static Color PercentageLabelBackgroundColor = Color.FromRgba(0xd5, 0xd1, 0x8d, 200);
	}
}

