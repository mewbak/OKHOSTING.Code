using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using OKHOSTING.Tools;
using OKHOSTING.Tools.Extensions;
using OKHOSTING.Code.ORM.Validators;
using OKHOSTING.Code.ORM.Sql.Generators;

namespace OKHOSTING.Code.ORM
{ 
	/// <summary>
	/// Represents an object that can be stored in a database or serialized as xml, 
	/// the center piece of all OKHOSTING DataObjects libraries
	/// </summary>
	/// <remarks>
	/// If you want your object to be stored in a database 
	/// or as xml you must inherit this class
	/// </remarks>
	[Serializable]
	public abstract class DataObject : IXmlSerializable, IComparable, IStringSerializable
	{
		#region Constructors

		/// <summary>
		/// Initializes and populates the object, it's type and it Values collections
		/// </summary>
		protected DataObject()
		{
			//Creating the DataType associated to this DataObject (Throught custom explicit conversión)
			this.dtype = (DataType)this.GetType();

			//Initializing Data Values
			AllValues = new List<DataValueInstance>();
			foreach (DataValue dv in this.DataType.AllValues)
			{
				//Adding to the collection
				this.AllValues.Add(new DataValueInstance(this, dv));
			}

			//Initializing to default values
			this.Init();
		}

		#endregion

		#region Fields

		/// <summary>
		/// Associated DataType with metadata info of the DataObject
		/// </summary>
		DataType dtype;

		/// <summary>
		/// Indicates wether the DataObject is saved in the DataBase or not
		/// </summary>
		bool isSaved = false;
		
		/// <summary>
		/// Indicates wether the DataObject has been selected (loaded) from the DataBase
		/// </summary>
		bool isSelected = false;

		#endregion

		#region Public Properties

		/// <summary>
		/// Returns all DataValues that exists in this DataObject
		/// </summary>
		public readonly List<DataValueInstance> AllValues;

		/// <summary>
		/// Returns a List<DataValueInstance> containing all DataValues that conform the primary key
		/// </summary>
		public List<DataValueInstance> PrimaryKey
		{
			get
			{
				return GetValues(true);
			}
		}

		/// <summary>
		/// Returns a List&lt;DataValueInstance&gt; containing all DataValues that are regular values (not primary key and not collection)
		/// </summary>
		public List<DataValueInstance> RegularValues
		{
			get
			{
				return GetValues(false);
			}
		}

		/// <summary>
		/// Returns a list containing all DataValueInstances that are foreign keys
		/// </summary>
		public List<DataValueInstance> ForeignKeys
		{
			get
			{
				List<DataValueInstance> fk;

				fk = new List<DataValueInstance>();
				foreach (DataValueInstance dvalue in AllValues)
				{
					if (dvalue.DataValue.IsForeignKey) fk.Add(dvalue);
				}

				return fk;
			}
		}

		/// <summary>
		/// Returns the current instance's DataType
		/// </summary>
		public DataType DataType
		{
			get
			{
				return dtype;
			}
		}

		/// <summary>
		/// Indicates wether the DataObject is saved in the DataBase or not
		/// </summary>
		public bool IsSaved
		{
			get
			{
				return isSaved;
			}
		}

		/// <summary>
		/// Indicates wether the DataObject has been selected (loaded) from the DataBase
		/// </summary>
		public bool IsSelected
		{
			get
			{
				return isSelected;
			}
		}

