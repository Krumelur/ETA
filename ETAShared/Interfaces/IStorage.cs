using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

		Task<ISupplyData> SaveSupplyDataAsync(ISupplyData supplyData);

		Task<ISupplyData> GetSupplyDataAsync(int id);

		Task<IList<ISupplyData>> GetSupplyDataAsync(Expression<Func<ISupplyData, bool>> filter);
	}
}
