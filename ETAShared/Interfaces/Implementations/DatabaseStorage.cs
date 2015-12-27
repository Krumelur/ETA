using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SQLite;

namespace EtaShared
{
	/// <summary>
	/// Implementation of IStorage that writes to a Sqlite DB (using Sqlite-Net).
	/// </summary>
	public class DatabaseStorage : IStorage
	{
		/// <summary>
		/// Entity to store configuration in the DB. This is never exposed directly.
		/// </summary>
		[Table("ConfigSettings")]
		[Preserve]
		class DatabaseConfigItem
		{
			[PrimaryKey, AutoIncrement]
			public int Id { get; set; }
			public string Key { get; set; }
			public string Value { get; set; }
		}

		/// <summary>
		/// Entity to store ISupplyData in the DB. This is never exposed directly, only the interface is used.
		/// Sqlite-Net requires entities to be non-abstract classes with a public constructor and they must have
		/// certain attributes. Since I don't want to depend on Sqlite-Net, the app is using the ISupplyData interface.
		/// The DatabaseStorage class internally uses this entity.
		/// </summary>
		[Table("SupplyData")]
		[Preserve]
		class DatabaseSupplyDataItem : ISupplyData
		{
			/// <summary>
			/// Required constructor for Sqlite-Net.
			/// </summary>
			public DatabaseSupplyDataItem()
			{
			}

			/// <summary>
			/// Copy constructor.
			/// </summary>
			/// <param name="item"></param>
			public DatabaseSupplyDataItem(ISupplyData item)
			{
				Debug.Assert(item != null);

				this.Id = item.Id;
				this.Amount = item.Amount;
				this.TimeStamp = item.TimeStamp;
				this.Unit = item.Unit;
			}

			[PrimaryKey, AutoIncrement]
			public int Id { get; set; }

			public double Amount { get; set; }

			public DateTime TimeStamp { get; set; }

			public string Unit { get; set; }

			public override string ToString() => $"Id = {Id}, Amount = {Amount}, TimeStamp = {TimeStamp}, Unit = {Unit}";
		}

		public DatabaseStorage(ILogger logger)
		{
			this.logger = logger;
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

			if (this.connection == null)
			{
				Debug.Assert(!string.IsNullOrWhiteSpace(this.DatabasePath));
				this.connection = new SQLiteAsyncConnection(this.DatabasePath);
			}

			this.logger?.Log($"Database is located at: {this.DatabasePath}");

			await this.connection.CreateTablesAsync(CreateFlags.None, typeof(DatabaseConfigItem), typeof(DatabaseSupplyDataItem)).ConfigureAwait(false);

			this.initChecked = true;
		}
		bool initChecked;

		SQLiteAsyncConnection connection;

		/// <summary>
		/// Internal helper to get a ConfigItem object from the DB.
		/// </summary>
		/// <param name="key"></param>
		/// <returns>ConfigItem or NULL if key is not found</returns>
		async Task<DatabaseConfigItem> GetConfigItemAsync(string key)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(key));
			key = key.ToLower();
			// FindAsync() queries and returns FirstOrDefault (compare to GetAsync() which throws if nothing is found).
			var item = await this.connection.FindAsync<DatabaseConfigItem>(c => c.Key == key).ConfigureAwait(false);
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
				await this.connection.DeleteAsync(item).ConfigureAwait(false);
			}

			if (value == null)
			{
				// Bail out if there is no new value.
				return;
			}

			// Write back.
			item = new DatabaseConfigItem
			{
				Id = -1,
				Key = key,
				Value = value
			};
			
			// Always insert, because we always delete it first.
			await this.connection.InsertAsync(item);
		}

		public async Task<ISupplyData> SaveSupplyDataAsync(ISupplyData supplyData)
		{
			if (supplyData == null)
			{
				this.logger?.Log("Nothing to save!");
				return null;
			}

			await this.InitDatabase();

			// Sqlite-Net requires non-abstract classes with a parameterless constructor,
			// so we have to create something with this criteria.
			var item = supplyData as DatabaseSupplyDataItem;
			if (item == null)
			{
				item = new DatabaseSupplyDataItem(supplyData);
			}

			if (item.Id <= 0)
			{
				// Sqlite-Net only generates a new key if the PK is < 0.
				item.Id = -1;
				await this.connection.InsertAsync(item).ConfigureAwait(false);
			}
			else
			{
				await this.connection.UpdateAsync(item).ConfigureAwait(false);
			}

			return item;
		}

		public async Task<ISupplyData> GetSupplyDataAsync(int id)
		{
			await this.InitDatabase();
			var supplyData = await this.connection.FindAsync<DatabaseSupplyDataItem>(id).ConfigureAwait(false);
			return supplyData;
		}

		public async Task<IList<ISupplyData>> GetSupplyDataAsync(Expression<Func<ISupplyData, bool>> filter)
		{
			// Sqlite-Net does not get all data and then
			// call a filter method on all results but instead filters directly at database level by turning an Expression
			// into valid SQL queries. An expression is not a delegate! It can be described as a recipe
			// which (in this case) will be turned into valid SQL queries. 
			// Since Sqlite-Net can only work with real types but not with interfaces, it expects an
			// "Expression<Func<DatabaseSupplyDataItem, bool>>" - but the caller cannot access the implementation specific
			// "DatabaseSupplyDataItem", he only has "ISupplyData".
			// The solution is to recreate the Expression and change the type to "DatabaseSupplyDataItem".
			var castExpression = Expression.Lambda<Func<DatabaseSupplyDataItem, bool>>(filter.Body, filter.Parameters);

			await this.InitDatabase();
			var result = await this.connection.Table<DatabaseSupplyDataItem>().Where(castExpression).ToListAsync();
			// The result is a list of "DatabaseSupplyItem" which cannot implicitly be cast to a list of "ISupplyData"
			// so we have to do this manually.
			var castResult = await Task.Run(() => result.ToList<ISupplyData>()).ConfigureAwait(false);
			return castResult;
		}

		public async Task DeleteSuppliesDataAsync()
		{
			await this.connection.DropTableAsync<DatabaseSupplyDataItem>();
			await this.connection.CreateTableAsync<DatabaseSupplyDataItem>();
		}
	}
}
