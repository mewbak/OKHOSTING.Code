using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using OKHOSTING.Code.ORM.Filters;
using System.Collections.Generic;

namespace OKHOSTING.Code.ORM
{

	/// <summary>
	/// A collection of DataObjects
	/// </summary>
	/// 
	/// </typeparam>
	[Serializable]
	public class DataObjectList : List<DataObject>, IXmlSerializable
	{
		/// <summary>
		/// DataType of the items at this collection
		/// </summary>
		DataType dataType;

		/// <summary>
		/// Collection of Datavalues that are actually contained in this collection (not empty)
		/// </summary>
		List<DataValue> dataValues;

		/// <summary>
		/// Creates a new DataObjectCollection instance
		/// </summary>
		public DataObjectList()
		{
		}
		
		/// <summary>
		/// Creates a new DataObjectCollection instance
		/// </summary>
		/// <param name="dataType">The Type of DataObjects that this collection will handle</param>
		public DataObjectList(DataType dataType)
		{
			this.dataType = dataType;
		}

		/// <summary>
		/// DataType of the items at this collection
		/// </summary>
		public DataType DataType
		{
			get
			{
				return dataType;
			}
		}

		/// <summary>
		/// Collection of Datavalues that are actually contained in this collection (not empty)
		/// </summary>
		public List<DataValue> DataValues
		{
			get
			{
				//if no dataValues have been set, use all DataValues from DataSource by default
				if (dataValues == null && DataType != null)
				{
					return DataType.AllValues;
				}
				else if (dataValues != null)
				{
					return dataValues;
				}
				else
				{
					return null;
				}
			}
			set
			{
				dataValues = value;
			}
		}

		/// <summary>
		/// Returns a subset of the collection applying the specified filter
		/// </summary>
		/// <param name="filter">
		/// Filter used to get the collection subset
		/// </param>
		/// <returns>
		/// Subset of the collection applying the specified filter
		/// </returns>
		public DataObjectList Select(IFilter filter)
		{
			//Validating if the filter was specified
			if (filter == null)
			{
				//Without filter; returning the complete collection
				return this;
			}
			else
			{
				//Creating a filter collection that contains only the 
				//specified filter
				FilterList col = new FilterList();
				col.Add(filter);

				//Calling to the Select method overload that really does the work
				return this.Select(col);
			}
		}

		/// <summary>
		/// Returns a subset of the collection applying the specified filters
		/// </summary>
		/// <param name="filters">
		/// Filters used to get the collection subset
		/// </param>
		/// <returns>
		/// Subset of the collection applying the specified filters
		/// </returns>
		public DataObjectList Select(FilterList filters)
		{
			//Local vars
			DataObjectList col;
			bool match;

			//Validating if exists filters defined
			if (filters == null)
			{
				return this;
			}

			//Creating collection for subset store
			col = new DataObjectList(this.DataType);

			//Crossing the DataObjects of the collection
			foreach (DataObject dobj in this)
			{
				//Initializing cycle variables
				match= true;

				//Crossing the filters and validating if each one
				//is succesfully evaluated for the current DataObject. 
				//Only if all filters are fulfilled the DataObject will 
				//be added to the filtered collection
				foreach(IFilter filter in filters)
				{
					//Evaluating the filter with the current DataObject
					match = filter.Match(dobj);

					//If the evaluation was false, then breaking the cycle;
					//The DataObject will not be added to the filtered collection
					if (!match) break;
				}

				//Only if all the filters are fulfilled the DataObject 
				//is Added to the filtered collection
				if (match) col.Add(dobj);
			}

			//Retrieving the filtered collection
			return col;
		}

		/// <summary>
		/// Select from the collection the DataObject with the primary 
		/// key that have the DataObject passed as argument
		/// </summary>
		/// <param name="item">
		/// DataObject in which his primary key will be considered 
		/// for the search
		/// </param>
		/// <returns>
		/// null if dont exists an DataObject with the primary key of the
		/// item argument in the collection, otherwise, a reference to the 
		/// item
		/// </returns>
		public DataObject Select(DataObject item)
		{
			//Crossing the items in the collection
			foreach (DataObject dobj in this)
			{
				//Validating if the primary key of the current DataObject
				//is the same that the primary key of the item argument
				if (dobj.Equals(item))
				{
					//Retrieving the object and Leaving the procedure
					return dobj;
				}
			}

			//The searched item wasn't founded; returning null
			return null;
		}

		/// <summary>
		/// Sort the collection in ascending order. This method sorts
		/// considering the ToString() method result of each DataObject 
		/// contained on the collection, and for this reason, is highly 
		/// recommendable override this method on each DataObject that 
		/// you desire sort throught this method
		/// </summary>
		public new void Sort()
		{
			//Calling to the overload that really does the work
			Sort(SortDirection.Ascending);
		}

		/// <summary>
		/// Sort the collection in the specified order. This method sorts
		/// considering the ToString() method result of each DataObject 
		/// contained on the collection, and for this reason, is highly 
		/// recommendable override this method on each DataObject that 
		/// you desire sort throught this method
		/// </summary>
		/// <param name="direction">
		/// Direction of sort
		/// </param>
		public void Sort(SortDirection direction)
		{
			//Creating arrays of DataObjects and his keys used for sort
			string[] keys = new string[this.Count];
			DataObject[] dobjs = new DataObject[this.Count];

			//Crossing the items of the collection and 
			//filled the keys and dataObjects arrays
			for(int i= 0; i < this.Count; i++)
			{
				//IMPORTANT: The method ToString() will be 
				//used as key for the sort
				keys[i]= this[i].ToString();
				dobjs[i]= this[i];
			}

			//Sorting using the keys array (the result of ToString() method on each DataObject)
			Array.Sort(keys, dobjs);

			//Re-asign sorted dobjs
			for (int i = 0; i < this.Count; i++)
			{
				//Sort ascending or descending
				if (direction == SortDirection.Ascending)
				{
					this[i] = dobjs[i];
				}
				else
				{
					this[this.Count - 1 - i] = dobjs[i];
				}
			}
		}

