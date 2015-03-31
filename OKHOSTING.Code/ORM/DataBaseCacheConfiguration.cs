using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Defines which DataTypes can be cached and which can't
	/// </summary>
	public class DataBaseCacheConfiguration : OKHOSTING.Tools.ConfigurationBase
	{
		/// <summary>
		/// List of DataTypes that will be cached
		/// </summary>
		public List<DataType> DataTypes;

		/// <summary>
		/// Current configuration
		/// </summary>
		public static DataBaseCacheConfiguration Current;

		/// <summary>
		/// Loads the current configuration
		/// </summary>
		static DataBaseCacheConfiguration()
		{
			Current = (DataBaseCacheConfiguration)OKHOSTING.Tools.ConfigurationBase.Load(typeof(DataBaseCacheConfiguration));
		}
	}
}
