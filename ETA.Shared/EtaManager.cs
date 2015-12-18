using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Linq;

namespace ETA.Shared
{
	/// <summary>
	/// High level access to the ETA services.
	/// </summary>
	public sealed class EtaManager
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="webApi">implementation to use for server communication</param>
		public EtaManager(IEtaWebApi webApi, ILogger logger)
		{
			this.webApi = webApi;
			this.logger = logger;
		}
		IEtaWebApi webApi;
		ILogger logger;

		/// <summary>
		/// Configuration options.
		/// </summary>
		public EtaConfig Config { get; set; }
		
		/// <summary>
		/// Helper to check config before performing a call to the web API.
		/// </summary>
		/// <param name="caller"></param>
		/// <returns>true if initialization is correct</returns>
		bool CheckInitialization([CallerMemberName] string caller = null)
		{
			if (this.Config.ConnectionAddress != null)
			{
				return true;
			}
			else
			{
				this.logger?.Log($"[{caller}] failed due to uninitialized configuration - call Initialize() first.");
				return false;
			}
		}

		IList<XElement> GetContentElements(string xml, string elementName)
		{
			List<XElement> elements = null;
			try
			{
				var xmlDoc = XDocument.Parse(xml);
				var ns = xmlDoc.Root.GetDefaultNamespace();
				elements = xmlDoc.Root.Elements(ns + elementName).ToList();
			}
			catch (Exception ex)
			{
				this.logger?.Log(ex);
			}

			return elements;
		}

		XElement GetContentElement(string xml, string elementName)
		{
			var elements = this.GetContentElements(xml, elementName);
			return elements?.SingleOrDefault();
		}

		/// <summary>
		/// Checks if the host is available.
		/// </summary>
		/// <param name="token"></param>
		/// <returns>true, if the host can be reached.</returns>
		public async Task<bool> IsHostReachableAsync(CancellationToken token = default(CancellationToken))
		{
			return !string.IsNullOrWhiteSpace(await this.GetApiVersionAsync(token).ConfigureAwait(false));
		}

		public async Task<string> GetApiVersionAsync(CancellationToken token = default(CancellationToken))
		{
			if (!this.CheckInitialization())
			{
				return null;
			}

			string xmlResponse = await this.webApi.GetApiVersionXmlAsync(token).ConfigureAwait(false);
			string apiVersion = null;

			try
			{
				apiVersion = this.GetContentElement(xmlResponse, "api").Attribute("version").Value;
			}
			catch (Exception ex)
			{
				this.logger?.Log(ex);
			}

			return apiVersion;
		}

		public Task<NumericUnit> GetStockContentAsync(CancellationToken token = default(CancellationToken))
		{
			throw new NotImplementedException();
		}

		public Task<NumericUnit> GetStockContentWarningLevelAsync(CancellationToken token = default(CancellationToken))
		{
			throw new NotImplementedException();
		}

		public Task<NumericUnit> GetTotalConsumptionAsync(CancellationToken token = default(CancellationToken))
		{
			throw new NotImplementedException();
		}

		public Task<IList<EtaError>> GetErrorsAsync(CancellationToken token = default(CancellationToken))
		{
			throw new NotImplementedException();
		}
	}
}

