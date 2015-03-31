using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using OKHOSTING.Code.ORM;
using OKHOSTING.Code.ORM.Filters;
using OKHOSTING.Code.ORM.Sql.Filters;
using OKHOSTING.Code.ORM.Validators;

namespace OKHOSTING.Code.ORM
{ 
	/// <summary>
	/// Represents an on memory Database
	/// </summary>
	public class DataSet : RelationalDataBase
	{
		#region Properties

		/// <summary>
		/// Internal DataSet object that contains the data on memory
		/// </summary>
		private System.Data.DataSet InnerDataSet;

		/// <summary>
		/// File name of the DataSet serialized on hard disk
		/// </summary>
		private string FileName;

		/// <summary>
		/// Indicates if the changes on the DataSet, will be saved automatically 
		/// when the class be destroyed
		/// </summary>
		public bool AutoSave;

		#endregion

		#region Constructors and destructors

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <remarks>This constructor is only used for serialization</remarks>
		private DataSet() 
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="fileName">
		/// File name of the DataSet serialized on hard disk
		/// </param>
		public DataSet(string fileName) : this(fileName, true) { }
		
		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="fileName">
		/// File name of the DataSet serialized on hard disk
		/// </param>
		/// <param name="autoSave">
		/// Indicates if the changes on the DataSet, will be saved automatically 
		/// when the class be destroyed
		/// </param>
		public DataSet(string fileName, bool autoSave)
		{
			//Initializing class internal data
			this.FileName = fileName;
			this.AutoSave = autoSave;

			//Loading the DataSet from disk
			Load();
		}

		/// <summary>
		/// Destroy the class
		/// </summary>
		~DataSet()
		{
			//Saving the DataSet if apply
			if (this.AutoSave) Save();
		}

		#endregion

		#region Load and save

		/// <summary>
		/// Loads the specified file name on the DataSet
		/// </summary>
		private void Load()
		{
			//initiate InnerDataSet
			if(InnerDataSet == null) InnerDataSet = new System.Data.DataSet();

			//Validating if the specified file exists
			if (System.IO.File.Exists(FileName))
			{
				//Reading the file
				InnerDataSet.ReadXml(FileName);
			}
			else
			{
				//Creating the file on hard disk
				System.IO.File.Create(FileName).Close();
			}
		}

		/// <summary>
		/// Save the changes in the DataSet to the hard disk
		/// </summary>
		public void Save()
		{
			//Writing the DataSet to disk
			InnerDataSet.WriteXml(FileName, XmlWriteMode.WriteSchema);
		}

		#endregion

		#region Select

