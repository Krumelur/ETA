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
		/// <param name="logger">logger to use</param>
		public EtaManager(IEtaWebApi webApi, ILogger logger)
		{
			this.webApi = webApi;
			this.Logger = logger;
		}
		IEtaWebApi webApi;

		/// <summary>
		/// Used for log output.By default this logs to the debug console but you can set your own logger.
		/// </summary>
		public ILogger Logger
		{
			get; set;
		}

		/// <summary>
		/// Configuration options.
		/// </summary>
		public EtaConfig Config {
			get
			{
				return this.config;
			}
			set
			{
				this.config = value;
				this.webApi.SetHostUrl(this.config.ConnectionAddress);
			}
		}
		EtaConfig config;
		
		/// <summary>
		/// Helper to check config before performing a call to the web API.
		/// </summary>
		/// <param name="caller"></param>
		/// <returns>true if initialization is correct</returns>
		bool CheckInitialization([CallerMemberName] string caller = null)
		{
			if (this.Config?.ConnectionAddress != null)
			{
				return true;
			}
			else
			{
				this.Logger?.Log($"[{caller}] failed due to uninitialized configuration - set the Config property.");
				return false;
			}
		}

		IList<XElement> GetContentElements(string xml, string elementName, bool includeDescendants = false)
		{
			List<XElement> elements = null;
			try
			{
				var xmlDoc = XDocument.Parse(xml);
				var ns = xmlDoc.Root.GetDefaultNamespace();
				elements = includeDescendants
					? xmlDoc.Root.Descendants(ns + elementName).ToList()
					: xmlDoc.Root.Elements(ns + elementName).ToList();
			}
			catch (Exception ex)
			{
				this.Logger?.Log(ex);
			}

			return elements;
		}

		XElement GetContentElement(string xml, string elementName, bool includeDescendants = false)
		{
			var elements = this.GetContentElements(xml, elementName, includeDescendants);
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
				this.Logger?.Log(ex);
			}

			return apiVersion;
		}

		public async Task<NumericUnit> GetSuppliesAsync(CancellationToken token = default(CancellationToken))
		{
			if (!this.CheckInitialization())
			{
				return NumericUnit.Empty;
			}

			string xmlResponse = await this.webApi.GetSuppliesXmlAsync(token).ConfigureAwait(false);
			NumericUnit amount = NumericUnit.Empty;

			try
			{
				var value = this.GetContentElement(xmlResponse, "value").Attribute("strValue").Value;
				var unit= this.GetContentElement(xmlResponse, "value").Attribute("unit").Value;

				amount = new NumericUnit(Convert.ToDouble(value), unit);
			}
			catch (Exception ex)
			{
				this.Logger?.Log(ex);
			}

			return amount;
		}

		public async Task<NumericUnit> GetSuppliesWarningLevelAsync(CancellationToken token = default(CancellationToken))
		{
			if (!this.CheckInitialization())
			{
				return NumericUnit.Empty;
			}

			string xmlResponse = await this.webApi.GetSuppliesWarningLevelXml(token).ConfigureAwait(false);
			NumericUnit amount = NumericUnit.Empty;

			try
			{
				var value = this.GetContentElement(xmlResponse, "value").Attribute("strValue").Value;
				var unit = this.GetContentElement(xmlResponse, "value").Attribute("unit").Value;

				amount = new NumericUnit(Convert.ToDouble(value), unit);
			}
			catch (Exception ex)
			{
				this.Logger?.Log(ex);
			}

			return amount;
		}

		public async Task<NumericUnit> GetTotalConsumptionAsync(CancellationToken token = default(CancellationToken))
		{
			if (!this.CheckInitialization())
			{
				return NumericUnit.Empty;
			}

			string xmlResponse = await this.webApi.GetTotalConsumptionXmlAsync(token).ConfigureAwait(false);
			NumericUnit amount = NumericUnit.Empty;

			try
			{
				var value = this.GetContentElement(xmlResponse, "value").Attribute("strValue").Value;
				var unit = this.GetContentElement(xmlResponse, "value").Attribute("unit").Value;

				amount = new NumericUnit(Convert.ToDouble(value), unit);
			}
			catch (Exception ex)
			{
				this.Logger?.Log(ex);
			}

			return amount;
		}

		public async Task<IList<EtaError>> GetErrorsAsync(CancellationToken token = default(CancellationToken))
		{
			if (!this.CheckInitialization())
			{
				return null;
			}

			string xmlResponse = await this.webApi.GetErrorsXmlAsync(token).ConfigureAwait(false);

			List<EtaError> errors = new List<EtaError>();

			try
			{
				foreach (var errorEl in this.GetContentElements(xmlResponse, "error", true))
				{
					var msg = errorEl.Attribute("msg").Value;
					var time = Convert.ToDateTime(errorEl.Attribute("time").Value);
					var desc = errorEl.Value.ToString();
					var errorTypeRaw = errorEl.Attribute("priority").Value;
					var errorType = errorTypeRaw.ToLower().Contains("warn") ? EtaError.ERROR_TYPE.Warning : EtaError.ERROR_TYPE.Error;

					var error = new EtaError(msg, desc, time, errorType);
					errors.Add(error);
				}
			}
			catch (Exception ex)
			{
				this.Logger?.Log(ex);
			}

			return errors;
		}
	}
}

