using System;
using Xamarin.Forms;

namespace ETA
{
	public class OffsetConverter : IValueConverter
	{
		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double ret = 0f;
			try
			{
				var d = System.Convert.ToDouble (value);
				var offset = System.Convert.ToDouble (parameter);
				ret = d + offset;
			}
			catch
			{
				// Ignore...
			}
			return ret;
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException ();
		}
	}
}

