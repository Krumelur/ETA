using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EtaShared
{
	public class DemoWebApi : IEtaWebApi
	{
		public async Task<string> GetApiVersionXmlAsync(CancellationToken token = default(CancellationToken))
		{
			await Task.Delay(1000);
			return (
			@"<?xml version=""1.0"" encoding=""utf-8""?>" +
			@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
			@" <api version=""1.1""/>" +
			@"</eta>");
		}

		public async Task<string> GetErrorsXmlAsync(CancellationToken token = default(CancellationToken))
		{
			await Task.Delay(1000);
			return (
			@"<?xml version=""1.0"" encoding=""utf-8""?>" +
			@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
			@"<errors uri=""/user/errors"">" +
			@"<fub uri=""/112/10021"" name=""Kessel"">" +
			@"<error msg=""Flue gas sensor Interrupted"" priority=""Error"" time=""2011-06-29 12:47:50"">Sensor or Cable broken or badly connected</error>" +
			@"<error msg=""Water pressure too low 0,00 bar"" priority=""Error"" time=""2011-06-29 12:48:12"">Top up heating water! If this warning occurs more than once a year, please contact plumber.</error>" +
			@"<error msg=""Erinnerung Aschebox leeren 1000 kg"" priority=""Warnung"" time=""2015-12-21 07:00:00"">Die Verschlüsse an der Aschebox öffnen und diese vom Kessel abziehen und entleeren. Der Zählerstand [Verbrauch seit Aschebox leeren] wird beim Abnehmen der Aschebox automatisch auf Null zurückgesetzt.</error>" +
			@"</fub>" +
			@"<fub uri=""/112/10101"" name=""HK1""/>" +
			@"</errors>" +
			@"</eta>");
		}

		public async Task<string> GetSuppliesWarningLevelXml(CancellationToken token = default(CancellationToken))
		{
			await Task.Delay(1000);
			return (
			@"<?xml version=""1.0"" encoding=""utf-8""?>" +
			@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
			@"<value uri=""/user/var/112/10201/0/0/12015"" strValue=""500"" unit=""kg"" decPlaces=""0"" scaleFactor=""1"" advTextOffset=""0"">500</value>" +
			@"</eta>");
		}

		public async Task<string> GetSuppliesXmlAsync(CancellationToken token = default(CancellationToken))
		{
			await Task.Delay(1000);
			return (
			@"<?xml version=""1.0"" encoding=""utf-8""?>" +
			@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
			@"<value uri = ""/user/var/112/10201/0/0/12015"" strValue = ""908"" unit = ""kg"" decPlaces = ""0"" scaleFactor = ""10"" advTextOffset = ""0"" >9076</value >" +
			@"</eta>");
		}

		public async Task<string> GetTotalConsumptionXmlAsync(CancellationToken token = default(CancellationToken))
		{
			await Task.Delay(1000);
			return (
			@"<?xml version=""1.0"" encoding=""utf-8""?>" +
			@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
			@"<value uri=""/user/var/112/10201/0/0/12015"" strValue=""15432"" unit=""kg"" decPlaces=""0"" scaleFactor=""1"" advTextOffset=""0"" >15432></value>" +
			@"</eta>");
		}

		public void SetHostUrl(string connectionAddress)
		{
		}
	}
}
