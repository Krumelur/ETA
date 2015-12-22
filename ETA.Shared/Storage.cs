using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SQLite;

namespace ETA.Shared
{
	public class Storage : IStorage
	{
		[Table("ConfigSettings")]
		class ConfigItem
		{
			[PrimaryKey, AutoIncrement]
			public int Id { get; set; }
			public string Key { get; set; }
			public string Value { get; set; }
		}

		public Storage(ILogger logger)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(this.DatabasePath));
			this.logger = logger;
			this.connection = new SQLiteAsyncConnection(this.DatabasePath);
		}

		ILogger logger;

		/// <summary>
		/// Defines the path and filename of the database.
		/// </summary>
		public string DatabasePath { get; set; }

		async Task InitDatabase()
		{
			if (this.initChecked)
			{
				return;
			}

			this.logger?.Log($"Database is located at: {this.DatabasePath}");

			await this.connection.CreateTablesAsync(CreateFlags.None, typeof(ConfigItem), typeof(SupplyData)).ConfigureAwait(false);

			this.initChecked = true;
		}
		bool initChecked;

		readonly SQLiteAsyncConnection connection;

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

		/// <summary>
		/// Internal helper to get a ConfigItem object from the DB.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>ConfigItem or NULL if key is not found</returns>
		async Task<ConfigItem> GetConfigItemAsync(string key)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(key));
			key = key.ToLower();
			// FindAsync() queries and returns FirstOrDefault (compare to GetAsync() which throws if nothing is found).
			var item = await this.connection.FindAsync<ConfigItem>(c => c.Key == key).ConfigureAwait(false);
			return item;
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

			var existingItem = await this.GetConfigItemAsync(key).ConfigureAwait(false);

			if (value == null)
			{
				if (existingItem != null)
				{
					await this.connection.DeleteAsync(existingItem).ConfigureAwait(false);
				}
				return;
			}

			if (existingItem == null)
			{
				existingItem = new ConfigItem
				{
					Key = key,
					Value = value
				};
			}

			await this.connection.InsertOrReplaceAsync(existingItem);
		}

		public Task SaveSupplyDataAsync(SupplyData supplyData)
		{
			return this.connection.InsertOrReplaceAsync(supplyData);
		}

		public async Task<SupplyData> GetSupplyDataAsync(int id)
		{
			var supplyData = await this.connection.FindAsync<SupplyData>(id).ConfigureAwait(false);
			return supplyData;
		}

		public async Task<List<SupplyData>> GetSupplyDataAsync(Func<SupplyData, bool> filter)
		{
			var result = await this.connection.Table<SupplyData>().Where(d => filter(d)).ToListAsync().ConfigureAwait(false);
			return result;
		}
	}
}
