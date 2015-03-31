/*

Copyright 2003-2010 OK HOSTING S.C.
info@okhosting.com
okhosting.com
 
Authors: Edgard David Iván Muñoz Chávez, Leopoldo Arenas Flores

This file is part of Softosis.

Softosis is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

Softosis is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Softosis.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{

	/// <summary>
	/// Allows you management DataValues and convert them form 
	/// DataValue instances to string reprentations and viceversa
	/// </summary>
	/// <remarks>Usefull for creating string representation of DataObjects, that can be used in filenames and url's, and then be parsed again into a DataObject</remarks>
	public static class DataValueString
	{
		/// <summary>
		/// Returns a DataObject with all the DataValues
		/// whose values exists in the values collection
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataObject whose DataValues will be loaded
		/// </param>
		/// <param name="valuesString">
		/// NameValueCollection with the next structure:
		/// 
		///		- Name: Name of the DataValue referenced by the item
		///		- Value: String representation of the value of referenced DataValue
		///		
		/// </param>
		/// <returns>
		/// DataObject with all the DataValues
		/// of the specified DataType whose values exists in the values
		/// collection
		/// </returns>
		public static DataObject ToDataObject(DataType dtype, NameValueCollection values)
		{
			//Validating if the dtype and pkQueryString arguments are null
			if (dtype == null) throw new ArgumentNullException("dtype");
			if (values == null) throw new ArgumentNullException("values");

			//Creating an instance of the DataObject with the specified type
			DataObject dobj = DataObject.From(dtype);
			dobj.InitForeignKeys();

			//Filling the atomized dictionary
			DataValueInstanceAtomizedDictionary atomized;
			atomized = new DataValueInstanceAtomizedDictionary();
			atomized.AddRange(dobj.PrimaryKey);
			atomized.AddRange(dobj.RegularValues);

			//Crossing the DataValues candidates to load
			foreach (KeyValuePair<string, DataValueInstance> dvi in atomized)
			{
				//Getting the value of the current DataValue
				string val = values[dvi.Key];

				//Validating if the value was specified on dicResult collection
				if (val != null)
				{
					//Parsing the value 
					dvi.Value.Value = TypeConverter.ChangeType(val, dvi.Value.DataValue.ValueType);
				}
			}

			//Returning the dictionary
			return dobj;
		}

		/// <summary>
		/// Returns a DataObject with all the DataValues
		/// whose values exists in the values collection
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataObject whose DataValues will be loaded
		/// </param>
		/// <param name="values">
		/// NameValueCollection with the next structure:
		/// 
		///		- Name: Name of the DataValue referenced by the item
		///		- Value: String representation of the value of referenced DataValue
		///		
		/// </param>
		/// <returns>
		/// DataObject with all the DataValues
		/// of the specified DataType whose values exists in the values
		/// collection
		/// </returns>
		public static DataObject ToDataObject(string value)
		{
			DataObject dobj;
			DataType dtype;

			dtype = Type.GetType(value.Split('|')[0], true);
			dobj = ToDataObject(dtype, value.Split('|')[1]);

			return dobj;
		}

		/// <summary>
		/// Returns a DataObject with all the DataValues
		/// whose values exists in the values collection
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataObject whose DataValues will be loaded
		/// </param>
		/// <param name="values">
		/// NameValueCollection with the next structure:
		/// 
		///		- Name: Name of the DataValue referenced by the item
		///		- Value: String representation of the value of referenced DataValue
		///		
		/// </param>
		/// <returns>
		/// DataObject with all the DataValues
		/// of the specified DataType whose values exists in the values
		/// collection
		/// </returns>
		public static DataObject ToDataObject(DataType dtype, string values)
		{
			//Validating if the dtype and pkQueryString arguments are null
			if (dtype == null) throw new ArgumentNullException("dtype");
			if (values == null) throw new ArgumentNullException("values");

			//Creating an instance of the DataObject with the specified type
			DataObject dobj = DataObject.From(dtype);
			dobj.InitForeignKeys();

			//Filling the atomized dictionary
			DataValueInstanceAtomizedDictionary atomized;
			atomized = new DataValueInstanceAtomizedDictionary();
			atomized.AddRange(dobj.PrimaryKey);
			atomized.AddRange(dobj.RegularValues);

			//Crossing the DataValues candidates to load
			foreach (KeyValuePair<string, DataValueInstance> dvi in atomized)
			{
				//Getting the value of the current DataValue
				string val = GetValueFromString(values, dvi.Key);

				//Validating if the value was specified on dicResult collection
				if (val != null)
				{
					//Parsing the value 
					dvi.Value.Value = TypeConverter.ChangeType(val, dvi.Value.DataValue.ValueType);
				}
			}

			//Returning the dictionary
			return dobj;
		}

		/// <summary>
		/// Creates a NameValueCollection filed with the atomized values of the DataObject
		/// </summary>
		/// <param name="dobj">
		/// DataObject which will be atomized and represented in the NameValueCollection
		/// </param>
		/// <returns>
		/// NameValueCollection filed with the atomized values of the List
		/// </returns>
		public static NameValueCollection ToNameValues(DataObject dobj)
		{
			//Validating if the dobj argument is null
			if (dobj == null) throw new ArgumentNullException("dobj");

			//Creating a value collection with the primary key and regular values
			List<DataValueInstance> values = new List<DataValueInstance>();
			values.AddRange(dobj.PrimaryKey);
			values.AddRange(dobj.RegularValues);
			
			//Returning the NameValueCollection
			return ToNameValues(values);
		}

		/// <summary>
		/// Creates a NameValueCollection filed with the atomized values of the List
		/// </summary>
		/// <param name="dataValuesInstances">
		/// List which will be atomized and represented in the NameValueCollection
		/// </param>
		/// <returns>
		/// NameValueCollection filed with the atomized values of the List
		/// </returns>
		public static NameValueCollection ToNameValues(List<DataValueInstance> dataValuesInstances)
		{
			//Validating if the dataProperties argument is null
			if (dataValuesInstances == null) throw new ArgumentNullException("dataProperties");

			//Creating the result
			NameValueCollection result = new NameValueCollection();

			//Filling the atomized dictionary
			DataValueInstanceAtomizedDictionary atomized;
			atomized = new DataValueInstanceAtomizedDictionary(dataValuesInstances);

			//Crossing the DataValues candidates to load
			foreach (KeyValuePair<string, DataValueInstance> dvi in atomized)
			{
				//Validating if the value was specified on dicResult collection
				if (!NullValues.IsNull(dvi.Value.Value))
				{
					//Adding the value to the collection
					result.Add(dvi.Key, TypeConverter.ToString(dvi.Value.Value));
				}
			}

			//Returning the NameValueCollection
			return result;
		}

		/// <summary>
		/// Returns the string representation of the specified DataObject (includes the primary key only)
		/// </summary>
		/// <param name="dobj">DataObject which will be atomized and represented in a string</param>
		/// <returns>
		/// The primary key values properties stored on a string on the format 
		/// "Property1=ValueOfProperty1&Property2=ValueOfProperty2"
		/// </returns>
		public static string ToString(DataObject dobj)
		{
			//Returning the string to the caller
			return ToString(dobj.PrimaryKey);
		}

		/// <summary>
		/// Creates a string representation of a List
		/// </summary>
		/// <param name="dataValuesInstances">List that will be parsed as a string</param>
		/// <returns>String containing all values in dataValuesInstances</returns>
		public static string ToString(List<DataValueInstance> dataValuesInstances)
		{
			//First, convert to NameValueCollection
			NameValueCollection nameValues = ToNameValues(dataValuesInstances);

			//Now convert to string
			return ToString(nameValues);
		}

		/// <summary>
		/// Returns the string representation of the specified NameValueCollection
		/// </summary>
		/// <param name="values">List that will be parsed as a string</param>
		/// <returns>
		/// The properties stored on values on the format
		/// Property1=ValueOfProperty1&Property2=ValueOfProperty2...
		/// </returns>
		public static string ToString(NameValueCollection values)
		{
			//Local Vars
			string nameValues = string.Empty;

			//Crossing the name / value pairs 
			foreach (string name in values.AllKeys)
			{
				nameValues += name + "=" + values[name] + "&";
			}

			//Remove last &
			nameValues = nameValues.TrimEnd('&');

			//Returning the string to the caller
			return nameValues;
		}

		/// <summary>
		/// Gets the value of a key inside a string
		/// </summary>
		/// <param name="values">String that contains key value pairs</param>
		/// <param name="key">Key which value will be obtained from the string</param>
		/// <returns>Value of the specified key</returns>
		private static string GetValueFromString(string values, string key)
		{
			int i = values.IndexOf(key);
			
			if (i != -1)
			{
				int j = values.IndexOf('=', i);
				int k = values.IndexOf('&', j);
				
				if (j != -1)
				{
					if (k != -1)
					{
						return values.Substring(j + 1, k - (j + 1));
					}
					else
					{
						return values.Substring(j + 1);
					}
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}
	}
}
