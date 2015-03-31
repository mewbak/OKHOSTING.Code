using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// DataValue dictionary with additional functionality for management of Atomized DataValues
	/// </summary>
	public class DataValueAtomizedDictionary : Dictionary<string, DataValue>
	{
		/// <summary>
		/// Allows split an atomized primary key in its parts
		/// </summary>
		public const char AtomizedPrimaryKeySeparator = '_';
		
		/// <summary>
		/// Constructs the collection
		/// </summary>
		public DataValueAtomizedDictionary()
		{
		}

		/// <summary>
		/// Constructs the collection
		/// </summary>
		/// <param name="values">
		/// List<DataValueInstance> to convert
		/// </param>
		public DataValueAtomizedDictionary(List<DataValue> values)
		{
			AddRange(values);
		}

		/// <summary>
		/// Atomizes and adds a DataValue
		/// </summary>
		/// <param name="dvalue">DataValue to be added</param>
		public void Add(DataValue dvalue)
		{
			Add(dvalue, null);
		}

		/// <summary>
		/// Atomizes and adds a List<DataValue>
		/// </summary>
		/// <param name="values">DataValueinstances to be added</param>
		public void AddRange(List<DataValue> dvalues)
		{
			AddRange(dvalues, null);
		}

		/// <summary>
		/// Atomizes and adds a List<DataValue>
		/// </summary>
		/// <param name="values">DataValueinstances to be added</param>
		/// <param name="prefix">Prefix that will be added before the name of each item</param>
		private void Add(DataValue dvalue, string prefix)
		{
			//Validating argument 
			if (dvalue == null) throw new ArgumentNullException("value");

			//Validating argument 
			if (prefix == null) prefix = string.Empty;

			//Validating if the current DataValue is a DataObject
			if (DataType.IsDataObjectSubClass(dvalue.ValueType))
			{
				//Getting the DataType
				DataType dtype;

				dtype = (DataType) dvalue.ValueType;

				//Add inner prikary key
				AddRange(dtype.PrimaryKey, prefix + dvalue.ColumnName + DataValueAtomizedDictionary.AtomizedPrimaryKeySeparator);
			}
			else
			{
				//Copying the DataValue to DataValueInstanceDictionary
				this.Add(prefix + dvalue.ColumnName, dvalue);
			}
		}

		/// <summary>
		/// Atomizes and adds a List<DataValue>
		/// </summary>
		/// <param name="values">DataValueinstances to be added</param>
		/// <param name="prefix">Prefix that will be added before the name of each item</param>
		private void AddRange(List<DataValue> dvalues, string prefix)
		{
			//Validating if the collection argument is null
			if (dvalues == null) throw new ArgumentNullException("collection");

			//Crossing the DataValuesInstances contained on the original collection
			foreach (DataValue dv in dvalues)
			{
				Add(dv, prefix);
			}
		}
	}
}