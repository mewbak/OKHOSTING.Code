using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OKHOSTING.Tools;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Cache system that allows database Select operations to be "cached" in order to improve performance.
	/// When a Select operation is performed with no filters, the result is stored in a memory cache during the application lifecycle,
	/// in case the same Select operation is performed again, result will be returned from the cache instead of performing the operation in the DataBase again.
	/// When a Inser, Delete or Update operation is performed in a cached DataType, the cache is deleted to avoid incorrect select results.
	/// </summary>
	public static class DataBaseCache
	{
		/// <summary>
		/// Private cache with results of different DataTypes Select operations
		/// </summary>
		private static Dictionary<DataType, DataObjectList> Cache = new Dictionary<DataType,DataObjectList>();

		/// <summary>
		/// Subscribes to DataBase events so datalogs can be created for every database-write operation
		/// </summary>
		public static void PlugIn_OnSessionStart()
		{
			//avoid duplicate subscriptions
			DataBase.Current.BeforeSelect -= DataBase_BeforeSelect;
			DataBase.Current.AfterSelect -= DataBase_AfterSelect;
			DataBase.Current.AfterInsert -= DataBase_AfterOperation;
			DataBase.Current.AfterUpdate -= DataBase_AfterOperation;
			DataBase.Current.AfterDelete -= DataBase_AfterOperation;

			DataBase.Current.BeforeSelect += new DataBase.BeforeSelectEventHandler(DataBase_BeforeSelect);
			DataBase.Current.AfterSelect += new DataBase.AfterSelectEventHandler(DataBase_AfterSelect);
			DataBase.Current.AfterInsert += new DataBase.OperationEventHandler(DataBase_AfterOperation);
			DataBase.Current.AfterUpdate += new DataBase.OperationEventHandler(DataBase_AfterOperation);
			DataBase.Current.AfterDelete += new DataBase.OperationEventHandler(DataBase_AfterOperation);
		}

		/// <summary>
		/// See if the result is already cached. If cached, return the cached result and cancel the DataBase select operation.
		/// </summary>
		/// <remarks>
		/// Only works for select operations with no filters
		/// </remarks>
		static void DataBase_BeforeSelect(DataBase sender, SelectEventArgs e)
		{
			//exit if the operation conaints any filter
			if(e.Filters.Count > 0 || e.OrderBy.Count > 0) return;

			//see if result is already cached, and if so, cancel the operation and return the cached result
			if (Cache.ContainsKey(e.DataType))
			{
				e.Cancel = true;
				e.Result = Cache[e.DataType];
				Log.WriteDebug("Cache returned for:" + e.DataType);
			}
		}

		/// <summary>
		/// See if the result is already cached, and if not, cache it
		/// </summary>
		/// <remarks>
		/// Only works for select operations with no filters
		/// </remarks>
		static void DataBase_AfterSelect(DataBase sender, SelectEventArgs e)
		{
			//exit if the operation conaints any filter
			if (e.Filters.Count > 0 || e.OrderBy.Count > 0) return;

			//see if result is already cached, and if so, cancel the operation and return the cached result
			if (DataBaseCacheConfiguration.Current.DataTypes.Contains(e.DataType) && !Cache.ContainsKey(e.DataType))
			{
				Cache.Add(e.DataType, e.Result);
				Log.WriteDebug("Cache created for:" + e.DataType);
			}
		}

		#region TODO: Aplicar filtros

		///// <summary>
		///// See if the result is already cached. If cached, return the cached result and cancel the DataBase select operation.
		///// </summary>
		//static void DataBase_BeforeSelect(DataBase sender, SelectEventArgs e)
		//{
		//	//exit if the operation conaints any filter
		//	//if(e.Filters.Count > 0 || e.OrderBy.Count > 0) return;

		//	//see if result is already cached, and if so, cancel the operation and return the cached result
		//	if (Cache.ContainsKey(e.DataType))
		//	{
		//		e.Cancel = true;
		//		e.Result = Cache[e.DataType];

		//		//apply filters
		//		if (e.Filters.Count > 0)
		//		{
		//			e.Result = e.Result.Select(e.Filters);
		//		}

		//		//apply first order by only
		//		if (e.OrderBy.Count > 0)
		//		{
		//			e.Result.Sort(e.OrderBy[0]);
		//		}

		//		Log.Write("Cache returned for:" + e.DataType);
		//	}
		//}

		///// <summary>
		///// See if the result is already cached, and if not, cache it
		///// </summary>
		//static void DataBase_AfterSelect(DataBase sender, SelectEventArgs e)
		//{
		//	//exit if the operation conaints any filter

		//	//see if result is already cached, and if so, cancel the operation and return the cached result
		//	if (!Cache.ContainsKey(e.DataType))
		//	{
		//		if (e.Filters.Count == 0)
		//		{
		//			//cache the result
		//			Cache.Add(e.DataType, e.Result);
		//			Log.Write("Cache created for:" + e.DataType);
		//		}

		//		//else if (DataBase.Current.Count(e.DataType) < 100)
		//		else
		//		{
		//			//get the full table and cache the result
		//			DataBase.Current.Select(e.DataType);
		//		}
		//	}
		//}

		#endregion

		/// <summary>
		/// If an insert, update or delete operation is performed on a cached DataType, delete the cache
		/// </summary>
		static void DataBase_AfterOperation(DataBase sender, DataBaseOperationEventArgs e)
		{
			if (Cache.ContainsKey(e.DataObject.DataType))
			{
				Cache.Remove(e.DataObject.DataType);
				Log.WriteDebug("Cache removed for:" + e.DataObject.DataType);
			}
		}
	}
}