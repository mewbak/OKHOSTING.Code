using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// DataValueInstance dictionary with additional functionality for management of Atomized DataValues
	/// </summary>
	public class DataValueInstanceAtomizedDictionary : Dictionary<string, DataValueInstance>
	{
		/// <summary>
		/// Constructs the collection
		/// </summary>
		public DataValueInstanceAtomizedDictionary()
		{
		}
		
		/// <summary>
		/// Constructs the collection
		/// </summary>
		/// <param name="values">
		/// List<DataValueInstance> to convert
		/// </param>
		public DataValueInstanceAtomizedDictionary(List<DataValueInstance> values)
		{
			AddRange(values);
		}

		/// <summary>
		/// Atomizes and adds a single DataValueInstance<DataValueInstance>
		/// </summary>
		/// <param name="dvalue">DataValueinstance to be added</param>
		public void Add(DataValueInstance dvalue)
		{
			Add(dvalue, string.Empty);
		}

		/// <summary>
		/// Atomizes and adds a List<DataValueInstance>
		/// </summary>
		/// <param name="values">DataValueinstances to be added</param>
		public void AddRange(List<DataValueInstance> values)
		{
			AddRange(values, String.Empty);
		}

		/// <summary>
		/// Atomizes and adds a single DataValieInstance<DataValueInstance>
		/// </summary>
		/// <param name="values">DataValueinstances to be added</param>
		private void Add(DataValueInstance dvalue, string prefix)
		{
			//Validating argument 
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//Validating argument 
			if (prefix == null) prefix = string.Empty;

			//Validating if the current DataValue is a DataObject
			if (DataType.IsDataObjectSubClass(dvalue.DataValue.ValueType))
			{
				//Getting the DataObject 
				DataObject dobj;

				if (dvalue.Value == null)
				{
					dobj = DataObject.From(dvalue.DataValue.ValueType);
				}
				else
				{
					dobj = (DataObject)dvalue.Value;
				}

				//id dobj is null, make sure all values in the primary key are nulls
				//in order to avoid a primary key with mixed null/not null values
				if (NullValues.IsNull(dobj))
				{
					foreach (DataValueInstance dvi in dobj.PrimaryKey)
					{
						dvi.Value = NullValues.GetNullValue(dvi.DataValue.ValueType);
					}
				}

				//Add inner prikary key
				AddRange(dobj.PrimaryKey, prefix + dvalue.DataValue.ColumnName + DataValueAtomizedDictionary.AtomizedPrimaryKeySeparator);
			}
			else
			{
				//Copying the DataValue to DataValueInstanceDictionary
				this.Add(prefix + dvalue.DataValue.ColumnName, dvalue);
			}
		}

		/// <summary>
		/// Atomizes and adds a List<DataValueInstance>
		/// </summary>
		/// <param name="values">DataValueinstances to be added</param>
		/// <param name="prefix">Prefix that will be added before the name of each item</param>
		private void AddRange(List<DataValueInstance> values, string prefix)
		{
			//Validating if the collection argument is null
			if (values == null) throw new ArgumentNullException("collection");
			if (prefix == null) throw new ArgumentNullException("prefix");

			//Crossing the DataValuesInstances contained on the original collection
			foreach (DataValueInstance dvi in values)
			{
				Add(dvi, prefix);
			}
		}
	}
}