		/// <summary>
		/// Returns a simple string representation of this object and all it's properties and values
		/// </summary>
		public string RawView
		{
			get
			{
				string s = "";

				foreach (DataValueInstance dv in AllValues)
				{
					s += dv.DataValue.Name + ": ";

					if (!NullValues.IsNull(dv.Value))
					{
						s += dv.Value + "\r\n";
					}
					else
					{
						s += "\r\n";
					}
				}

				return s;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns the string representation of the DataObject
		/// </summary>
		public override string ToString()
		{
			return ((IStringSerializable)this).SerializeToString();
		}

		/// <summary>
		/// Execute all validations defined on the current Instance and
		/// returns an ValidationError array with the errors resultants of the
		/// validations (or empty if all the valiations were sucessfully)
		/// </summary>
		/// <param name="validatePK">
		/// Indicates if the primary key must be validated
		/// </param>
		/// <param name="operation">
		/// Indicates the operation whose execution raises this validation
		/// </param>
		/// <returns>
		/// ValidationError array with the errors resultants of the
		/// validations or empty if all the valiations were sucessfully
		/// </returns>
		public virtual List<ValidationError> Validate(DataBaseOperation operation)
		{
			//Local Varsdmember
			ValidationError err;
			List<ValidationError> errors;
			PrimaryKeyValidator pkV;

			//Creating array of validation errors
			errors = new List<ValidationError>();

			//Validate primary key values
			pkV = new PrimaryKeyValidator(this, operation);

			//Validating primary key
			err = pkV.Validate();

			//If errors were founded then adding to the array
			if (err != null) errors.Add(err);

			//If Insert or Update, validate regular values
			if (operation == DataBaseOperation.Insert || operation == DataBaseOperation.Update)
			{
				//Crossing the validators on the DataObject
				foreach (DataValueValidator v in DataValueValidator.GetValidators(this.DataType.RegularValues, this))
				{
					//Executing validation...
					err = v.Validate();

					//If errors were founded then adding to the array
					if (err != null) errors.Add(err);
				}
			}

			//Retrieving to the caller the errors founded
			return errors;
		}

		/// <summary>
		/// Merges another DataObject with the current DataObject. 
		/// Re-assigns all foreign-key related data from the other DataObject in favor of the current one
		/// and deletes the merged DataObject at the end.
		/// </summary>
		/// <remarks>
		/// The merged DataObject will be deleted. DataObject DataValues will not be copied into the current DataObject, only foreign-key related DataObjects will 
		/// be reasigned to the current one
		/// </remarks>
		/// <param name="dobj">DataObject that willl be merged and deleted</param>
		public void Merge(DataObject dobj)
		{
			if (NullValues.IsNull(dobj)) throw new ArgumentNullException("dobj");
			if (!dobj.DataType.Equals(this.DataType)) throw new ArgumentException("Argument's DataType must be the same as the current DataObject", "dobj");

			foreach (DataValue foreignKey in this.DataType.GetInboundForeignKeys())
			{
				//get all dataobjects related to dobj
				DataObjectList relatedDataObjects = DataBase.Current.SelectByForeignKey(foreignKey, dobj);

				foreach (DataObject related in relatedDataObjects)
				{
					//replace old value (dobj) with the current DataObject
					related[foreignKey].Value = this;
				}

				relatedDataObjects.Update();
			}

			dobj.Delete();
		}

		#endregion

		#region DataValues

		/// <summary>
		/// Indexer of the collection accessible by name
		/// </summary>
		/// <param name="DataValue">
		/// DataValue that you want to retrieve
		/// </param>
		/// <returns>
		/// DataValueInstance with the specified DataValue
		/// </returns>
		public DataValueInstance this[DataValue dvalue]
		{
			get
			{
				return this[dvalue.Name];
			}
		}

		/// <summary>
		/// Indexer of the collection accessible by name
		/// </summary>
		/// <param name="name">
		/// Name of DataValueInstance that you want to retrieve
		/// </param>
		/// <returns>
		/// DataValueInstance with the specified name
		/// </returns>
		public DataValueInstance this[string name]
		{
			get
			{
				//Crossing the DataValueInstances in the collection
				foreach (DataValueInstance dvi in this.AllValues)
				{
					//If the current DataValueInstance is the searched DataValueInstance 
					//retrieve it to the caller and leaving the procedure
					if (dvi.DataValue.Name == name) return dvi;
				}

				//If the execution of code reach this line, the DataValueInstance 
				//was not founded and throws the corresponding exception 
				throw new ArgumentException("DataObject does not contain a DataValueInstance named '" + name + "'", "name");
			}
		}

		/// <summary>
		/// Returns a boolean value that indicates if in the collection 
		/// exists an DataValueInstance with the specified name
		/// </summary>
		/// <param name="name">
		/// Name of the DataValueInstance searched
		/// </param>
		/// <returns>
		/// true if exists the DataValueInstance, otherwise false
		/// </returns>
		public bool ContainsValue(string name)
		{
			//Crossing the DataValueInstances on the collection
			foreach (DataValueInstance dvi in this.AllValues)
			{
				//Validating if the current DataValueInstance is the searched
				//and if it's, returning true to the caller
				if (dvi.DataValue.Name == name) return true;
			}

			//If the code execution reach this line, dont exists
			//a DataValueInstance with the specified name and retrieve
			//false to the caller
			return false;
		}

		/// <summary>
		/// Returns the list of DataValueInstances that are (or are not) part of the primary key
		/// </summary>
		/// <param name="isPrimaryKey">
		/// Whether the filter must include primary keys or not primary keys
		/// </param>
		/// <returns>
		/// Collection of DataValueInstance that fulfills the specified criteria
		/// </returns>
		public List<DataValueInstance> GetValues(bool isPrimaryKey)
		{
			return GetValues(isPrimaryKey, null);
		}

		/// <summary>
		/// Returns the list of DataValueInstances that fulfills the specified criteria.
		/// </summary>
		/// <param name="isPrimaryKey">
		/// Whether the filter must include primary keys or not primary keys
		/// </param>
		/// <param name="declaringDataType">
		/// DataType that must declare the DataValue to be retrieved, or null if not matters
		/// the DeclaringDataType on the filter
		/// </param>
		/// <returns>
		/// Collection of DataValueInstance that fulfills the specified criteria
		/// </returns>
		public List<DataValueInstance> GetValues(bool isPrimaryKey, DataType declaringDataType)
		{
			//Creating collection of DataValues
			List<DataValueInstance> col = new List<DataValueInstance>();

			//Crossing the DataValues of the collection
			foreach (DataValueInstance dvi in this.AllValues)
			{
				//Adding the current DataValue to the collection only if apply
				if (
					(dvi.DataValue.IsPrimaryKey == isPrimaryKey) &&
					(declaringDataType == null || dvi.DataValue.DeclaringDataType.Equals(declaringDataType))
					)
				{
					col.Add(dvi);
				}
			}

			//Retrieving the filtered collection
			return col;
		}

		/// <summary>
		/// Returns the value of the specified DataValue
		/// </summary>
		/// <param name="name">
		/// Name of DataValue
		/// </param>
		/// <returns>
		/// Value of the specified DataValue on this DataObject instance
		/// </returns>
		public object GetValue(string name)
		{
			return this[name].DataValue.GetValue(this);
		}

		/// <summary>
		/// Returns the value of the specified DataValue
		/// </summary>
		/// <param name="value">
		/// Value that you want to get
		/// </param>
		/// <returns>
		/// Value of the specified DataValue on this DataObject instance
		/// </returns>
		public object GetValue(DataValue value)
		{
			return value.GetValue(this);
		}

		/// <summary>
		/// Establish the value of the specified DataValue
		/// on this DataObject instance
		/// </summary>
		/// <param name="name">
		/// Name of DataValue to set
		/// </param>
		/// <param name="value">
		/// Value to establish on DataValue
		/// </param>
		public void SetValue(string name, object value)
		{
			this[name].DataValue.SetValue(this, value);
		}

		/// <summary>
		/// Establish the value of the specified DataValue
		/// on this DataObject instance
		/// </summary>
		/// <param name="value">
		/// DataValue to set
		/// </param>
		/// <param name="newValue">
		/// Value to establish on DataValue
		/// </param>
		public void SetValue(DataValue value, object newValue)
		{
			value.SetValue(this, newValue);
		}

		#endregion 

		#region DataEvents

		/// <summary>
		/// Allows you to subscribe a function to an event of the specified
		/// DataEvent
		/// </summary>
		/// <param name="devent">
		/// DataEvent for subscription
		/// </param>
		/// <param name="handler">
		/// Delegate that will be subscripted to the event
		/// </param>
		public void AddEventHandler(DataEvent devent, Delegate handler)
		{
			devent.AddHandler(this, handler);
		}

		/// <summary>
		/// Allows you to unsubscribe a function of the specified DataEvent
		/// </summary>
		/// <param name="devent">
		/// DataEvent for unsubscription
		/// </param>
		/// <param name="handler">
		/// Delegate that will be unsubscripted of the event
		/// </param>
		public void RemoveEventHandler(DataEvent devent, Delegate handler)
		{
			devent.RemoveHandler(this, handler);
		}

		#endregion

		#region DataMethods

		/// <summary>
		/// Allows you to invoke the specified method 
		/// </summary>
		/// <param name="dmethod">
		/// DataMethod that toy want to invoke
		/// </param>
		/// <param name="args">
		/// Array of arguments for the method invocation
		/// </param>
		/// <returns>
		/// Return value of the method
		/// </returns>
		public object Invoke(DataMethod dmethod, object[] args)
		{
			return dmethod.Invoke(this, args);
		}

		#endregion

		#region Initialization

		/// <summary>
		/// Sets all DataValues of the DataObject to their default null values and 
		/// if any dataproperty is a DataObject, an instance of this DataObject 
		/// it's created
		/// </summary>
		protected virtual void Init()
		{
			//Crossing the DataValues loaded
			foreach (DataValueInstance dv in this.AllValues)
			{
				if (dv.DataValue.IsForeignKey && dv.DataValue.IsPrimaryKey)
				{
					//If it's a DataObject, create an new instance
					dv.Value = dv.DataValue.ValueType.CreateInstance();
				}
				else
				{
					//Initializing primitive data to its null value
					dv.Value = NullValues.GetNullValue(dv.DataValue.ValueType);
				}
			}
		}

		/// <summary>
		/// Sets all foreign keys of the DataObject to their default instance
		/// </summary>
		public void InitForeignKeys()
		{
			foreach(DataValueInstance dv in this.ForeignKeys)
			{
				if (DataType.IsDataObjectSubClass(dv.DataValue.ValueType) && NullValues.IsNull(dv.Value))
				{
					dv.Value = dv.DataValue.ValueType.CreateInstance();
				}
			}
		}

		/// <summary>
		/// Creates a DataObject from its xml representation
		/// </summary>
		/// <param name="reader">
		/// XmlReader used to load the xml structure
		/// </param>
		/// <returns>
		/// DataObject instance
		/// </returns>		
		public static DataObject From(XmlReader reader)
		{
			//Loading the DataType attribute
			reader.MoveToAttribute(typeof(DataType).Name);
			DataType dtype = DataType.Parse(reader.Value);
			reader.MoveToElement();

			//Creating the DataObject instance
			DataObject dobj = (DataObject) dtype.InnerType.CreateInstance();

			//Loading DataObject data
			((IXmlSerializable)dobj).ReadXml(reader);
			
			//Returning DataObject
			return dobj;
		}
		
		/// <summary>
		/// Creates a DataObject of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for the DataObject creation
		/// </param>
		/// <returns>
		/// DataObject of the specified DataType
		/// </returns>
		public static DataObject From(DataType dtype)
		{
			//Validating that the dtype argument is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Creating DataObject of the specified Type
			DataObject dobj = (DataObject) dtype.InnerType.CreateInstance();

			//Returning DataObject
			return dobj;
		}

		#endregion

		#region Support functions

		/// <summary>
		/// Returns true if both DataObjects share the 
		/// same DataType and PrimaryKey values
		/// </summary>
		/// <param name="dobj">
		/// The dataobject to be compared against 
		/// this DataObject
		/// </param>
		/// <returns>
		/// true if both DataObjects share the same DataType 
		/// and PrimaryKey values, otherwise false
		/// </returns>
		public bool Equals(DataObject dobj)
		{
			//Local vars
			bool retValue = true;
			
			//Validating if dobj argument is null
			if (dobj == null) throw new ArgumentNullException("dobj");

			//Check if both DataObject have the same DataType
			if (!(this.DataType.IsAssignableFrom(dobj.DataType) || dobj.DataType.IsAssignableFrom(this.DataType)))
			{
				retValue = false;
			}
			else
			{
				//Crossing the DataValues of the Primary Key in this DataObject
				foreach (DataValueInstance dv in this.PrimaryKey)
				{
					//Validating if the DataValue have the same value in both instances
					if (!dv.Value.Equals(dv.DataValue.GetValue(dobj)))
					{
						retValue = false;
						break;
					}
				}
			}

			//Returning the value
			return retValue;
		}

		/// <summary>
		/// Returns true if the object can be converted to a DataObject and 
		/// both share the same DataType and PrimaryKey values
		/// </summary>
		/// <param name="obj">
		/// Object to compare against the DataObject
		/// </param>
		/// <returns>
		/// true if the object can be converted to a DataObject and 
		/// both share the same DataType and PrimaryKey values, 
		/// otherwise false
		/// </returns>
		public override bool Equals(object obj)
		{
			return (obj is DataObject && Equals((DataObject)obj));
		}

		/// <summary>
		/// Serves as a hash function for DataObjects
		/// </summary>
		public override int GetHashCode()
		{
			//First, we use the DataType hash code as a seed
			int hash = 1;
			int level = 1;

			//Then we use the primary key, assuming there is at least one numeric value in the primary key
			foreach(DataValueInstance dvi in this.PrimaryKey)
			{
				try
				{
					//if this is a numeric value, we convert it to integer and do a logical XOR on the current hash
					if (dvi.DataValue.IsNumeric)
					{
						hash = hash * ((int) TypeConverter.ChangeType(dvi.Value, typeof(int))) * level;
						level++;
					}
				}
				finally { }
			}

			//return the obtained hash
			return hash;
		}

		/// <summary>
		/// Compares the current instance with another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects
		/// being compared. The return value has these meanings: Value Meaning Less than
		/// zero This instance is less than obj. Zero This instance is equal to obj.
		/// Greater than zero This instance is greater than obj.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown in case obj is null</exception>
		/// <exception cref="ArgumentException">dobj is not the same type as this instance</exception>
		int IComparable.CompareTo(object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");
			if (!(obj.GetType().IsAssignableFrom(obj.GetType()) || obj.GetType().IsAssignableFrom(obj.GetType()))) throw new ArgumentException("Argument is not the same type as this instance", "dobj");

			return CompareTo((DataObject)obj);
		}

		/// <summary>
		/// Compares the current instance with another DataObject of the same type.
		/// </summary>
		/// <param name="dobj">A DataObject to compare with this instance.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects
		/// being compared. The return value has these meanings: Value Meaning Less than
		/// zero This instance is less than obj. Zero This instance is equal to obj.
		/// Greater than zero This instance is greater than obj.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown in case obj is null</exception>
		/// <exception cref="ArgumentException">dobj is not the same type as this instance</exception>
		int CompareTo(DataObject dobj)
		{
			if (dobj == null) throw new ArgumentNullException("dobj");
			if (!(this.DataType.IsAssignableFrom(dobj.DataType) || dobj.DataType.IsAssignableFrom(this.DataType))) throw new ArgumentException("Argument is not the same type as this instance", "dobj");

			//Return the compare result of these 2 object's hash code
			return this.GetHashCode().CompareTo(dobj.GetHashCode());
		}

		/// <summary>
		/// Copies all DataValues from the current DataObject 
		/// to another, duplicating all information
		/// </summary>
		/// <param name="dobj">
		/// DataObject which DataProperty values will be copied to
		/// </param>
		public void CopyTo(DataObject dobj)
		{
			//Validating that dobj argument is null
			if (dobj == null) throw new ArgumentNullException("dobj");

			//Validating if the specified DataObject can be assigned
			//to the current DataObject
			if (!(this.DataType.IsAssignableFrom(dobj.DataType) || dobj.DataType.IsAssignableFrom(this.DataType)))
				throw new Exception(
					"Cannot copy values from DataType '" + this.DataType.FullName + "' to DataType '" +
					dobj.DataType.FullName + "' because they are not assignable from each other");

			//Copying DataValues...
			foreach (DataValue dv in this.DataType.AllValues)
			{
				if(dobj.DataType.AllValues.Contains(dv)) dobj.SetValue(dv, this.GetValue(dv));
			}

			//copy isSaved and isSelected
			dobj.isSaved = this.isSaved;
			dobj.isSelected = this.isSelected;
		}

		#endregion

		#region Database Operations

		/// <summary>
		/// Loads the current DataObject from the DataBase
		/// </summary>
		/// <returns>True if the current DataObject was found in the DataBase, false otherwise</returns>
		public bool Select()
		{
			return DataBase.Current.Select(this) != null;
		}

		/// <summary>
		/// Loads the current DataObject from the DataBase only if it's not already loaded
		/// </summary>
		public void SelectOnce()
		{
			if (!IsSelected)
			{
				DataBase.Current.Select(this);
			}
		}

		/// <summary>
		/// Inserts the current DataObject into the Database
		/// </summary>
		/// <returns>
		/// Number of rows afected by the operation
		/// </returns>
		public void Insert()
		{
			DataBase.Current.Insert(this);
		}

		/// <summary>
		/// If the current DataObject is not already saved in the Database, it is inserted, otherwise, it is updated
		/// </summary>
		/// <returns>
		/// Number of rows afected by the operation
		/// </returns>
		public void Save() 
		{
			if (this.Exists())
			{
				this.Update();
			}
			else
			{
				this.Insert();
			}
		}

		/// <summary>
		/// Deletes the current DataObject from the DataBase
		/// </summary>
		public void Delete() 
		{
			DataBase.Current.Delete(this);
		}

		/// <summary>
		/// Updates the current DataObject from the DataBase
		/// </summary>
		public void Update()
		{
			DataBase.Current.Update(this);
		}

		/// <summary>
		/// Returns a value that indicates if the specified DataObject 
		/// exists on the Database
		/// </summary>
		/// <returns>
		/// true if DataObject exists
		/// </returns>
		public bool Exists()
		{
			return DataBase.Current.Exists(this);
		}

		#endregion

		#region Events

		/// <summary>
		/// Runs after the current DataObject is loaded from DataBase
		/// </summary>
		internal protected virtual void OnAfterSelect()
		{
			this.isSelected = true;
			this.isSaved = true;
		}

		/// <summary>
		/// Runs before the current DataObject is inserted into the DataBase
		/// </summary>
		internal protected virtual void OnBeforeInsert()
		{
			List<ValidationError> errors;

			errors = Validate(DataBaseOperation.Insert);
			if (errors.Count > 0) throw new ValidationException(errors, this, "DataObject can't be inserted, it contains invalid data");
		}

		/// <summary>
		/// Runs after the current DataObject is inserted into the DataBase
		/// </summary>
		internal protected virtual void OnAfterInsert()
		{
			this.isSaved = true;
		}

		/// <summary>
		/// Runs before the current DataObject is updated into the DataBase
		/// </summary>
		internal protected virtual void OnBeforeUpdate()
		{
			List<ValidationError> errors;

			errors = Validate(DataBaseOperation.Update);
			if (errors.Count > 0) throw new ValidationException(errors, this, "DataObject can't be updated, it contains invalid data");
		}

		/// <summary>
		/// Runs after the current DataObject is updated into the DataBase
		/// </summary>
		internal protected virtual void OnAfterUpdate()
		{
			isSaved = true;
		}

		/// <summary>
		/// Runs before the current DataObject is deleted in the DataBase
		/// </summary>
		internal protected virtual void OnBeforeDelete()
		{
			List<ValidationError> errors;

			errors = Validate(DataBaseOperation.Delete);
			if (errors.Count > 0) throw new ValidationException(errors, this, "DataObject can't be deleted, it contains invalid data");
		}

		/// <summary>
		/// Runs after the current DataObject is deleted in the DataBase
		/// </summary>
		internal protected virtual void OnAfterDelete()
		{
			this.isSaved = false;
		}

		/*
		 * TODO: implement events for before and after methods execution to implement security or logging
		internal protected virtual void OnBeforeMethodInvoked(DataMethod)
		{
			throw new NotImplementedException();
		}
		*/

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the specified Xml into the current DataObject
		/// </summary>
		/// <param name="reader">
		/// XmlReader used to read the xml DataObject representation
		/// </param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			//Local vars
			XmlNodeList nodes;
			XmlReader subReader = reader.ReadSubtree();
			XmlDocument xml = new XmlDocument();
			xml.Load(subReader);

			//Getting the DataType of DataObject
			DataType xmlType = DataType.Parse(xml.DocumentElement.Attributes[typeof(DataType).Name].Value);

			//Validating if the DataType is equals to the current DataType
			if (!xmlType.Equals(this.DataType))
				throw new InvalidDataTypeException(
					xmlType.GetType(),
					"Can't read XML of DataType '" + xmlType + "' to deserialize a DataObject of Type '" + DataType + "'");

			//Initializing the Values collection
			this.AllValues.Clear();
			foreach (DataValue dv in this.DataType.AllValues)
			{
				this.AllValues.Add(new DataValueInstance(this, dv));
			}

			//Initializing the DataObject
			this.Init();
			this.InitForeignKeys();

			//Creating the DataValueInstanceAtomizedDictionary for recovery the DataValues values of DataObject
			DataValueInstanceAtomizedDictionary atomized = new DataValueInstanceAtomizedDictionary(this.AllValues);

			//Crossing the DataValues from the DataObject
			foreach (KeyValuePair<string, DataValueInstance> dvi in atomized)
			{
				//Searching the node of the current DataValue	
				nodes = xml.GetElementsByTagName(dvi.Key);

				//Validating if the DataValue was founded; if not
				//continue with the next DataValue
				if (nodes.Count > 0)
				{
					//Loading the Node of the DataValue
					XmlNode node = nodes[0];

					//Validating if the node is wrong defined
					if (!string.IsNullOrWhiteSpace(node.InnerText))
					{
						dvi.Value.DataValue.SetValue(
							dvi.Value.DataObject,
							TypeConverter.ChangeType(node.InnerText, dvi.Value.DataValue.ValueType));
					}
				}
			}

			//IsSaved
			nodes = xml.GetElementsByTagName("IsSaved");
			if (nodes.Count > 0)
			{
				//Loading the Node of the DataValue
				XmlNode node = nodes[0];

				//Validating if the node is wrong defined
				if (!string.IsNullOrWhiteSpace(node.InnerText))
				{
					isSaved = (bool)TypeConverter.ChangeType(node.InnerText, typeof(bool));
				}
			}

			subReader.Close();
		}

		/// <summary>
		/// Serialize the current DataObject into a xml writer
		/// </summary>
		/// <param name="writer">
		/// XmlWriter used for the serialization
		/// </param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			//Local Vars
			XmlElement node;

			//Creating the Xml document
			XmlDocument xml = new System.Xml.XmlDocument();

			//Creating the root node
			XmlElement mainNode = xml.CreateElement(this.GetType().Name);
			xml.AppendChild(mainNode);

			//Creating DataValueInstanceAtomizedDictionary collection of the DataObject
			DataValueInstanceAtomizedDictionary atomized = new DataValueInstanceAtomizedDictionary(this.AllValues);

			//Crossing the DataValues of the DataObject
			foreach (KeyValuePair<string, DataValueInstance> dvi in atomized)
			{
				//Creating the DataValue node
				node = xml.CreateElement(dvi.Key);

				//Getting the value of the current DataValue
				object currentValue = dvi.Value.DataValue.GetValue(dvi.Value.DataObject);

				//Setting the Value if not is null
				if (!NullValues.IsNull(currentValue))
				{
					node.InnerText = TypeConverter.SerializeToString(currentValue);
				}

				//Appending the node of the DataValue
				mainNode.AppendChild(node);
			}

			//IsSaved
			node = xml.CreateElement("IsSaved");
			node.InnerText = TypeConverter.SerializeToString(IsSaved);
			mainNode.AppendChild(node);

			//Establishing the Name of the entity
			writer.WriteAttributeString(typeof(DataType).Name, this.DataType.UniqueId);

			//Writing xml...
			mainNode.WriteContentTo(writer);
		}

