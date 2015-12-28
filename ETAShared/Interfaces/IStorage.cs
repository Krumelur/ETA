using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EtaShared
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

		/// <summary>
		/// Saves supply data.
		/// </summary>
		/// <param name="supplyData"></param>
		/// <returns></returns>
		Task<ISupplyData> SaveSupplyDataAsync(ISupplyData supplyData);

		/// <summary>
		/// Gets information about a specific supplies record.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<ISupplyData> GetSupplyDataAsync(int id);

		/// <summary>
		/// Gets filtered supplies.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		Task<IList<ISupplyData>> GetSupplyDataAsync(Expression<Func<ISupplyData, bool>> filter);

		/// <summary>
		/// Gets the newest stored supply data.
		/// </summary>
		/// <returns></returns>
		Task<ISupplyData> GetLatestSupplyDataAsync();

		/// <summary>
		/// Deletes all supplies history.
		/// </summary>
		Task DeleteSuppliesDataAsync();
	}
}
