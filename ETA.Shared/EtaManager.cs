using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ETA.Shared
{
	/// <summary>
	/// High level access to the ETA services.
	/// </summary>
	public sealed class EtaManager
	{
		public static void InitDefaultDependencies(ILogger customLogger = null)
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
			SimpleIoc.Default.Register<IEtaWebApi, EtaWebApi>();
			if (customLogger == null)
			{
				SimpleIoc.Default.Register<ILogger, DebugLogger>();
			}
			else
			{
				SimpleIoc.Default.Register<ILogger>(() => customLogger);
				customLogger.Log("Custom logger successfully registered!");
			}
			SimpleIoc.Default.Register<EtaManager>();
		}

		/// <summary>
		/// Gets the only instance of the manager.
		/// </summary>
		public static EtaManager Instance => SimpleIoc.Default.GetInstance<EtaManager>();

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
				this.logger?.Log($"[{caller}] failed due to uninitialized configuration - set the Config property.");
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
				this.logger?.Log(ex);
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
				this.logger?.Log(ex);
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
				this.logger?.Log(ex);
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
				this.logger?.Log(ex);
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
				this.logger?.Log(ex);
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

					var error = new EtaError(msg, desc, time);
					errors.Add(error);
				}
			}
			catch (Exception ex)
			{
				this.logger?.Log(ex);
			}

			return errors;
		}
	}
}

