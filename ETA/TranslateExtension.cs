using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Globalization;
using System.Resources;
using ETA.Resources;

namespace ETA
{
	[ContentProperty ("Text")]
	public class TranslateExtension : IMarkupExtension
	{
		//readonly CultureInfo ci;

		public TranslateExtension() {
			//ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo ();
		}

		public string Text { get; set; }

		public object ProvideValue (IServiceProvider serviceProvider)
		{
			if (Text == null)
				return "";

			ResourceManager resmgr = Strings.ResourceManager;

			//var translation = resmgr.GetString (Text, ci);
			var translation = resmgr.GetString (Text);

			if (translation == null) {
				translation = Text;
				/*
				#if DEBUG
				throw new InvalidOperationException (
					String.Format ("Key '{0}' was not found in resources'.", Text));
				#else
				translation = Text; // HACK: returns the key, which GETS DISPLAYED TO THE USER
				#endif
				*/
			}
			return translation;
		}
	}
}