		/// <summary>
		/// Following online help recommendations, this method allways return null
		/// </summary>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		#endregion

		#region IStringSerializable Members

		/// <summary>
		/// Creates a string represnetation of this DataObject, including the DataType and primary key, so it can be deserialized at later time
		/// </summary>
		/// <returns>String representation of this DataObject</returns>
		/// <example>OKHOSTING.Business.DataObjects.Customer, OKHOSTING.Business.DataObjects|id=5</example>
		string IStringSerializable.SerializeToString()
		{
			return
				"DataType=" +
				((IStringSerializable)this.DataType).SerializeToString() +
				"&" +
				TypeConverter.ToString(this.PrimaryKey);
		}

		/// <summary>
		/// NOT IMPLEMENTED
		/// </summary>
		void IStringSerializable.DeserializeFromString(string s)
		{
			DataType dtype;

			//Validating if the sting is null
			if (s == null) throw new ArgumentNullException("s");

			//deserialize datatype
			dtype = TypeConverter.ToDataType(TypeConverter.GetValueFromQueryString(s, "DataType"));

			//validate if we are of the same DataType
			if (!dtype.Equals(this.DataType)) throw new ArgumentException("Can't deserialize from '" + dtype.FullName + "' to '" + this.DataType.FullName + "'");

			//get all values
			TypeConverter.ToDataValueInstances(s, this.AllValues);
		}

		#endregion
	}
}