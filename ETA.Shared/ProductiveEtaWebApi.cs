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
	public class ProductiveEtaWebApi : IEtaWebApi
	{
		/// <summary>
		/// Creates an instance.
		/// </summary>
		/// <param name="hostUrl">base URL of the host without trailing slash</param>
		/// <param name="logger"></param>
		public ProductiveEtaWebApi (string hostUrl, ILogger logger)
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

		public Task<string> GetApiVersionXmlAsync (CancellationToken token = default(CancellationToken))
		{
			return this.GetXmlResponseAsync(this.hostUrl + "/user/api", token);
		}

		public Task<string> GetStockContentXmlAsync (CancellationToken token = default(CancellationToken))
		{
			throw new NotImplementedException ();
		}

		public Task<string> GetStockContentWarningLevelXmlAsync(CancellationToken token = default(CancellationToken))
		{
			throw new NotImplementedException ();
		}

		public Task<string> GetTotalConsumptionXmlAsync(CancellationToken token = default(CancellationToken))
		{
			throw new NotImplementedException ();
		}

		public Task<string> GetErrorsXmlAsync(CancellationToken token = default(CancellationToken))
		{
			throw new NotImplementedException ();
		}
	}
}