		/// <summary>
		/// Sorts the collection by the specified DataValue
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used for sort
		/// </param>
		/// <param name="direction">
		/// Direction of the sort
		/// </param>
		public void Sort(OrderByItem order)
		{
			//Local vars
			object[] keys;
			DataObject[] dobjs;
			object val;
			
			//Validating that the DataValue isn't null
			if (order == null)
			{
				throw new ArgumentNullException("order");
			}

			//Validating if the DataValue exists on the DataType of the items 
			if (!this.DataType.ContainsMember(order.OrderBy.Name))
			{
				throw new ArgumentException("DataType '" + this.DataType.FullName + "' does not contain a definition for '" + order.OrderBy + "'", "order.OrderBy");
			}

			//Creating arrays for the items of the collection and his keys to sort
			keys= new object[this.Count];
			dobjs = new DataObject[this.Count];

			//Crossing the items of the collection
			for(int i= 0; i < this.Count; i++)
			{
				//Getting the value of the member corresponding to the DataValue 
				//at the current DataObject
				val = this[i].GetValue(order.OrderBy);

				//Determining the value to use as sort key
				if (val is System.IComparable)
				{
					//The value implements IComparable, the value is used as key
					keys[i] = val;
				}
				else if (val == null)
				{
					//The value is null, the key too
					keys[i] = null;
				}
				else
				{
					//The value is not null an don't implement IComparable; 
					//The sort key is equal to the result of ToString() method
					keys[i] = val.ToString();
				}
				
				//Adding the DataObject to the DataObjects array
				dobjs[i]= this[i];
			}

			//Sorting...
			Array.Sort(keys, dobjs);
			
			//Re-asign sorted dobjs
			for (int i = 0; i < this.Count; i++)
			{
				//Sort ascending or descending
				if (order.Direction == SortDirection.Ascending)
				{
					this[i] = dobjs[i];
				}
				else
				{
					this[this.Count - 1 - i] = dobjs[i];
				}
			}
		}

		/// <summary>
		/// Returns the collection represented as string
		/// </summary>
		/// <returns>
		/// An string with the name of each DataObject contained
		/// on the collection. The name of each DataObject is getted
		/// using the ToString() method, and for this reason, is highly 
		/// recommendable override this method on each DataObject 
		/// </returns>
		public override string ToString()
		{
			//Local Vars
			string str = "";

			//Crosing the DataObjects on the collection
			foreach (DataObject dobj in this)
			{
				//Adding to the string the info of the current DataObject
				str += dobj + "\n";
			}

			//Retrieving the string
			return str;
		}

		#region DataBase operations

		/// <summary>
		/// Inserts the complete DataObjectCollection into the DataBase
		/// </summary>
		public void Insert()
		{
			DataBase.Current.Insert(this);
		}

		/// <summary>
		/// Updates the complete DataObjectCollection into the DataBase
		/// </summary>
		public void Update()
		{
			DataBase.Current.Update(this);
		}

		/// <summary>
		/// Deletes the complete DataObjectCollection into the DataBase
		/// </summary>
		public void Delete()
		{
			DataBase.Current.Delete(this);
		}

		/// <summary>
		/// Saves the complete DataObjectCollection into the DataBase
		/// </summary>
		public void Save()
		{
			foreach (DataObject dobj in this)
			{
				dobj.Save();
			}
		}

		/// <summary>
		/// Returns a value indicating if the specified DataObject exists on the DataObjectCollection (based on its primary key)
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be searched in the DataObjectCollection
		/// </param>
		/// <returns>
		/// True if the DataObject exists, False otherwise
		/// </returns>
		public bool Exists(DataObject dobj)
		{
			return Select(dobj) != null;
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Following online help recommendations, this method allways return null
		/// </summary>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Deserialize the specified Xml to respective 
		/// DataObject instance
		/// </summary>
		/// <param name="reader">
		/// XmlReader Object used to read the xml DataObject representation
		/// </param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			XmlReader subReader = reader.ReadSubtree();
			subReader.Read();

			//read all child nodes
			while (subReader.Read())
			{
				DataType dtype;
				DataObject dobj;

				if (subReader.Name == typeof(DataObjectList).Name && subReader.NodeType == XmlNodeType.EndElement) 
					break;

				//get DataType
				subReader.MoveToAttribute("DataType");
				dtype = DataType.Parse(subReader.Value);
				subReader.MoveToElement();

				//create DataObject
				dobj = DataObject.From(dtype);

				//deserialize DataObject
				((IXmlSerializable)dobj).ReadXml(subReader);

				//add deserialized DataObject
				this.Add(dobj);
			}

			subReader.Close();
		}

		/// <summary>
		/// Serialize the DataObject
		/// </summary>
		/// <param name="writer">
		/// XmlWriter used for the serialization
		/// </param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			//serialize each DataObject
			foreach (DataObject dobj in this)
			{
				writer.WriteStartElement(dobj.DataType.Name);
				((IXmlSerializable) dobj).WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		#endregion
	}
}