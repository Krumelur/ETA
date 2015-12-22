using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETA.Shared
{
	/// <summary>
	/// Gives access to local storage.
	/// </summary>
	public interface IStorage
	{
		/// <summary>
		/// Gets a configuration value.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		Task<string> GetConfigValueAsync(string key, string defaultValue = null);

		/// <summary>
		/// Saves a config value.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value">NULL deletes the value</param>
		/// <returns></returns>
		Task SetConfigValueAsync(string key, string value);

		Task SaveSupplyDataAsync(SupplyData supplyData);

		Task<SupplyData> GetSupplyDataAsync(int id);

		Task<List<SupplyData>> GetSupplyDataAsync(Func<SupplyData, bool> filter);
	}
}
