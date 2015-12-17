using System;
using System.Net.Http;
using System.Threading;
using ModernHttpClient;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Linq;

namespace ETA.Shared
{
	public interface ILogger
	{
		void Log(string s);

		void Log(Exception ex);
	}

}

