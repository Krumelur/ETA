using System;
using System.Net.Http;
using System.Threading;
using ModernHttpClient;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ETA.Shared
{
	/// <summary>
	/// Implementation of IEtaWebApi for productive use. Communicates over the network with the host.
	/// </summary>
	public class EtaWebApi : IEtaWebApi
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="hostUrl">base URL of the host without trailing slash</param>
		/// <param name="logger"></param>
		public EtaWebApi (string hostUrl, ILogger logger)
		{
			Debug.Assert(!String.IsNullOrWhiteSpace(hostUrl));

			this.hostUrl = hostUrl;
			this.logger = logger;
		}

		string hostUrl;
		ILogger logger;

		Lazy<HttpClient> client = new Lazy<HttpClient>(() => new HttpClient(new NativeMessageHandler()) { Timeout = TimeSpan.FromSeconds(30) }, true);

		public HttpClient Client
		{
			get
			{
				return this.client.Value;
			}
		}

		/// <summary>
		/// Helper to get an XML response from the ETA web API.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		async Task<string> GetXmlResponseAsync(string url, CancellationToken token)
		{
			string xmlResponse = null;
			try
			{
				var response = await this.Client.GetAsync(url, token).ConfigureAwait(false);
				if (!response.IsSuccessStatusCode)
				{
					return null;
				}
				xmlResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			catch (TaskCanceledException)
			{
				this.logger?.Log("Cancelled getting XML response.");
			}
			return xmlResponse;
		}

		public async Task<string> GetApiVersionXmlAsync (CancellationToken token = default(CancellationToken))
		{
			var xml = await this.GetXmlResponseAsync(this.hostUrl + "/user/api", token).ConfigureAwait(false);
			this.logger?.Log($"'{nameof(GetApiVersionXmlAsync)}' received XML: {xml}");
			return xml;
		}

		public async Task<string> GetSuppliesXmlAsync (CancellationToken token = default(CancellationToken))
		{
			var xml = await this.GetXmlResponseAsync(this.hostUrl + "/user/var/112/10201/0/0/12015", token).ConfigureAwait(false);
			this.logger?.Log($"'{nameof(GetSuppliesXmlAsync)}' received XML: {xml}");
            return xml;
		}

		public async Task<string> GetSuppliesWarningLevelXml(CancellationToken token = default(CancellationToken))
		{
			var xml = await this.GetXmlResponseAsync(this.hostUrl + "/user/var/112/10201/0/0/12042", token).ConfigureAwait(false);
			this.logger?.Log($"'{nameof(GetSuppliesWarningLevelXml)}' received XML: {xml}");
			return xml;
		}

		public async Task<string> GetTotalConsumptionXmlAsync(CancellationToken token = default(CancellationToken))
		{
			var xml = await this.GetXmlResponseAsync(this.hostUrl + "/user/var/112/10021/0/0/12016", token).ConfigureAwait(false);
			this.logger?.Log($"'{nameof(GetTotalConsumptionXmlAsync)}' received XML: {xml}");
			return xml;
		}

		public async Task<string> GetErrorsXmlAsync(CancellationToken token = default(CancellationToken))
		{
			var xml = await this.GetXmlResponseAsync(this.hostUrl + "/user/errors", token).ConfigureAwait(false);
			this.logger?.Log($"'{nameof(GetErrorsXmlAsync)}' received XML: {xml}");
			return xml;
		}
	}
}

