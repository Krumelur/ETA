using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Linq;

namespace EtaShared
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
		public EtaManager(IEtaWebApi webApi, ILogger logger, IStorage storage, Func<ISupplyData> supplyDataCreator)
		{
			this.webApi = webApi;
			this.Logger = logger;
			this.storage = storage;
			this.supplyDataCreator = supplyDataCreator;
		}

		IEtaWebApi webApi;
		readonly IStorage storage;
		readonly Func<ISupplyData> supplyDataCreator;

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
			
			string apiVersion = null;

			try
			{
				string xmlResponse = await this.webApi.GetApiVersionXmlAsync(token).ConfigureAwait(false);
				apiVersion = this.GetContentElement(xmlResponse, "api").Attribute("version").Value;
			}
			catch (Exception ex)
			{
				this.Logger?.Log(ex);
			}

			return apiVersion;
		}

		public async Task<ISupplyData> GetLatestCachedSupplyAsync()
		{
			var cachedSupplies = await this.storage.GetLatestSupplyDataAsync();
			return cachedSupplies;
		}

		public async Task<NumericUnit> GetSuppliesAsync(CancellationToken token = default(CancellationToken))
		{
			if (!this.CheckInitialization())
			{
				return NumericUnit.Empty;
			}
			
			NumericUnit amount = NumericUnit.Empty;

			try
			{
				string xmlResponse = await this.webApi.GetSuppliesXmlAsync(token).ConfigureAwait(false);

				var value = Convert.ToDouble(this.GetContentElement(xmlResponse, "value").Value);
				var divider = Convert.ToDouble(this.GetContentElement(xmlResponse, "value").Attribute("scaleFactor").Value);
				var unit= this.GetContentElement(xmlResponse, "value").Attribute("unit").Value;

				amount = new NumericUnit(value / divider, unit);
			}
			catch (Exception ex)
			{
				this.Logger?.Log(ex);
			}

			if (amount != NumericUnit.Empty)
			{
				// Check if there is a value that's younger than 12 hours. If yes, update it. 
				// If not, create a new entry.
				var halfDayAgo = DateTime.Now - TimeSpan.FromHours(12);
				var existingEntries = await this.storage.GetSupplyDataAsync(x => x.TimeStamp >= halfDayAgo).ConfigureAwait(false);
				ISupplyData data = existingEntries?.FirstOrDefault();
				if (data == null)
				{
					this.Logger?.Log("No existing value found - creating new supply data entry.");
					data = this?.supplyDataCreator();
				}
				// Store retrieved value in local DB.
				if (data != null)
				{
					data.Amount = amount.Value;
					data.Unit = amount.Unit;
					data.TimeStamp = DateTime.Now;

					await this.storage.SaveSupplyDataAsync(data);
				}
			}

			return amount;
		}

		public async Task<NumericUnit> GetSuppliesWarningLevelAsync(CancellationToken token = default(CancellationToken))
		{
			if (!this.CheckInitialization())
			{
				return NumericUnit.Empty;
			}

			NumericUnit amount = NumericUnit.Empty;

			try
			{
				string xmlResponse = await this.webApi.GetSuppliesWarningLevelXml(token).ConfigureAwait(false);

				var value = Convert.ToDouble(this.GetContentElement(xmlResponse, "value").Value);
				var divider = Convert.ToDouble(this.GetContentElement(xmlResponse, "value").Attribute("scaleFactor").Value);
				var unit = this.GetContentElement(xmlResponse, "value").Attribute("unit").Value;

				amount = new NumericUnit(value / divider, unit);
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

			NumericUnit amount = NumericUnit.Empty;

			try
			{
				string xmlResponse = await this.webApi.GetTotalConsumptionXmlAsync(token).ConfigureAwait(false);

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

			List<EtaError> errors = new List<EtaError>();

			try
			{
				string xmlResponse = await this.webApi.GetErrorsXmlAsync(token).ConfigureAwait(false);

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

				return errors;
			}
			catch (Exception ex)
			{
				this.Logger?.Log(ex);
			}

			return null;
		}

		public async Task<IList<ISupplyData>> GetSuppliesInRangeAsync(DateTime start, DateTime end)
		{
			// Generate some test data in the DB to check performance.
#if DEBUG
			/*
			var rand = new Random();
			double amount = 0f;
			DateTime date = DateTime.Now;
			while (amount <= 3500f)
			{
				var item = new SupplyData
				{
					Id = -1,
					Amount = amount,
					TimeStamp = date,
					Unit = "kg"
				};
				amount += rand.Next(10, 60);
				date = date - TimeSpan.FromHours(rand.Next(22, 55));
				await this.storage.SaveSupplyDataAsync(item);
			}
			*/
#endif
		
			var supplies = await this.storage.GetSupplyDataAsync(data => data.TimeStamp >= start && data.TimeStamp <= end);
			return supplies;
		}

		public async Task<NumericUnit> GetAverageConsumptionPerDayAsync(IList<ISupplyData> supplies)
		{
			var averagesPerDay = new List<double>();
			await Task.Run(() =>
			{
				// Order ascending by date (start with oldest).
				var orderedSupplies = supplies.OrderBy(d => d.TimeStamp).ToList();
				for (int i = 1; i < orderedSupplies.Count; i++)
				{
					var previousData = orderedSupplies[i - 1];
					var currentData = orderedSupplies[i];

					// Check if suppplies have been restocked.
					if (currentData.Amount > previousData.Amount)
					{
						// Skip refill.
						this.Logger?.Log($"Average calculation: storage refilled - skipping {currentData}");
					}
					else
					{
						var deltaAmount = previousData.Amount - currentData.Amount;
						var deltaTime = (currentData.TimeStamp - previousData.TimeStamp).TotalDays;

						var averagePerDay = deltaAmount / deltaTime;

						averagesPerDay.Add(averagePerDay);
					}
				}
			});

			if (averagesPerDay.Count >= 1)
			{
				var totalAveragePerDay = averagesPerDay.Average();
				return new NumericUnit(totalAveragePerDay, "kg");
			}

			return NumericUnit.Empty;
		}

		public async Task<NumericUnit> GetTotalConsumptionAsync(IList<ISupplyData> supplies)
		{
			if (supplies == null || supplies.Count <= 0)
			{
				return NumericUnit.Empty;
			}

			double totalAmount = 0f;
			await Task.Run(() =>
			{
				// Order ascending by date (start with oldest).
				var orderedSupplies = supplies.OrderBy(d => d.TimeStamp).ToList();
				for (int i = 1; i < orderedSupplies.Count; i++)
				{
					var previousData = orderedSupplies[i - 1];
					var currentData = orderedSupplies[i];

					// Check if suppplies have been restocked.
					if (currentData.Amount > previousData.Amount)
					{
						// Skip refill.
						this.Logger?.Log($"Comsumption calculation: storage refilled - skipping {currentData}");
					}
					else
					{
						var deltaAmount = previousData.Amount - currentData.Amount;
						totalAmount += deltaAmount;
					}
				}
			}).ConfigureAwait(false);

			return totalAmount > 0 ? new NumericUnit(totalAmount, "kg") : NumericUnit.Empty;
		}
			
		public async Task DeleteSuppliesDataAsync()
		{
			await this.storage.DeleteSuppliesDataAsync().ConfigureAwait(false);
		}
	}
}

