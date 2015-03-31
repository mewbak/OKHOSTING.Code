using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OKHOSTING.Code.ORM;
using OKHOSTING.Code.ORM.Filters;
using OKHOSTING.Code.ORM.Validators;
using OKHOSTING.Tools;
using OKHOSTING.Tools.Extensions;
using System.Data;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// A DataBase is an organized data structure that allows select/insert/update/delete 
	/// operations of DataObjects
	/// </summary>
	/// <remarks>
	/// A Database can be any type of DataObject container, like a Sql DataBase, MySql, DataSet, 
	/// a file system, a memory DataBase, a remote api, etc.
	/// </remarks>
	public abstract class DataBase: System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Constructs the class instance (protected for hide to clients)
		/// </summary>
		protected DataBase()
		{
			//Raise the DataBaseCreated event
			DataBase.OnDataBaseCreated(this);
		}

		#region Properties

		/// <summary>
		/// Determines if the logical deleted entities are affected
		/// by database operations
		/// </summary>
		public virtual bool IncludeLogicalDeleted { get; set; }

		/// <summary>
		/// Retrieve the Database associated to the current session. By default
		/// the database associated will be the defined at the configuration file of the 
		/// application. However, this property can be set to the Database object that you 
		/// desire and since that moment, this property retrieve to you this Database
		/// </summary>
		public static DataBase Current
		{
			get
			{
				//if database already exist, retrieve it from the session
				if (Session.Current.ContainsKey(typeof(DataBase).FullName))
				{
					return (DataBase)Session.Current[typeof(DataBase).FullName];
				}
				///otherwise, create a new database and store it in the session
				else
				{
					//Create a new Database for the current process or session
					//deserializing from the configuration file
					DataBase current = ((Configuration)Configuration.Load(typeof(Configuration))).CurrentDataBase;

					//Storing the database in the current session
					Session.Current[typeof(DataBase).FullName] = current;

					//Retrieving database recently created
					return current;
				}
			}
			set
			{
				Session.Current[typeof(DataBase).FullName] = value;
			}
		}

		#endregion

		#region Select

		/// <summary>
		/// Load all the DataObjects of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataObjects to load
		/// </param>
		/// <returns>
		/// DataObjectCollection with all the DataObjects of the specified DataType
		/// </returns>
		public virtual DataObjectList Select(DataType dtype)
		{
			return Select(dtype, null, null, null);
		}

		/// <summary>
		/// Load all the DataObjects of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataObjects to load
		/// </param>
		/// <returns>
		/// DataObjectCollection with all the DataObjects of the specified DataType
		/// </returns>
		public virtual DataObjectList Select(DataType dtype, FilterList filters)
		{
			return Select(dtype, filters, null, null);
		}

		/// <summary>
		/// Load all the DataObjects of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataObjects to load
		/// </param>
		/// <param name="filters">
		/// Filters collection that the DataObjects to load 
		/// must fulfill
		/// </param>
		/// <param name="orderBy">
		/// Defines the order in which the results will be sorted
		/// </param>
		/// <returns>
		/// DataObjectCollection with all the DataObjects of the specified DataType
		/// </returns>
		public virtual DataObjectList Select(DataType dtype, FilterList filters, List<OrderByItem> orderBy)
		{
			return Select(dtype, filters, orderBy, null);
		}

		/// <summary>
		/// Loads the DataObject from the DataBase, using it's primary key
		/// </summary>
		/// <param name="dobj">
		/// DataObject with a defined primary key
		/// </param>
		/// <returns>
		/// Same DataObject filled with all the information from the DataBase
		/// </returns>
		public virtual DataObject Select(DataObject dobj)
		{
			//Validating if the dobj argument is null
			if (NullValues.IsNull(dobj)) throw new ArgumentNullException("dobj");

			//Creating a primary key filter
			PrimaryKeyFilter pkFilter = new PrimaryKeyFilter(dobj);

			//Selecting the DataObject specified
			DataObjectList result = Select(dobj.DataType, new FilterList(pkFilter));

			//Validating if the DataObject was founded
			if (result.Count > 0)
			{
				//Copying DataObject and returnig it
				result[0].CopyTo(dobj);
			}
			else
			{
				dobj = null;
			}

			//return resulting dataobject
			return dobj;
		}

		/// <summary>
		/// Loads the DataObject from the DataBase, using it's primary key
		/// </summary>
		/// <param name="dobj">
		/// DataObject with a defined primary key
		/// </param>
		/// <param name="loadForeignKeys">
		/// If true, the foreign key DataObjects will be loaded too
		/// </param>
		/// <returns>
		/// Same DataObject filled with all the information from the DataBase
		/// </returns>
		public virtual DataObject Select(DataObject dobj, bool loadForeignKeys)
		{
			//Loading the DataObject from its primary key
			Select(dobj);

			//Validating if the aggregate objects must be loaded
			if (loadForeignKeys)
			{
				//Crossing the DataValues of the DataObject
				foreach (DataValueInstance dvi in dobj.AllValues)
				{
					//Validating if the current DataValue is an DataObject
					if (dvi.DataValue.IsForeignKey)
					{
						//Loading the aggregate DataObject
						DataObject fk = (DataObject)dvi.Value;
						Select(fk);
					}
				}
			}

			//Devolviendo DataObject
			return dobj;
		}

		/// <summary>
		/// Loads the DataObject from the DataBase, using it's primary key. This method
		/// searches for inherited DataObjects and returns the lower inherited DataObject found in the DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject with a defined primary key
		/// </param>
		/// <returns>
		/// Same DataObject filled with all the information from the DataBase
		/// </returns>
		public virtual DataObject SelectInherited(DataObject dobj)
		{
			//Validating if the dobj argument is null
			if (dobj == null) throw new ArgumentNullException("dobj", "Argument cannot be null");

			DataObjectList inherited;
			dobj.SelectOnce();

			inherited = SearchInheritedFrom(dobj);
			if (inherited.Count == 0)
			{
				return dobj;
			}
			else
			{
				return SelectInherited(inherited[0]);
			}
		}

		/// <summary>
		/// Searches for a DataObject with a primary key = id. It assumes dtype has a single primary key value of integral type
		/// </summary>
		/// <param name="id">Id of the DataObject to select</param>
		/// <returns>Found DataObject with all it's values</returns>
		public virtual DataObject SelectById(DataType dtype, int id)
		{
			DataObject dobj;

			dobj = DataObject.From(dtype);
			dobj.PrimaryKey[0].Value = TypeConverter.ChangeType(id, dobj.PrimaryKey[0].DataValue.ValueType);

			return Select(dobj);
		}

		/// <summary>
		/// Loads a collection of children DataObjects.
		/// Children DataObjects are DataObject with a ForeignKey which value equals dobj
		/// </summary>
		/// <param name="value">
		/// DataObject which be used as a filter for searching children
		/// </param>
		/// <param name="foreignKey">
		/// DataValue inside a DataType which values will be compared with dobj
		/// </param>
		public virtual DataObjectList SelectByForeignKey(DataValue foreignKey, DataObject value)
		{
			return SelectByForeignKey(foreignKey, value, null);
		}

		/// <summary>
		/// Loads a collection of children DataObjects.
		/// Children DataObjects are DataObject with a ForeignKey which value equals dobj
		/// </summary>
		/// <param name="value">
		/// DataObject which be used as a filter for searching children
		/// </param>
		/// <param name="foreignKey">
		/// DataValue inside a DataType which values will be compared with dobj
		/// </param>
		/// <param name="dvalues">List of DataValues that will be selected</param>
		public virtual DataObjectList SelectByForeignKey(DataValue foreignKey, DataObject value, List<DataValue> dvalues)
		{
			DataObjectList children;
			ForeignKeyFilter filter;

			//Validating if the arguments are null
			if (foreignKey == null) throw new ArgumentNullException("childRelatedValue", "Argument cannot be null");
			if (value == null) throw new ArgumentNullException("dobj", "Argument cannot be null");

			//Getting the collection reference and initializing it
			children = new DataObjectList(foreignKey.DeclaringDataType);

			//Loading the DataObject collection corresponding to the DataValue to fill
			filter = new ForeignKeyFilter(foreignKey, value);
			children = this.Select(foreignKey.DeclaringDataType, new FilterList(filter), null, dvalues);

			//Crossing the DataObject on the collection
			foreach (DataObject item in children)
			{
				//Establishing the foreign key on the foreign entity
				item.SetValue(foreignKey, value);
			}

			return children;
		}

		/// <summary>
		/// Load the DataObjects of the specified DataType that
		/// fulfills the indicated filters (combined with the And 
		/// Logical Operator)
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataValues to load
		/// </param>
		/// <param name="dvalues">
		/// List of DataValues to load
		/// </param>
		/// <param name="orderName">
		/// Name of the order definition used to sort the DataObjectCollection to return
		/// (or null if not used)
		/// </param>
		/// <param name="filters">
		/// Filters collection that the DataObjects to load 
		/// must fulfill
		/// </param>
		/// <returns>
		/// DataObjectCollection with the loaded DataObjects
		/// </returns>
		public virtual DataObjectList Select(DataType dtype, FilterList filters, List<OrderByItem> orderBy, List<DataValue> dvalues)
		{
			return Select(dtype, filters, orderBy, dvalues, null);
		}

		/// <summary>
		/// Load the DataObjects of the specified DataType that
		/// fulfills the indicated filters (combined with the And 
		/// Logical Operator)
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataValues to load
		/// </param>
		/// <param name="dvalues">
		/// List of DataValues to load
		/// </param>
		/// <param name="orderBy">
		/// Sorts the result by one or more DataValues, ascending or descending
		/// </param>
		/// <param name="filters">
		/// Filters collection that the DataObjects to load must fulfill
		/// </param>
		/// <param name="limit">
		/// Used for paging, to return only the DataObjects between an index and another
		/// </param>
		/// <returns>
		/// DataObjectCollection with the loaded DataObjects
		/// </returns>
		public abstract DataObjectList Select(DataType dtype, FilterList filters, List<OrderByItem> orderBy, List<DataValue> dvalues, SelectLimit limit);

		/// <summary>
		/// Returns a DataTable with the results of the 
		/// specified select with aggregate functions
		/// </summary>
		/// <param name="aggegateSelectFields">
		/// Aggregate fields definitions
		/// </param>
		/// <param name="dtype">
		/// DataType to query
		/// </param>
		/// <returns>
		/// DataTable with the results of the 
		/// specified select with aggregate functions
		/// </returns>
		public virtual DataTable SelectGroup(DataType dtype, List<AggregateSelectField> aggregateSelectFields)
		{
			return SelectGroup(dtype, aggregateSelectFields, null);
		}

		/// <summary>
		/// Returns a DataTable with the results of the 
		/// specified select with aggregate functions
		/// </summary>
		/// <param name="aggegateSelectFields">
		/// Aggregate fields definitions
		/// </param>
		/// <param name="dtype">
		/// DataType to query
		/// </param>
		/// <param name="dataValuesToGroup">
		/// Columns for group by
		/// </param>
		/// <returns>
		/// DataTable with the results of the 
		/// specified select with aggregate functions
		/// </returns>
		public virtual DataTable SelectGroup(DataType dtype, List<AggregateSelectField> aggregateSelectFields, List<DataValue> dataValuesToGroup)
		{
			return SelectGroup(dtype, aggregateSelectFields, dataValuesToGroup);
		}

		/// <summary>
		/// Returns a DataTable with the results of the 
		/// specified select with aggregate functions
		/// </summary>
		/// <param name="aggegateSelectFields">
		/// Aggregate fields definitions
		/// </param>
		/// <param name="dtype">
		/// DataType to query
		/// </param>
		/// <param name="filters">
		/// Filters to apply on Select
		/// </param>
		/// <param name="dataValuesToGroup">
		/// Columns for group by
		/// </param>
		/// <param name="orderBy">
		/// Defines the order in which the results will be sorted
		/// </param>
		/// <returns>
		/// DataTable with the results of the 
		/// specified select with aggregate functions
		/// </returns>
		public abstract DataTable SelectGroup(DataType dtype, List<AggregateSelectField> aggregateSelectFields, List<DataValue> dataValuesToGroup, FilterList filters, List<OrderByItem> orderBy);

		#endregion

		#region Search

		/// <summary>
		/// Returns a value indicating if the specified DataObject exists on the DataBase (based on its primary key)
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be searched in the DataBase
		/// </param>
		/// <returns>
		/// True if the DataObject exists, False otherwise
		/// </returns>
		public bool Exists(DataObject dobj)
		{
			if (NullValues.IsNull(dobj))
			{
				return false;
			}
			else
			{
				//Creating a primary key filter
				PrimaryKeyFilter pkFilter = new PrimaryKeyFilter(dobj);

				//Selecting the DataObject specified
				DataObjectList result = Select(dobj.DataType, new FilterList(pkFilter), null, dobj.DataType.PrimaryKey);

				return result.Count > 0;
			}
		}

		/// <summary>
		/// Returns a value indicating if the specified DataObject exists (based on its primary key)
		/// as a specific DataType
		/// </summary>
		/// <remarks>
		/// Use this method to see if a DataObject exists in the Database as a base class.
		/// A DataObject could exist in the DataBase as a base class, but not as the final class.
		/// </remarks>
		/// <example>
		///	Class Dog inherits from Class Animal. In your Database, you have an Animal with Id = 5, 
		///	but it's not a Dog, it's just an animal. 
		///	Dog dog = new Dog();
		///	dog.Id = 5;
		///	DataBase.Current.Exist(dog);					//will return false
		///	DataBase.Current.Exist(dog, typeof(Dog));		//will return false
		///	DataBase.Current.Exist(dog, typeof(Animal));	//will return true
		/// </example>
		/// <param name="dobj">
		/// DataObject to be searched in the DataBase
		/// </param>
		/// <param name="dtype">
		/// DataType (must be a base class of dobj) that will be searched in the DataBase
		/// </param>
		/// <returns>
		/// True if the DataObject exists as the specified DataType, False otherwise
		/// </returns>
		public bool Exists(DataObject dobj, DataType dtype)
		{
			//create a child DataObject and populate it with the same primary key
			DataObject child = (DataObject) dtype.InnerType.CreateInstance();
			dobj.CopyTo(child);

			//Creating a primary key filter
			PrimaryKeyFilter pkFilter = new PrimaryKeyFilter(child);

			//Selecting the DataObject specified
			DataObjectList result = Select(child.DataType, new FilterList(pkFilter), null, child.DataType.PrimaryKey);

			return result.Count > 0;
		}

		/// <summary>
		/// Do a generical search of the entities with the specified DataType
		/// searching in all it DataValues the specified string with the like 
		/// pattern. This search can use an excessive amount of system resources
		/// reason why it's use is recommended only for small sets of data
		/// </summary>
		/// <param name="dtype">
		/// DataType for search
		/// </param>
		/// <param name="searchedString">
		/// String that will be searched
		/// </param>
		/// <returns>
		/// DataObject collection with all the DataObjects that contains the 
		/// searchedString on it's DataValues
		/// </returns>
		public virtual DataObjectList Search(DataType dtype, string searchedString)
		{
			return Search(dtype, searchedString, false, null, null, null);
		}

		/// <summary>
		/// Do a generical search of the entities with the specified DataType
		/// searching in all it DataValues the specified string with the like 
		/// pattern. This search can use an excessive amount of system resources
		/// reason why it's use is recommended only for small sets of data
		/// </summary>
		/// <param name="dtype">
		/// DataType for search
		/// </param>
		/// <param name="searchedString">
		/// String that will be searched
		/// </param>
		/// <param name="deepSearch">
		/// If set to true, the search looks into inbound and oubound 
		/// foreign keys ass well as the main dtype, 
		/// if false, will only search into the main dtype
		/// </param>
		/// <returns>
		/// DataObject collection with all the DataObjects that contains the 
		/// searchedString on it's DataValues
		/// </returns>
		public virtual DataObjectList Search(DataType dtype, string searchedString, bool deepSearch, List<OrderByItem> orderBy, List<DataValue> dvalues, SelectLimit limit)
		{
			//Local Vars
			OrFilter orAux;

			//Validating if the dtype argument is null
			if (dtype == null) throw new ArgumentNullException("dtype", "Argument cannot be null");

			//Creating the Or logical filters for the Query
			OrFilter or = GetSearchFilter(dtype, searchedString);

			//Searching in all outbound foreign keys
			if (deepSearch)
			{
				foreach (DataValue dv in dtype.GetOutboundForeignKeys())
				{
					//Creating the Or Logical filter with the DataType of the aggregate DataObject
					orAux = GetSearchFilter(dv.ValueType, searchedString);

					//Validating the filter is valid
					if (orAux != null)
					{
						//Select all foreign DataObjects that match the criteria and 
						//add them as foreign key filters to the main search
						foreach (DataObject dobj in Select(dv.ValueType, new FilterList(orAux)))
						{
							//Creating the Foreign Key filter and adding criteria to main search
							ForeignKeyFilter fkf = new ForeignKeyFilter(dv, dobj);
							or.InnerFilters.Add(fkf);
						}
					}
				}
			}

			//Querying the Database...
			DataObjectList result = Select(dtype, new FilterList(or), orderBy, dvalues);

			//Searching in the inbound Foreign Keys
			if (deepSearch)
			{
				foreach (DataValue dv in dtype.GetInboundForeignKeys())
				{
					//ommit DataObject generic foreign keys
					if (!DataType.IsDataObjectSubClass(dv.ValueType)) continue;

					//Creating the Or Logical filter with the DataType of the aggregate collection
					orAux = GetSearchFilter(dv.DeclaringDataType, searchedString);

					//Validating the filter is valid
					if (orAux != null)
					{
						//Select all related DataObjects that match the criteria and 
						//add them as foreign key filters to the main search
						foreach (DataObject dobj in Select(dv.DeclaringDataType, new FilterList(orAux)))
						{
							//Loading the current DataObject
							DataObject temp = (DataObject)dobj.GetValue(dv);

							//if null value, continue
							if (temp == null) continue;

							//load temp from database to be sure it is of the corresponding dtype
							DataObjectList tempCollection = Select(dtype, new FilterList(){ new PrimaryKeyFilter(temp) });
							
							//if no object was found, continue
							if (tempCollection.Count == 0) continue;
							
							//get firs result
							temp = tempCollection[0];

							//validating if the DataObjectCollection to return already contains the current DataObject
							if (!result.Contains(temp))
							{
								//adding temp to the result collection
								result.Add(temp);
							}
						}
					}
				}
			}

			//Returning the collection
			if (limit != null)
			{
				DataObjectList range = new DataObjectList();
				range.AddRange(result.GetRange(limit.From, limit.Count));
				return range;
			}
			else
			{
				return result;
			}
		}

		/// <summary>
		/// Load all the DataObjects inherited from the specified DataObject
		/// and whose primary key is equals to the primary key of the specified
		/// DataObject instance
		/// </summary>
		/// <param name="dobj">
		/// DataObject with the primary key used to the search and with the parent
		/// DataType of all the DataObject's loaded
		/// </param>
		/// <param name="nameSpaceFilter">
		/// If its specified, indicates the NameSpace of .net to which must belongs
		/// a Child DataObject for be loaded
		/// </param>
		/// <returns>
		/// DataObjects inherited from the specified DataObject
		/// and whose primary key is equals to the primary key of the specified
		/// DataObject instance
		/// </returns>
		public virtual DataObjectList SearchInheritedFrom(DataObject dobj)
		{
			//Creating Primary Key filter
			PrimaryKeyFilter filter = new PrimaryKeyFilter(dobj);

			//Creating the DataObjectCollection for stores the result
			DataObjectList result = new DataObjectList();

			//Getting all DataTypes inherited from the DataType of the specified DataObject
			List<DataType> childDataTypes = dobj.DataType.GetSubClassDataTypes();

			//Crossing the DataTypes
			foreach (DataType dt in childDataTypes)
			{
				//Loading DataObject's of the current DataType whose
				//Primary key are the same to the specified DataObject
				DataObjectList tempResult = Select(dt, new FilterList(filter));

				//Adding the set of data recently loaded
				result.AddRange(tempResult);
			}

			//Returns the DataObject's
			return result;
		}

		#endregion

		#region Insert

		/// <summary>
		/// Inserts a full DataObject collection into de DataBase
		/// </summary>
		/// <param name="dobjs">
		/// DataObject collection to be inserted
		/// </param>
		public virtual void Insert(DataObjectList dobjs)
		{
			//Validating if the DataObjectCollection is null
			if (dobjs == null)
			{
				throw new ArgumentNullException("dobjs", "Argument cannot be null");
			}

			//Crossing the DataObjects on the collection
			foreach (DataObject dobj in dobjs)
			{
				//Inserting DataObject
				this.Insert(dobj);
			}
		}

		/// <summary>
		/// Inserts a DataObject into the DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be inserted
		/// </param>
		public abstract void Insert(DataObject dobj);

		#endregion

		#region Update

		/// <summary>
		/// Update the entities of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType of the entities to update
		/// </param>
		/// <param name="dvalues">
		/// DataValues that will be updated with it's new values
		/// </param>
		/// <returns>
		/// The total of rows affected by the update
		/// </returns>
		public virtual void Update(DataType dtype, List<DataValueInstance> dvalues)
		{
			//Updating and returning the affected rows...
			this.Update(dtype, dvalues, null);
		}

		/// <summary>
		/// Update the entities of the specified DataType  
		/// that fulfill the indicated filter collection
		/// </summary>
		/// <param name="dtype">
		/// DataType of the entities to update
		/// </param>
		/// <param name="dvalues">
		/// DataValues that will be updated with it's new values
		/// </param>
		/// <param name="filters">
		/// Filters that must fulfill the entities to update (are 
		/// combined with the And Logical Operator)
		/// </param>
		/// <returns>
		/// The total of rows affected by the update
		/// </returns>
		public virtual void Update(DataType dtype, List<DataValueInstance> dvalues, FilterList filters)
		{
			//Validating if the dtype and dataValues arguments are null
			if (dtype == null) throw new ArgumentNullException("dtype");
			if (dvalues == null) throw new ArgumentNullException("dvalues");
			
			//default values
			if (filters == null) filters = new FilterList();

			//Loading the DataObjects to update
			DataObjectList dobjs = this.Select(dtype, filters);

			//create DataValue collection for massive update
			List<DataValue> _values = new List<DataValue>();
			
			//Crossing the DataValues collection
			foreach (DataValueInstance dvi in dvalues)
			{
				//update values in memory for all DataObjects
				foreach (DataObject dobj in dobjs)
				{
					dvi.DataValue.SetValue(dobj, dvi.Value);
				}

				//add to values collection
				_values.Add(dvi.DataValue);
			}

			//massive update
			Update(dobjs, _values);
		}

		/// <summary>
		/// Updates a full DataObject collection
		/// </summary>
		/// <param name="dobjs">
		/// DataObject collection to be updated
		/// </param>
		public virtual void Update(DataObjectList dobjs)
		{
			this.Update(dobjs, null);
		}

		/// <summary>
		/// Updates a full DataObject collection into the DataBase
		/// </summary>
		/// <param name="dobjs">DataObject collection to be updated</param>
		/// <param name="dvalues">List of DataValues that will be updated</param>
		public virtual void Update(DataObjectList dobjs, List<DataValue> dvalues)
		{
			//Validating if the DataObjectCollection is null
			if (dobjs == null) throw new ArgumentNullException("dobjs");
			
			//default values
			if (dvalues == null) dvalues = dobjs.DataType.AllValues;

			//Crossing the DataObjects on the collection
			foreach (DataObject dobj in dobjs)
			{
				//Updating DataObject
				this.Update(dobj, dvalues);
			}
		}

		/// <summary>
		/// Updates a DataObject on the DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be updated
		/// </param>
		public virtual void Update(DataObject dobj)
		{
			Update(dobj, null);
		}

		/// <summary>
		/// Updates a DataObject on the DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be updated
		/// </param>
		/// <param name="dvalues">List of DataValues that will be updated</param>
		public abstract void Update(DataObject dobj, List<DataValue> dvalues);

		#endregion

		#region Delete

		/// <summary>
		/// Deletes a full DataObject collection from the DataBase
		/// </summary>
		/// <param name="dobjs">
		/// DataObject collection to be deleted
		/// </param>
		/// <returns>
		/// Number of rows afected by the operation
		/// </returns>
		public virtual void Delete(DataObjectList dobjs)
		{
			//Validating if the DataObjectCollection is null
			if (dobjs == null) throw new ArgumentNullException("dobjs", "Argument cannot be null");

			//Crossing the DataObjects on the collection
			foreach (DataObject dobj in dobjs)
			{
				//Deleting DataObject
				this.Delete(DataBase.Current.SelectInherited(dobj));
			}
		}

		/// <summary>
		/// Deletes a DataObject from the DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be deleted
		/// </param>
		/// <returns>
		/// Number of rows afected by the operation
		/// </returns>
		public abstract void Delete(DataObject dobj);

		/// <summary>
		/// Delete the entities of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType of the entities to delete
		/// </param>
		/// <returns>
		/// The total of rows affected by the delete
		/// </returns>
		public virtual void Delete(DataType dtype)
		{
			//Deleting and returning the affected rows...
			Delete(dtype, null);
		}

		/// <summary>
		/// Deletes the entities of the specified DataType 
		/// that fulfill the indicated filter collection 
		/// </summary>
		/// <param name="dtype">
		/// DataType of the entities to delete
		/// </param>
		/// <param name="filters">
		/// Filters that must fulfill the entities to delete (are 
		/// combined with the And Logical Operator)
		/// </param>
		/// <returns>
		/// The total of rows affected by the delete
		/// </returns>
		public virtual void Delete(DataType dtype, FilterList filters)
		{
			//Validating if the dtype argument is null
			if (dtype == null) throw new ArgumentNullException("dtype", "Argument cannot be null");
			this.Delete(this.Select(dtype, filters));
		}

		/// <summary>
		/// Deletes a collection of children DataObjects.
		/// Children DataObjects are DataObject with a ForeignKey which value equals dobj
		/// </summary>
		/// <param name="dobj">
		/// DataObject which be used as a filter for searching children
		/// </param>
		/// <param name="childRelatedValue">
		/// DataValue inside a DataType which values will be compared with dobj
		/// </param>
		public void DeleteByForeignKey(DataValue foreignKey, DataObject value)
		{
			ForeignKeyFilter filter;

			//Validating if the arguments are null
			if (foreignKey == null) throw new ArgumentNullException("childRelatedValue", "Argument cannot be null");
			if (value == null) throw new ArgumentNullException("dobj", "Argument cannot be null");

			filter = new ForeignKeyFilter(foreignKey, value);
			Delete(foreignKey.DeclaringDataType, new FilterList(filter));
		}

		/// <summary>
		/// Mark a DataObject as deleted instead of really 
		/// delete it from the database
		/// </summary>
		/// <param name="dobj">
		/// DataObject to mark as deleted
		/// </param>
		/// <returns>
		/// Returns the number of rows affected by the 
		/// Logical Delete
		/// </returns>
		protected void LogicalDelete(DataObject dobj)
		{
			//Loading the DataObject to mark from the Database (this
			//functionality allows load a DataObject know only it primary 
			//key)
			Select(dobj);

			//Establishing the Deleted DataValue to true
			dobj.SetValue("Deleted", true);

			//Updating...
			Update(dobj);
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Returns the number of DataObjects found in the DataBase
		/// </summary>
		/// <param name="dtype">DataType that will be searched in the Database</param>
		/// <returns>Number of DataObjects found in the DataBase</returns>
		public virtual ulong Count(DataType dtype)
		{
			return Count(dtype, null);
		}
		
		/// <summary>
		/// Returns the number of DataObjects found in the DataBase
		/// </summary>
		/// <param name="dtype">DataType that will be searched in the Database</param>
		/// <param name="filters">Optional filters</param>
		/// <returns>Number of DataObjects found in the DataBase</returns>
		public virtual ulong Count(DataType dtype, FilterList filters)
		{
			return (ulong) Select(dtype, filters).Count;
		}

		/// <summary>
		/// Returns the Date and Hour from the database Server
		/// </summary>
		/// <returns>
		/// Date and Hour from the database Server
		/// </returns>
		public virtual DateTime DateTimeOnDBServer()
		{
			return DateTime.Now;
		}

		/// <summary>
		/// Returns a Global Unique Identifier from the Database
		/// </summary>
		/// <returns>
		/// Global Unique Identifier 
		/// </returns>
		public virtual string GetUniqueIdentifier()
		{
			return new Guid().ToString();
		}

		/// <summary>
		/// Performs initial tasks so the current DataBase can perform operations on these DataTypes
		/// </summary>
		/// <param name="dtypes">List of DataTypes that will be supported by the current DataBase</param>
		/// <remarks>This method should only be executed on system setup</remarks>
		public abstract void Setup(List<DataType> dtypes);

		/// <summary>
		/// Verifies that all DataTypes are crreclty setup in the Database
		/// by performing a simple select operation on each one. 
		/// If errors are found, a list of exceptions is returned
		/// </summary>
		/// <param name="dtypes">List of DataTypes that will be verified by the current DataBase</param>
		/// <returns>A list of exceptions found in the current setup, if any</returns>
		/// <remarks>This method should only be executed after a system setup or system update</remarks>
		public virtual List<DataTypeNotSupportedException> VerifySetup(List<DataType> dtypes)
		{
			List<DataTypeNotSupportedException> errors = new List<DataTypeNotSupportedException>();

			//make a simple select to see if all the collumns are defined for each DataType
			foreach (DataType dtype in dtypes)
			{
				try
				{
					Select(dtype);
				}
				catch (Exception e)
				{
					errors.Add(new DataTypeNotSupportedException(dtype, e.Message, e));
				}
			}

			return errors;
		}

		#endregion

		#region Protected methods

		/// <summary>
		/// Return an Or Logical filter with the structure 
		/// "DataValue1 like '%' + filter + '%' or DataValue2 like '%' + filter + '%' or ..."
		/// with all the DataValues of the specified DataType
		/// </summary>
		/// <param name="type">
		/// DataType used to create the filter
		/// </param>
		/// <param name="likePattern">
		/// String used as Like Pattern on the filter
		/// </param>
		/// <returns>
		/// Or Logical filter with the structure 
		/// "DataValue1 like '%' + filter + '%' or DataValue2 like '%' + filter + '%' or ..."
		/// with all the DataValues of the specified DataType
		/// </returns>
		protected OrFilter GetSearchFilter(DataType type, string likePattern)
		{
			//Creating Or Logic Filter
			OrFilter or = new OrFilter();

			//Crossing the DataValues on members of DataType
			foreach (DataValue dv in type.AllValues)
			{
				//LIKE filter for a string value
				if (dv.ValueType.Equals(typeof(string)))
				{
					//Creating Like filter and adding to Or Filter
					or.InnerFilters.Add(new LikeFilter(dv, "%" + likePattern + "%"));
				}

				//Compare filter for a numeric value
				else if (dv.IsNumeric)
				{
					//integral value
					if (dv.ValueType.IsIntegral())
					{
						int pattern;
						if (Int32.TryParse(likePattern, out pattern))
						{
							or.InnerFilters.Add(new ValueCompareFilter(dv, pattern, CompareOperator.Equal));
						}
					}

					//decimal value
					else
					{
						decimal pattern;
						if (decimal.TryParse(likePattern, out pattern))
						{
							or.InnerFilters.Add(new ValueCompareFilter(dv, pattern, CompareOperator.Equal));
						}
					}
				}
			}

			//Establishing Or logical filter to null if dont have inner filters
			if (or.InnerFilters.Count == 0) or = null;

			//Return Or Logical Filter
			return or;
		}

		/// <summary>
		/// Returns the Filter to use for discard logical deleted entities
		/// </summary>
		/// <param name="dtype">
		/// DataType to be query
		/// </param>
		/// <returns>
		/// ValueCompareFilter with the filter criteria
		/// </returns>
		protected ValueCompareFilter GetDeletedFilter(DataType dtype)
		{
			return new ValueCompareFilter((DataValue)dtype["Deleted"], false, CompareOperator.Equal);
		}

		/// <summary>
		/// Returns a value that indicates if must be filtered 
		/// the logical deleted entities of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType to validate
		/// </param>
		/// <returns>
		/// true if the logical deleted entities must be filtered, otherwise false
		/// </returns>
		protected bool MustFilterDeleted(DataType dtype)
		{
			return IsIDeleted(dtype) && !IncludeLogicalDeleted;
		}

		/// <summary>
		/// Retuns a value that indicates if the specified DataType 
		/// implement the logical delete behavior
		/// </summary>
		/// <param name="dtype">
		/// DataType to validate
		/// </param>
		/// <returns>
		/// true if the specified DataType implements IDeleted, otherwise false
		/// </returns>
		protected bool IsIDeleted(DataType dtype)
		{
			return typeof(IDeleted).IsAssignableFrom(dtype.InnerType);
		}

		#endregion

		#region Events

		/// <summary>
		/// Delegate for DataBaseCreated event
		/// </summary>
		public delegate void DataBaseCreatedEventHandler(DataBase sender);

		/// <summary>
		/// Delegate for BeforeSelect event
		/// </summary>
		public delegate void BeforeSelectEventHandler(DataBase sender, SelectEventArgs e);
		
		/// <summary>
		/// Delegate for AfterSelect event
		/// </summary>
		public delegate void AfterSelectEventHandler(DataBase sender, SelectEventArgs e);

		/// <summary>
		/// Delegate for BeforeSelectGroup event
		/// </summary>
		public delegate void BeforeSelectGroupEventHandler(DataBase sender, SelectGroupEventArgs e);

		/// <summary>
		/// Delegate for AfterSelectGroup event
		/// </summary>
		public delegate void AfterSelectGroupEventHandler(DataBase sender, SelectGroupEventArgs e);
		
		/// <summary>
		/// Delegate for insert, delete, and update operation events
		/// </summary>
		public delegate void OperationEventHandler(DataBase sender, DataBaseOperationEventArgs e);

		/// <summary>
		/// Occurs when a new DataBase instance is created in the current application
		/// </summary>
		public static event DataBaseCreatedEventHandler DataBaseCreated;

		/// <summary>
		/// Occurs before a Select operation is performed
		/// </summary>
		public event BeforeSelectEventHandler BeforeSelect;

		/// <summary>
		/// Occurs after a Select operation is performed
		/// </summary>
		public event AfterSelectEventHandler AfterSelect;

		/// <summary>
		/// Occurs before a Select Group operation is performed
		/// </summary>
		public event BeforeSelectGroupEventHandler BeforeSelectGroup;

		/// <summary>
		/// Occurs after a Select Group operation is performed
		/// </summary>
		public event AfterSelectGroupEventHandler AfterSelectGroup;

		/// <summary>
		/// Occurs before a Insert operation is performed
		/// </summary>
		public event OperationEventHandler BeforeInsert;

		/// <summary>
		/// Occurs after a Insert operation is performed
		/// </summary>
		public event OperationEventHandler AfterInsert;

		/// <summary>
		/// Occurs before a Update operation is performed
		/// </summary>
		public event OperationEventHandler BeforeUpdate;

		/// <summary>
		/// Occurs after a Update operation is performed
		/// </summary>
		public event OperationEventHandler AfterUpdate;

		/// <summary>
		/// Occurs before a Delete operation is performed
		/// </summary>
		public event OperationEventHandler BeforeDelete;

		/// <summary>
		/// Occurs after a Delete operation is performed
		/// </summary>
		public event OperationEventHandler AfterDelete;

		/// <summary>
		/// Raises the DataBaseCreated event
		/// </summary>
		protected static void OnDataBaseCreated(DataBase dataBase)
		{
			//Raise the DataBase event
			if (DataBase.DataBaseCreated != null) DataBase.DataBaseCreated(dataBase);
		}

		/// <summary>
		/// Raises the BeforeSelect event. Also, adds filters to affect (or not) logical deleted DataObjects
		/// </summary>
		protected virtual void OnBeforeSelect(SelectEventArgs e)
		{
			//check if DataType is supported
			//if (!SupportedDataTypes.Contains(e.DataType)) throw new DataTypeNotSupportedException(e.DataType);

			//If DataType implements IDeleted and IncludeLogicalDeleted, add filter so logically deleted objects are not retrieved
			if (MustFilterDeleted(e.DataType))
			{
				e.Filters.Add(GetDeletedFilter(e.DataType));
			}

			//Raise the DataBase event
			if (BeforeSelect != null) BeforeSelect(this, e);

			//Raise the DataType event
			e.DataType.OnBeforeSelect(e);
		}

		/// <summary>
		/// Raises the AfterSelect event
		/// </summary>
		protected virtual void OnAfterSelect(SelectEventArgs e)
		{
			//Raise the DataBase event
			if (AfterSelect != null) AfterSelect(this, e);

			//Raise the DataType event
			e.DataType.OnAfterSelect(e);

			//Raise the DataObject event
			foreach (DataObject dobj in e.Result)
			{
				dobj.OnAfterSelect();
			}
		}

		/// <summary>
		/// Raises the BeforeSelect event. Also, adds filters to affect (or not) logical deleted DataObjects
		/// </summary>
		protected virtual void OnBeforeSelectGroup(SelectGroupEventArgs e)
		{
			//If DataType implements IDeleted and IncludeLogicalDeleted, add filter so logically deleted objects are not retrieved
			if (MustFilterDeleted(e.DataType))
			{
				e.Filters.Add(GetDeletedFilter(e.DataType));
			}

			//Raise the DataBase event
			if (BeforeSelectGroup != null) BeforeSelectGroup(this, e);

			//Raise the DataType event
			e.DataType.OnBeforeSelectGroup(e);
		}

		/// <summary>
		/// Raises the AfterSelect event
		/// </summary>
		protected virtual void OnAfterSelectGroup(SelectGroupEventArgs e)
		{
			//Raise the DataBase event
			if (AfterSelectGroup != null) AfterSelectGroup(this, e);

			//Raise the DataType event
			e.DataType.OnAfterSelectGroup(e);
		}

		/// <summary>
		/// Raises the BeforeInsert event
		/// </summary>
		protected virtual void OnBeforeInsert(DataBaseOperationEventArgs e)
		{
			//check if DataType is supported
			//if (!SupportedDataTypes.Contains(e.DataObject.DataType)) throw new DataTypeNotSupportedException(e.DataObject.DataType);

			//Raise the DataBase event
			if (BeforeInsert != null) BeforeInsert(this, e);

			//Raise the DataType event
			e.DataObject.DataType.OnBeforeInsert(e);

			//Raise the DataObject event
			e.DataObject.OnBeforeInsert();
		}

		/// <summary>
		/// Raises the AfterInsert event
		/// </summary>
		protected virtual void OnAfterInsert(DataBaseOperationEventArgs e)
		{
			//Raise the DataObject event
			e.DataObject.OnAfterInsert();

			//Raise the DataType event
			e.DataObject.DataType.OnAfterInsert(e);

			//Raise the DataBase event
			if (AfterInsert != null) AfterInsert(this, e);
		}

		/// <summary>
		/// Raises the BeforeUpdate event
		/// </summary>
		protected virtual void OnBeforeUpdate(DataBaseOperationEventArgs e)
		{
			//check if DataType is supported
			//if (!SupportedDataTypes.Contains(e.DataObject.DataType)) throw new DataTypeNotSupportedException(e.DataObject.DataType);

			//Raise the DataBase event
			if (BeforeUpdate != null) BeforeUpdate(this, e);

			//Raise the DataType event
			e.DataObject.DataType.OnBeforeUpdate(e);

			//Raise the DataObject event
			e.DataObject.OnBeforeUpdate();
		}

		/// <summary>
		/// Raises the AfterUpdate event
		/// </summary>
		protected virtual void OnAfterUpdate(DataBaseOperationEventArgs e)
		{
			//Raise the DataBase event
			if (AfterUpdate != null) AfterUpdate(this, e);

			//Raise the DataType event
			e.DataObject.DataType.OnAfterUpdate(e);

			//Raise the DataObject event
			e.DataObject.OnAfterUpdate();
		}

		/// <summary>
		/// Raises the BeforeDelete event
		/// </summary>
		protected virtual void OnBeforeDelete(DataBaseOperationEventArgs e)
		{
			//check if DataType is supported
			//if (!SupportedDataTypes.Contains(e.DataObject.DataType)) throw new DataTypeNotSupportedException(e.DataObject.DataType);

			//Raise the DataBase event
			if (BeforeDelete != null) BeforeDelete(this, e);

			//Raise the DataType event
			e.DataObject.DataType.OnBeforeDelete(e);

			//Raise the DataObject event
			e.DataObject.OnBeforeDelete();
			
			//Look for inherited objects and delete them
			Delete(this.SearchInheritedFrom(e.DataObject));
		}

		/// <summary>
		/// Raises the AfterDelete event
		/// </summary>
		protected virtual void OnAfterDelete(DataBaseOperationEventArgs e)
		{
			//Raise the DataBase event
			if (AfterDelete != null) AfterDelete(this, e);

			//Raise the DataType event
			e.DataObject.DataType.OnAfterDelete(e);

			//Raise the DataObject event
			e.DataObject.OnAfterDelete();
		}

		#endregion

		#region Transactions

		/// <summary>
		/// Gets a value indicating if a transaction is currently active
		/// </summary>
		public abstract bool IsTransactionActive
		{
			get;
		}

		/// <summary>
		/// Begins a new transaction
		/// </summary>
		public abstract void BeginTransaction();

		/// <summary>
		/// Commits the current transaction
		/// </summary>
		public abstract void CommitTransaction();

		/// <summary>
		/// Rolls back the current transaction
		/// </summary>
		public abstract void RollBackTransaction();

		#endregion

		#region Not yet implemented

		/*
		This methods are not yet finished and should not be used
		
		/// <summary>
		/// Inserts a child DataObject into the DataBase
		/// but a parent DataObject must be already registered
		/// </summary>
		/// <param name="dobj">
		/// Child DataObject to be inserted
		/// </param>
		/// <remarks>
		/// Usefull to create a child object when the base object already exist.
		/// For example, you have a "Person" that is already registered, but you want to convert
		/// that Person into a "User" (Person is the base class and User inherits from Person).
		/// Use this method to create the User without the need of previously deleting the Person
		/// Only the higher level DataType is inserted.
		/// </remarks>
		public abstract void InsertChild(DataObject dobj);

		/// <summary>
		/// Deletes a child DataObject into from the DataBase
		/// but without deleting the parent DataObject
		/// </summary>
		/// <param name="dobj">
		/// Child DataObject to be deleted
		/// </param>
		/// <remarks>
		/// Usefull to delete a child object without deleting the base object.
		/// For example, you have a "User" already registered, but you want to convert
		/// that User into a "Person" (Person is the base class and User inherits from Person).
		/// Use this method to delete the User without the need of deleting the Person.
		/// Only the higher level DataType is deleted.
		/// </remarks>
		public abstract void DeleteChild(DataObject dobj);
		 
		*/

		#endregion

		#region IXmlSerializable Members

		/// <summary>
		/// Following online help recommendations, this method allways return null
		/// </summary>
		public virtual System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Deserialize the specified Xml into the current instance
		/// </summary>
		/// <param name="reader">
		/// XmlReader used to read the xml DataBase representation
		/// </param>
		public virtual void ReadXml(System.Xml.XmlReader reader)
		{
		}

		/// <summary>
		/// Serialize the current DataBase into a xml writer
		/// </summary>
		/// <param name="writer">
		/// XmlWriter used for the serialization
		/// </param>
		public virtual void WriteXml(System.Xml.XmlWriter writer)
		{
		}

		#endregion
	}
}
