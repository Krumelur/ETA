using System.Threading;
using System.Threading.Tasks;

namespace ETA.Shared
{
	/// <summary>
	/// Defines all methods to communicate with the ETA web service.
	/// This is low level communication without any error handling.
	/// </summary>
	public interface IEtaWebApi
	{
		/// <summary>
		/// Sets the host URL (no trailing slash expected).
		/// </summary>
		/// <param name="connectionAddress"></param>
		void SetHostUrl(string connectionAddress);

		/// <summary>
		/// Gets the API version of the webservice.
		/// </summary>
		/// <description>
		/// Located at: /user/api
		/// Example return value:
		/// 	<?xml version="1.0" encoding="utf-8"?>
		/// 	<eta version="1.0" xmlns="http://www.eta.co.at/rest/v1">
		/// 		<api version="1.1"/>
		/// 	</eta>
		/// </description>
		/// <returns>The API version.</returns>
		Task<string> GetApiVersionXmlAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// Gets the remaining supplies.
		/// </summary>
		/// <description>>
		/// Located at: /user/var/112/10201/0/0/12015
		/// Example return value:
		/// <?xml version="1.0" encoding="utf-8"?>
		/// <eta version="1.0" xmlns="http://www.eta.co.at/rest/v1">
		/// 	<value uri="/user/var/112/10201/0/0/12015" strValue="908" unit="kg" decPlaces="0" scaleFactor="10" advTextOffset="0">9076</value>
		/// </eta>
		/// </description>
		/// <returns>The stock content.</returns>
		Task<string> GetSuppliesXmlAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// Gets the stock content warning level.
		/// </summary>
		/// <description>>
		/// Located at: /user/var/112/10201/0/0/12042
		/// Example return value:
		/// <?xml version="1.0" encoding="utf-8"?>
		/// <eta version="1.0" xmlns="http://www.eta.co.at/rest/v1">
		/// 	<value uri="/user/var/112/10201/0/0/12042" strValue="0" unit="kg" decPlaces="0" scaleFactor="10" advTextOffset="0">0</value>
		/// </eta>
		/// </description>
		/// <returns>The stock warning level.</returns>
		Task<string> GetSuppliesWarningLevelXml(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// Gets the total consumption.
		/// </summary>
		/// <description>>
		/// Located at: /user/var/112/10021/0/0/12016
		/// Example return value:
		/// <?xml version="1.0" encoding="utf-8"?>
		/// <eta version="1.0" xmlns="http://www.eta.co.at/rest/v1">
		/// 	 <value uri="/user/var/112/10021/0/0/12016" strValue="23744" unit="kg" decPlaces="0" scaleFactor="10" advTextOffset="0">237439</value>
		/// </eta>
		/// </description>
		/// <returns>The total consumption.</returns>
		Task<string> GetTotalConsumptionXmlAsync(CancellationToken token = default(CancellationToken));

		/// <summary>
		/// Gets active errors.
		/// </summary>
		/// <description>>
		/// Located at: /user/errors
		/// Example return value:
		/// <?xml version="1.0" encoding="utf-8"?>
		/// <eta version="1.0" xmlns="http://www.eta.co.at/rest/v1">
		/// 	<errors uri="/user/errors">
		///			<fub uri="/112/10021" name="Kessel">
		///				<error msg="Flue gas sensor Interrupted" priority="Error" time="2011-06-29 12:47:50">Sensor or Cable broken or badly connected</error>
		///				<error msg="Water pressure too low 0,00 bar" priority="Error" time="2011-06-29 12:48:12">Top up heating water! If this warning occurs more than once a year, please contact plumber.</error>
		///			</fub>
		///			<fub uri="/112/10101" name="HK1"/>
		///		</errors>
		///</eta>
		/// </description>
		/// <returns>Errors</returns>
		Task<string> GetErrorsXmlAsync(CancellationToken token = default(CancellationToken));
	}
}
