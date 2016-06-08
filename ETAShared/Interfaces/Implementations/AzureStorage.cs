using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json;

namespace EtaShared
{
	/// <summary>
	/// Implementation of IStorage that uses Azure	
	/// </summary>
	public class AzureStorage : IStorage
	{
		/// <summary>
		/// Entity to store configuration in the DB. This is never exposed directly.
		/// </summary>
		[DataTable("ConfigSettings")]
		[Preserve]
		class AzureConfigItem
		{
			public AzureConfigItem ()
			{
			}

			public string Id { get; set; }
			public string Key { get; set; }
			public string Value { get; set; }
		}

		/// <summary>
		/// Entity to store ISupplyData in the DB. This is never exposed directly, only the interface is used.
		/// </summary>
		[DataTable("SupplyData")]
		[Preserve]
		class AzureSupplyDataItem : ISupplyData
		{
			/// <summary>
			/// Empty c'tor is required by Azure Mobile Services.
			/// </summary>
			public AzureSupplyDataItem ()
			{
			}

			/// <summary>
			/// Copy constructor.
			/// </summary>
			/// <param name="item"></param>
			public AzureSupplyDataItem(ISupplyData item)
			{
				Debug.Assert(item != null);

				this.Amount = item.Amount;
				this.TimeStamp = item.TimeStamp;
				this.Unit = item.Unit;
			}

			public string Id { get; set; }

			public double Amount { get; set; }

			public DateTime TimeStamp { get; set; }

			public string Unit { get; set; }

			public override string ToString() => $"Id = {Id}, Amount = {Amount}, TimeStamp = {TimeStamp}, Unit = {Unit}";
		}

		public AzureStorage(ILogger logger)
		{
			this.logger = logger;
		}

		ILogger logger;

		/// <summary>
		/// Defines the path and filename of the database.
		/// </summary>
		public string DatabasePath { get; set; }

		MobileServiceClient mobileServiceClient;
		IMobileServiceSyncTable<AzureSupplyDataItem> suppliesTable;
		IMobileServiceSyncTable<AzureConfigItem> settingsTable;

		async Task InitDatabase()
		{
			if (this.initChecked)
			{
				return;
			}
			this.initChecked = true;

			var store = new MobileServiceSQLiteStore(this.DatabasePath);
			store.DefineTable<AzureSupplyDataItem>();
			store.DefineTable<AzureConfigItem>();
			this.mobileServiceClient = new MobileServiceClient("https://reneruppert.azurewebsites.net");
			await this.mobileServiceClient.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

			suppliesTable = this.mobileServiceClient.GetSyncTable<AzureSupplyDataItem>();
			settingsTable = this.mobileServiceClient.GetSyncTable<AzureConfigItem>();

			this.logger?.Log($"Database is located at: {this.DatabasePath}");
		}
		bool initChecked;

		/// <summary>
		/// Internal helper to get a ConfigItem object from the DB.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>ConfigItem or NULL if key is not found</returns>
		async Task<AzureConfigItem> GetConfigItemAsync(string key)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(key));
			key = key.ToLower();

			var item = (await this.settingsTable.Where (c => c.Key == key).ToListAsync ().ConfigureAwait (false)).FirstOrDefault ();
			return item;
		}

		public async Task<string> GetConfigValueAsync(string key, string defaultValue = null)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				this.logger?.Log($"Cannot get config value if no key is specified!");
				return defaultValue;
			}

			await this.InitDatabase();

			// FindAsync() queries and returns FirstOrDefault (compare to GetAsync() which throws if nothing is found).
			var item = await this.GetConfigItemAsync(key).ConfigureAwait(false);
			return item?.Value ?? defaultValue;
		}


		public async Task SetConfigValueAsync(string key, string value)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				this.logger?.Log($"Cannot save config value '{value}' if no key is specified!");
                return;
			}

			key = key.ToLower();

			await this.InitDatabase();

			// Get existing.
			var item = await this.GetConfigItemAsync(key).ConfigureAwait(false);

			// Delete.
			if (item != null)
			{
				await this.settingsTable.DeleteAsync (item).ConfigureAwait (false);
			}

			if (value == null)
			{
				// Bail out if there is no new value.
				return;
			}

			// Write back.
			item = new AzureConfigItem
			{
				Id = null,
				Key = key,
				Value = value
			};
			
			// Always insert, because we always delete it first.
			await this.settingsTable.InsertAsync(item).ConfigureAwait(false);
		}

		public async Task<ISupplyData> SaveSupplyDataAsync(ISupplyData supplyData)
		{
			if (supplyData == null)
			{
				this.logger?.Log("Nothing to save!");
				return null;
			}

			await this.InitDatabase();

			if (!(supplyData is AzureSupplyDataItem))
			{
				throw new InvalidOperationException ("Item to save must be of type " + nameof (AzureSupplyDataItem) + "!");
			}

			var item = supplyData as AzureSupplyDataItem;

			if (item.Id == null)
			{
				await this.suppliesTable.InsertAsync(item).ConfigureAwait(false);
			}
			else
			{
				await this.suppliesTable.UpdateAsync(item).ConfigureAwait(false);
			}

			await this.SyncSuppliesAsync ().ConfigureAwait (false);

			return item;
		}

		public async Task SyncSuppliesAsync()
		{
			await this.suppliesTable.PullAsync("allSupplies", suppliesTable.CreateQuery()).ConfigureAwait(false);
			await this.mobileServiceClient.SyncContext.PushAsync().ConfigureAwait(false);
		}

		public async Task<ISupplyData> GetSupplyDataAsync(int id)
		{
			throw new NotImplementedException ("Azure does not support integer IDs...don't know how to implement...");
		}

		public async Task<IList<ISupplyData>> GetSupplyDataAsync(Expression<Func<ISupplyData, bool>> filter)
		{
			var castExpression = Expression.Lambda<Func<AzureSupplyDataItem, bool>>(filter.Body, filter.Parameters);

			await this.InitDatabase();
			var result = await this.suppliesTable.Where(castExpression).ToListAsync();
			// The result is a list of "AzureSupplyItem" which cannot implicitly be cast to a list of "ISupplyData"
			// so we have to do this manually.
			var castResult = await Task.Run(() => result.ToList<ISupplyData>()).ConfigureAwait(false);
			return castResult;
		}

		public async Task DeleteSuppliesDataAsync()
		{
			await this.suppliesTable.PurgeAsync (true).ConfigureAwait(false);
		}

		public async Task<ISupplyData> GetLatestSupplyDataAsync()
		{
			var newestItem = (await this.suppliesTable.OrderByDescending (d => d.TimeStamp).Take (1).ToListAsync ()).FirstOrDefault ();
			return newestItem;
		}
	}
}