		/// <summary>
		/// Returns a DataObjectCollection with the DataObjects of the specified type
		/// that fulfills the specified filters
		/// </summary>
		/// <param name="dtype">
		/// DataType of DataObjects to load
		/// </param>
		/// <param name="filters">
		/// Filters that must be fulfilled for the Loaded DataObjects
		/// </param>
		/// <param name="orderBy">
		/// DataValues that will be used for sorting the result
		/// </param>
		/// <returns>
		/// Collection of DataObjects that match the filter criteria, ordered by the specified orderBy list
		/// </returns>
		public override DataObjectList Select(DataType dtype, FilterList filters, List<OrderByItem> orderBy, List<DataValue> dvalues, SelectLimit limit)
		{
			//Validating if the dtype argument is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Initializing filters and orderby
			if (filters == null) filters = new FilterList();
			if (orderBy == null) orderBy = new List<OrderByItem>();
			if (dvalues == null) dvalues = dtype.AllValues;
			
			//Creating the DataObject collection to return
			DataObjectList result = new DataObjectList(dtype);
			result.DataValues = dvalues;

			//Call events
			SelectEventArgs e;
			e = new SelectEventArgs(dtype, filters, orderBy);
			OnBeforeSelect(e);

			//If e.Cancel is true, cancell the operation for custom processing
			if (!e.Cancel)
			{
				//Saving the original DataType for posterior use
				DataType originalDataType = dtype;

				//Loading the DataTable associated to specified DataType
				DataTable table = InnerDataSet.Tables[dtype.Name];

				//Validating if the table was founded
				if (table == null) throw new ArgumentException("DataType " + dtype + "is not implemented in this DataSet");

				//Crossing the rows on the Entity DataTable
				foreach (DataRow row in table.Rows)
				{
					//TODO: Implement SelectLimit, for now, parameter is ignored

					//Creating new DataObject from DataType
					DataObject dobj = DataObject.From(dtype);

					//Initializing DataObject
					dobj.InitForeignKeys();

					//Initializing the new DataObject with the current Row
					this.ToDataObject(row, dtype, dvalues, dobj);

					//Adding DataObject to collection to return
					result.Add(dobj);
				}

				//Crossing the Parent DataTypes of the root DataType
				dtype = dtype.BaseDataType;
				while (dtype != null)
				{
					//Crossing the loaded DataObjects
					foreach (DataObject dobj in result)
					{
						//Loading the related DataRow on the parent DataType
						DataRow row = this.FindDataRow(dobj, dtype);

						//Updating the DataObject with the DataRow data, if the 
						//DataRow was founded
						if (row != null) this.ToDataObject(row, dtype, dvalues, dobj);
					}

					//Loading the next parent
					dtype = dtype.BaseDataType;
				}

				//Restoring the original DataType
				dtype = originalDataType;

				//Loading the DataObject collection...
				result = result.Select(filters);

				//Sorting the collection only using the first order by item, 
				//TODO: multiple order by items are not yet supported
				if (orderBy != null && orderBy.Count > 0) result.Sort(orderBy[0]);
			}

			//Call events
			e.Result = result;
			OnAfterSelect(e);

			//Returning DataObject Collection
			return result;
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
		public override DataTable SelectGroup(DataType dtype, List<AggregateSelectField> aggregateSelectFields, List<DataValue> dataValuesToGroup, FilterList filters, List<OrderByItem> orderBy)
		{
			throw new NotImplementedException();

			//pending implementation
			//InnerDataSet.Tables[dtype.TableName].Compute("Expression", "Filter");
		}
		
		#endregion

		#region Inserts

		/// <summary>
		/// Inserts a DataObject into de DataBase
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be inserted
		/// </param>
		/// <returns>
		/// Number of rows afected by the operation
		/// </returns>
		public override void Insert(DataObject dobj)
		{
			//Call events
			DataBaseOperationEventArgs e;
			e = new DataBaseOperationEventArgs(DataBaseOperation.Insert, dobj);
			OnBeforeInsert(e);

			//If e.Cancel is true, cancell the operation for custom processing
			if (!e.Cancel)
			{

				//Getting the root DataType of the DataObject
				DataType dtype = dobj.DataType;

				//Crossing all the types in the hierarchy of the DataObject
				while (dtype != null)
				{
					//Finding the table of the DataObject
					DataTable table = InnerDataSet.Tables[dtype.Name];

					//Validating if the table was founded
					if (table == null) throw new ArgumentException("DataType " + dtype + "is not implemented in this DataSet");

					//Creating the new row and appending to the table
					DataRow row = table.NewRow();
					this.ToDataRow(dobj, dtype, dtype.AllValues, row);
					table.Rows.Add(row);

					//Getting the reference to the next object in the hierarchy
					dtype = dtype.BaseDataType;
				}
			}

			//Call events
			OnAfterInsert(e);
		}

		#endregion

		#region Update

		/// <summary>
		/// Update the specified DataObject
		/// </summary>
		/// <param name="dobj">
		/// DataObject to update
		/// </param>
		/// <param name="dvalues">Values that will be updated</param>
		public override void Update(DataObject dobj, List<DataValue> dvalues)
		{
			//Validating if the dobj argument is null
			if (dobj == null) throw new ArgumentNullException("dobj");

			//default values
			if (dvalues == null) dvalues = dobj.DataType.GetValues(false);

			//Call events
			DataBaseOperationEventArgs e;
			e = new DataBaseOperationEventArgs(DataBaseOperation.Update, dobj, dvalues);
			OnBeforeUpdate(e);

			//If e.Cancel is true, cancell the operation for custom processing
			if (!e.Cancel)
			{
				//Establishing the DataType to root DataObject DataType
				DataType dtype = dobj.DataType;

				//Crossing all the DataObject DataTypes
				while (dtype != null)
				{
					//Finding the DataRow to edit...
					DataRow row = this.FindDataRow(dobj, dtype);

					//Validating if the row was founded
					if (row != null)
					{
						//Updating using the specified DataObject...
						this.ToDataRow(dobj, dtype, dvalues, row);
					}

					//Loading next parent DataType
					dtype = dtype.BaseDataType;
				}
			}

			//Call events
			OnAfterUpdate(e);
		}

		#endregion

		#region Delete

		/// <summary>
		/// Delete the specified DataObject
		/// </summary>
		/// <param name="dobj">
		/// DataObject to delete
		/// </param>
		public override void Delete(DataObject dobj)
		{
			//Validating if the dobj argument is null
			if (dobj == null) throw new ArgumentNullException("dobj");

			//if there are any child DataObjects, delete them instead, recursively
			DataObjectList childs = SearchInheritedFrom(dobj);
			if (childs.Count > 0)
			{
				foreach (DataObject child in childs)
				{
					Delete(child);
				}

				return;
			}

			//Call events
			DataBaseOperationEventArgs e;
			e = new DataBaseOperationEventArgs(DataBaseOperation.Delete, dobj);
			OnBeforeDelete(e);

			//If e.Cancel is true, cancell the operation for custom processing
			if (!e.Cancel)
			{
				//Validating if the delete, its a logical Delete
				if (IsIDeleted(dobj.DataType))
				{
					//Performing the logical delete...
					base.LogicalDelete(dobj);
				}
				else
				{
					//Physical deletion; Finding the rows to delete...
					foreach (DataRow row in this.FindDataRows(dobj))
					{
						//Deleting the row and increasing the rows affected counter
						row.Delete();
					}
				}
			}

			//Call events
			OnAfterDelete(e);
		}

		#endregion

		#region Data Definition

		/// <summary>
		/// Create the Database table for the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for Table Creation
		/// </param>
		/// <param name="inherits">
		/// Indicates if do you want to create too, the parent DataTypes
		/// of the dtype argument
		/// </param>
		public override void Create(DataType dtype, bool inherits)
		{
			//Validating if the dtype argument is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Validating if the parent DataTypes will be created too
			if (inherits)
			{
				//Crossing the parent types
				while (dtype != null)
				{
					//Creating the table for the type
					SingleTypeCreate(dtype);

					//Loading next parent DataType
					dtype = dtype.BaseDataType;
				}
			}
			else
			{
				//Creating the table for the type
				SingleTypeCreate(dtype);
			}
		}

		/// <summary>
		/// Drop the Database table for the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for Table Creation
		/// </param>
		/// <param name="inherits">
		/// Indicates if do you want to drop too, the parent DataTypes
		/// of the dtype argument
		/// </param>
		public override void Drop(DataType dtype, bool inherits)
		{
			//Validating if the dtype argument is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Validating if the parent DataTypes will be droped too
			if(inherits)
			{
				//Crossing the parent types
				while(dtype!=null)
				{
					//Removing the DataTable...
					InnerDataSet.Tables.Remove(dtype.Name);

					//Loading next parent DataType
					dtype = dtype.BaseDataType;
				}
			}
			else
			{
				//Removing the DataTable...
				InnerDataSet.Tables.Remove(dtype.Name);
			}
		}

		/// <summary>
		/// Create the indexes for the table of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for index creation
		/// </param>
		/// <param name="inherits">
		/// Indicates if do you want to create too, the parent DataTypes 
		/// indexes of the dtype argument
		/// </param>
		public override void CreateIndexes(DataType dtype, bool inherits)
		{
			//Validating if the dtype argument is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Validating if the parent DataTypes must be affected
			if (inherits)
			{
				//Crossing the parent types
				while (dtype != null)
				{
					//Creating indexes...
					CreateIndexes(dtype);

					//Loading next parent DataType
					dtype = dtype.BaseDataType;
				}
			}
			else
			{
				//Creating indexes...
				CreateIndexes(dtype);
			}
		}

		/// <summary>
		/// Create the indexes for the table of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for index creation
		/// </param>
		/// <param name="inherits">
		/// Indicates if do you want to create too, the parent DataTypes 
		/// indexes of the dtype argument
		/// </param>
		public void CreateIndexes(DataType dtype)
		{
			//Getting the DataTable
			DataTable tblDataType = InnerDataSet.Tables[dtype.Name];

			//Loading the indexes defined on DataType
			object[] indexes = dtype.InnerType.GetCustomAttributes(typeof(IndexAttribute), true);

			//Crossing the indexes defined
			for (int indexCounter = 0; indexCounter < indexes.GetLength(0); indexCounter++)
			{
				//Getting the index
				IndexAttribute currentIndex = (IndexAttribute)indexes[indexCounter];

				//Adding the index (Only if unique)
				if (currentIndex.Unique)
					tblDataType.Constraints.Add(new UniqueConstraint(currentIndex.Name, currentIndex.DataValues, false));
			}
		}

		/// <summary>
		/// Create the foreign keys for the table of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for foreign keys creation
		/// </param>
		public override void CreateForeignKeys(DataType dtype, bool inherits)
		{
			//Getting the DataTable
			DataTable tblDataType;

			//Validating if dtype is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Validating if must create too, the parent DataTypes of the dtype argument
			if (inherits)
			{
				//Crossing the parent DataTypes of dtype
				while (dtype != null)
				{
					tblDataType = InnerDataSet.Tables[dtype.TableName];

					//Creating foreign keys
					foreach (DataValue dvalue in dtype.ForeignKeys)
					{
						//list of local columns
						DataValueAtomizedDictionary localColumns = new DataValueAtomizedDictionary();
						localColumns.Add(dvalue);
						List<string> localColumnsList = new List<string>();
						foreach (KeyValuePair<string, DataValue> column in localColumns)
						{
							localColumnsList.Add(column.Key);
						}

						//list of foreign columns
						DataType foreignDataType = dvalue.ValueType;
						DataValueAtomizedDictionary foreignColumns = new DataValueAtomizedDictionary();
						foreignColumns.AddRange(foreignDataType.PrimaryKey);
						List<string> foreignColumnsList = new List<string>();
						foreach (KeyValuePair<string, DataValue> column in localColumns)
						{
							foreignColumnsList.Add(column.Key);
						}

						//Adding the Foreign key relation
						tblDataType.Constraints.Add
							(
							new ForeignKeyConstraint
								(
								"FK_" + dvalue.ColumnName,
								dtype.Name,
								foreignColumnsList.ToArray(),
								localColumnsList.ToArray(),
								AcceptRejectRule.None,
								Rule.None,
								Rule.None
								)
							);
					}

					//Getting the next parent DataType
					dtype = dtype.BaseDataType;
				}
			}
			else
			{
				tblDataType = InnerDataSet.Tables[dtype.TableName];

				//Creating foreign keys
				foreach (DataValue dvalue in dtype.ForeignKeys)
				{
					//list of local columns
					DataValueAtomizedDictionary localColumns = new DataValueAtomizedDictionary();
					localColumns.Add(dvalue);
					List<string> localColumnsList = new List<string>();
					foreach (KeyValuePair<string, DataValue> column in localColumns)
					{
						localColumnsList.Add(column.Key);
					}

					//list of foreign columns
					DataType foreignDataType = dvalue.ValueType;
					DataValueAtomizedDictionary foreignColumns = new DataValueAtomizedDictionary();
					foreignColumns.AddRange(foreignDataType.PrimaryKey);
					List<string> foreignColumnsList = new List<string>();
					foreach (KeyValuePair<string, DataValue> column in localColumns)
					{
						foreignColumnsList.Add(column.Key);
					}
					
					//Adding the Foreign key relation
					tblDataType.Constraints.Add
						(
						new ForeignKeyConstraint
							(
							"FK_" + dvalue.ColumnName,
							dtype.Name,
							foreignColumnsList.ToArray(),
							localColumnsList.ToArray(),
							AcceptRejectRule.None,
							Rule.None,
							Rule.None
							)
						);
				}
			}
		}

		/// <summary>
		/// Returns a boolean value that determines if the Table
		/// associated to the specified DataType exists
		/// </summary>
		/// <param name="dtype">
		/// DataType for table validation
		/// </param>
		/// <returns>
		/// true if the database table for the specified DataType exists,
		/// otherwise false
		/// </returns>
		public override bool TableExists(DataType dtype)
		{
			return this.InnerDataSet.Tables.Contains(dtype.TableName);
		}

		/// <summary>
		/// Returns a value that indicates if the Constrainst 
		/// exists in the Underlying Database 
		/// </summary>
		/// <param name="Name">
		/// Name of the constraint to validate
		/// </param>
		/// <returns>
		/// Value that indicates if the Constrainst 
		/// exists in the Underlying Database 
		/// </returns>
		public override bool ConstraintExists(string Name)
		{
			//Local vars
			string table, constraint;
			bool result = false;

			//Validating if the name is correctly specified
			if (Name.IndexOf(".") == -1)
				throw new ArgumentException("For DataSet constraints, the constraint name must be completly qualified with the syntax Table.Constraint", "Name");

			//Desglosando el nombre del índice en tabla e indice
			table = Name.Substring(0, Name.IndexOf("."));
			constraint = Name.Substring(Name.IndexOf(".") + 1);

			//Getting the table
			DataTable tblData = InnerDataSet.Tables[table];

			//Validating if the table exists
			if (tblData != null)
			{
				//Validating if the constraint exists
				result = tblData.Constraints.Contains(constraint);
			}

			//Retrieving the result
			return result;
		}

		/// <summary>
		/// Verify if exists the specified index on the Database
		/// </summary>
		/// <param name="Name">
		/// Name of the index
		/// </param>
		/// <returns>
		/// Boolean value that indicates if exists 
		/// the specified index on the Database
		/// </returns>
		public override bool IndexExists(string Name)
		{
			throw new NotSupportedException("The method DataBase.IndexExists() is not supported on DataSet subclass");
		}

		#endregion 

		#region Support methods

		/// <summary>
		/// Creates a new DataTable on the Database for the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for table creation
		/// </param>
		/// <returns>
		/// The DataTable recently created
		/// </returns>
		protected virtual DataTable SingleTypeCreate(DataType dtype)
		{
			//Creating the new DataTable
			DataTable table = new DataTable(dtype.TableName);

			//Creating collection for definition of primary key
			List<DataColumn> primaryKey = new List<DataColumn>();

			//Loading the DataValues of the DataType (only local members for the type with it primary key)
			DataValueAtomizedDictionary members;
			members = new DataValueAtomizedDictionary();
			members.AddRange(dtype.PrimaryKey);
			members.AddRange(dtype.RegularValues);

			//Crossing the DataValues of the type
			foreach (KeyValuePair<string, DataValue> dv in members)
			{
				//Loading the validators of the DataValue
				DataValueValidator[] validators = DataValueValidator.GetValidators(dv.Value);

				//Creating the DataColumn corresponding to the DataValue
				DataColumn column = new DataColumn(dv.Key, dv.Value.ValueType);
				column.AllowDBNull = true;

				//Validating length restrictions (only for DataValues of string type)
				if (dv.Value.ValueType.Equals(typeof(string)))
				{
					//Crossing the validators on search for a StringLengthValidator
					for (int i = 0; i < validators.Length; i++)
					{
						//Validating if the current validator is a StringLengthValidator
						if (validators[i] is StringLengthValidator)
						{
							//Loading the StringLengthValidator
							StringLengthValidator val = (StringLengthValidator)validators[i];

							//Defining the column MaxLength property
							if (val.Operator == CompareOperator.LessThan)
							{
								column.MaxLength = (int)val.Length - 1;
							}

							else if (val.Operator == CompareOperator.LessThanEqual || val.Operator == CompareOperator.Equal)
							{
								column.MaxLength = (int)val.Length;
							}

							//Leaving the cycle
							break;
						}
					}
				}

				//Validating if the current DataValue it's a primary key; 
				//if it's, establishing a NOT NULL restriction for the column
				if (dv.Value.IsPrimaryKey)
				{
					//Establishing a NOT NULL restriction for the column
					column.AllowDBNull = false;

					//Adding column to primary key collection
					primaryKey.Add(column);
				}

				//Else, search for an AllowNullValidator, and if exist, establishing a [NOT] NULL restriction
				else if (DataValueValidator.IsImplemented(typeof(Required), dv.Value))
				{
					//Search for a AllowNullValidator
					for (int i = 0; i < validators.Length; i++)
					{
						//Validating if the current validator is of type AllowNullValidator
						if (validators[i] is Required)
							//Establishing the restriction...
							column.AllowDBNull = true;
					}
				}

				//Adding the column to DataTable
				table.Columns.Add(column);
			}

			//Establishing the primary key for the table
			table.PrimaryKey = primaryKey.ToArray();

			//Adding the DataTable to inner Dataset
			InnerDataSet.Tables.Add(table);

			//Returning the table to the caller
			return table;
		}
		
		/// <summary>
		/// Find the DataRow with the specified Primary key on the
		/// indicated DataType
		/// </summary>
		/// <param name="dobj">
		/// DataObject that defines the Primary key searched
		/// </param>
		/// <param name="dtype">
		/// DataType of the table for search
		/// </param>
		/// <returns>
		/// The requeste DataRow or null if this is not founded
		/// </returns>
		protected DataRow FindDataRow(DataObject dobj, DataType dtype)
		{
			//Creating primary key filter
			SqlPrimaryKeyFilter filter = new SqlPrimaryKeyFilter(dobj, new DataSetGenerator());
			filter.UseCompletelyQualifiedIdentifiers = false;

			//Loading the DataRows that match with the filter
			DataRow[] rows = InnerDataSet.Tables[dtype.Name].Select(filter.GetSqlFilter());

			//Returning the first row founded, or null if not exist
			if (rows.Length > 0)
				return rows[0];
			else
				return null;
		}

		/// <summary>
		/// Find all DataRows of the specified DataObject (in all his DataTypes)
		/// </summary>
		/// <param name="dobj">
		/// DataObject whose DataRows will be searched
		/// </param>
		/// <returns>
		/// Array of DataRows founded
		/// </returns>
		protected DataRow[] FindDataRows(DataObject dobj)
		{
			//Loading the DataObject DataType
			DataType dtype = dobj.DataType;

			//Creating the primary key filter
			SqlPrimaryKeyFilter sqlPrimaryKeyFilter = new SqlPrimaryKeyFilter(dobj, new DataSetGenerator());
			sqlPrimaryKeyFilter.UseCompletelyQualifiedIdentifiers = false;
			string filter = sqlPrimaryKeyFilter.GetSqlFilter();

			//Creating temporary array for the procedure
			List<DataRow> tempRows = new List<DataRow>();

			//Crossing all DataObjects related DataTypes
			while (dtype != null)
			{
				//Finding rows on the table associated to the current DataType
				DataRow[] temp = InnerDataSet.Tables[dtype.Name].Select(filter);

				//Adding the first item founded (if exists)
				if (temp.Length > 0) tempRows.Add(temp[0]);

				//Loading the next DataType
				dtype = dtype.BaseDataType;
			}

			//Returning the array of rows
			return tempRows.ToArray();
		}

		/// <summary>
		/// Converts the specified DataRow on its respective DataObject instance
		/// </summary>
		/// <param name="row">
		/// DataRow to convert
		/// </param>
		/// <param name="dtype">
		/// DataType whose members are loaded for this conversion
		/// </param>
		/// <param name="dobj">
		/// DataObject for conversion. This argument contains as input the DataObject
		/// structure of the DataObject table, but is used too as output, for
		/// return the result of the conversion
		/// </param>
		protected void ToDataObject(DataRow row, DataType dtype, List<DataValue> dvalues, DataObject dobj)
		{
			//Validating if row, dobj or dtype arguments are null
			if (row == null) throw new ArgumentNullException("row");
			if (dobj == null) throw new ArgumentNullException("dobj");
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Loading all the members that not are part of the primary key
			DataValueInstanceAtomizedDictionary properties;
			properties = new DataValueInstanceAtomizedDictionary();
			foreach (DataValue dv in dvalues)
			{
				if (dv.DeclaringDataType.Equals(dtype) && !dv.IsPrimaryKey) properties.Add(dobj[dv.Name]);
			}
			
			//If it's not the base type, add primary key members
			if (dtype.BaseDataType != null)
			{
				properties.AddRange(dobj.PrimaryKey);
			}
			
			//Crossing the DataValues of the DataObject
			foreach(KeyValuePair<string, DataValueInstance> dvi in properties)
			{
				//Getting the valueof the current DataValue
				object val = row[dvi.Key];

				//Validating if the value to assing is null...
				if (NullValues.IsNull(val))
				{
					dvi.Value.Value = TypeConverter.ChangeType(NullValues.GetNullValue(dvi.Value.DataValue.ValueType), dvi.Value.DataValue.ValueType);
				}
				else
				{
					dvi.Value.Value = TypeConverter.ChangeType(val, dvi.Value.DataValue.ValueType);
				}
			}
		}
	
		/// <summary>
		/// Converts the specified DataObject instance on his respective DataRow
		/// </summary>
		/// <param name="dobj">
		/// DataObject to convert
		/// </param>
		/// <param name="dtype">
		/// DataType whose members are loaded for this conversion
		/// </param>
		/// <param name="row">
		/// DataRow for conversion. This argument contains as input the DataRow
		/// structure of the DataObject table, but is used too as output, for
		/// return the result of the conversion
		/// </param>		
		protected void ToDataRow(DataObject dobj, DataType dtype, List<DataValue> dvalues, DataRow row)
		{
			//Validating if the dobj, dtype or row arguments are null
			if (dobj == null) throw new ArgumentNullException("dobj");
			if (dtype == null) throw new ArgumentNullException("dtype");
			if (dvalues == null) throw new ArgumentNullException("dvalues");
			if (row == null) throw new ArgumentNullException("row");

			//Loading all the members of the type
			DataValueInstanceAtomizedDictionary properties;
			properties = new DataValueInstanceAtomizedDictionary();
			foreach (DataValue dv in dvalues)
			{
				if (dv.DeclaringDataType.Equals(dtype) && !dv.IsPrimaryKey) properties.Add(dobj[dv.Name]);
			} 

			//If it's not the base type, add primary key members
			if (dtype.BaseDataType != null)
			{
				properties.AddRange(dobj.PrimaryKey);
			}
			
			//Starting the Edit on the Row
			row.BeginEdit();

			//Crossing the DataValue's...
			foreach(KeyValuePair<string, DataValueInstance> dvi in properties)
			{
				//Loading the value of current DataValue
				object val = dvi.Value.Value;

				//Assigning the value to the row
				if (!NullValues.IsNull(val))
					row[dvi.Key] = val;
				else
					row[dvi.Key] = NullValues.GetNullValue(dvi.Value.DataValue.ValueType);
			}

			//Ending the Edit on the Row
			row.EndEdit();
		}

		/// <summary>
		/// Converts a value to it's database-specific string representantion, so it can be included in a SQL script
		/// </summary>
		/// <param name="value">
		/// Value to be converted to string
		/// </param>
		/// <returns>
		/// A database-specific string representation of value
		/// </returns>
		public string Format(System.DateTime value)
		{
			return "#" + value.ToShortDateString() + "#";
		}

		#endregion

		#region DataSetGenerator

		/// <summary>
		/// Provides formatting methods for XML DataSet DataBases
		/// </summary>
		public class DataSetGenerator : OKHOSTING.Code.ORM.Sql.Generators.AnsiSqlGenerator
		{
			/// <summary>
			/// Initializes a new instance of the OKHOSTING.DataBases.DataTableFormatProvider class
			/// </summary>
			public DataSetGenerator()
			{
			}

			/// <summary>
			/// Converts a value to it's database-specific string representantion, so it can be included in a SQL script
			/// </summary>
			/// <param name="value">
			/// Value to be converted to string
			/// </param>
			/// <returns>
			/// A database-specific string representation of value
			/// </returns>
			public override string Format(System.DateTime value)
			{
				return "#" + value.ToShortDateString() + "#";
			}


			/// <summary>
			/// Not implemented
			/// </summary>
			public override string AutoIncrementalSettingName
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			/// <summary>
			/// Not supported by this generator
			/// </summary>
			public override string GetAutoIncrementalRetrieveFunction(DataType dtype)
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region Transactions

		/// <summary>
		/// Gets a value indicating if a transaction is currently active
		/// </summary>
		public override bool IsTransactionActive
		{
			get 
			{
				//always returns true
				return true;
			}
		}

		/// <summary>
		/// Begins a new transaction
		/// </summary>
		public override void BeginTransaction()
		{
			//do nothing
		}

		/// <summary>
		/// Commits the current transaction
		/// </summary>
		public override void CommitTransaction()
		{
			InnerDataSet.AcceptChanges();
		}

		/// <summary>
		/// Rolls back the current transaction
		/// </summary>
		public override void RollBackTransaction()
		{
			InnerDataSet.RejectChanges();
		}
		
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
		public override void InsertChild(DataObject dobj)
		{
			//Finding the table of the DataObject
			DataTable table = innerDataSet.Tables[dobj.DataType.Name];

			//Validating if the table was founded
			if (table == null) throw new ArgumentException("DataType " + dobj.DataType + "is not implemented in this DataSet");

			//Creating the new row and appending to the table
			DataRow row = table.NewRow();
			this.ToDataRow(dobj, dobj.DataType, dobj.DataType.AllValues, row);
			table.Rows.Add(row);
		}

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
		public override void DeleteChild(DataObject dobj)
		{
			//Validating if the dobj argument is null
			if (dobj == null) throw new ArgumentNullException("dobj");

			//Finding the row to delete...
			FindDataRow(dobj, dobj.DataType).Delete();
		}
		 
		*/

		#endregion

		#region IXmlSerializable Members

		/// <summary>
		/// Deserialize the specified Xml into the current instance
		/// </summary>
		/// <param name="reader">
		/// XmlReader used to read the xml DataBase representation
		/// </param>
		public override void ReadXml(System.Xml.XmlReader reader)
		{
			//Reading the file name
			reader.MoveToAttribute("FileName");
			FileName = reader.Value;

			//Reading the file name
			reader.MoveToAttribute("AutoSave");
			AutoSave = bool.Parse(reader.Value);
			
			//load dataset
			Load();

			//move to the next xml node
			reader.MoveToElement();
			reader.Read();
		}

		/// <summary>
		/// Serialize the current DataBase into a xml writer
		/// </summary>
		/// <param name="writer">
		/// XmlWriter used for the serialization
		/// </param>
		public override void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("FileName", FileName);
			writer.WriteAttributeString("AutoSave", AutoSave.ToString());
		}

		#endregion
	}
}