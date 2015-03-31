using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using OKHOSTING.Tools;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Contains the definition and scructure of a Type that inheriths from OKHOSTING.DataObject
	/// </summary>
	[Serializable]
	public sealed class DataType : Type, IStringSerializable, IXmlSerializable, IComparable
	{
		#region Constructors

		public DataType()
		{
		}

		#endregion
		
		#region Private and static Members

		/// <summary>
		/// Name of the table that represents this DataTaype in the DataBase
		/// </summary>
		private string tableName;
		
		/// <summary>
		/// System.Type associated to DataObject
		/// </summary>
		private SYstem.Type innerType;

		/// <summary>
		/// Collection of members (fields, properties, methods and events), of the DataType
		/// </summary>
		private List<DataMember> members = new List<DataMember>();

		/// <summary>
		/// Global Cache of already loaded DataTypes (because is static)
		/// </summary>
		private static Dictionary<Type, DataType> Cache
		{
			get
			{
				//if cache already exist, retrieve it from the session
				if (Session.Current.ContainsKey("DataType.Cache"))
				{
					return (Dictionary<Type, DataType>)Session.Current["DataType.Cache"];
				}
				///otherwise, create a new cache and store it in the session
				else
				{
					//Loading the cache for the current process or session
					Dictionary<Type, DataType> cache = new Dictionary<Type, DataType>();

					//Storing the cache in the current session
					try
					{
						Session.Current.Add("DataType.Cache", cache);
					}
					catch { }

					//Retrieving cache recently created
					return cache;
				}
			}
		}
		
		#endregion

		#region Properties

		/// <summary>
		/// System.Type in wich this DataType is created from
		/// </summary>
		public System.Type InnerType
		{
			get
			{
				return this.innerType;
			}
		}

		/// <summary>
		/// DataType's table name. The table's name that is ussed in the DataBase to reference this DataType
		/// </summary>
		public string TableName 
		{
			get
			{
				return tableName;
			}
		}

		/// <summary>
		/// DataType's unique identifier
		/// </summary>
		/// <remarks>
		/// Uses InnerType.AssemblyQualifiedName
		/// </remarks>
		public string UniqueId
		{
			get
			{
				//return innerType.AssemblyQualifiedName;
				//return innerType.FullName + ", " + InnerType.Assembly.FullName;
				
				//try to make strings shorter by using the fullname only
				return innerType.FullName;
			}
		}

		/// <summary>
		/// Parent DataType of the DataType
		/// </summary>
		public DataType BaseDataType
		{
			get
			{
				//Validating if the base type is the root DataObject
				if (DataType.IsDataObjectSubClass(innerType.BaseType))
				{
					return innerType.BaseType;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// List of DataMembers defined on the DataObject
		/// </summary>
		public List<DataMember> Members
		{
			get
			{
				return members;
			}
		}

		/// <summary>
		/// Returns a DataMemberCollection containing all DataValues
		/// </summary>
		public List<DataValue> AllValues
		{
			get
			{
				return GetMembers<DataValue>();
			}
		}

		/// <summary>
		/// Returns a list containing all DataValues that conform the primary key
		/// </summary>
		public List<DataValue> PrimaryKey
		{
			get
			{
				return GetValues(true);
			}
		}

		/// <summary>
		/// Returns a list containing all DataValues that are foreign keys
		/// </summary>
		public List<DataValue> ForeignKeys
		{
			get
			{
				List<DataValue> fk;

				fk = new List<DataValue>();
				foreach (DataValue dvalue in AllValues)
				{
					if (dvalue.IsForeignKey) fk.Add(dvalue);
				}

				return fk;
			}
		}

		/// <summary>
		/// Returns a DataMemberCollection containing all DataValues that are regular values (not primary key and not collection)
		/// </summary>
		public List<DataValue> RegularValues
		{
			get
			{
				return GetValues(false);
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Compare this DataType's instance with another to see if they are the same
		/// </summary>
		public bool Equals(DataType dtype)
		{
			//Validating if the argument is null
			if(dtype==null) throw new ArgumentNullException("dtype");

			//Comparing the InnerType types 
			return this.innerType.Equals(dtype.innerType);
		}

		/// <summary>
		/// Compare this Type's instance with another to see if they are the same
		/// </summary>
		public bool Equals(Type type)
		{
			//Validating if the argument is null
			if (type == null) throw new ArgumentNullException("type");

			//Comparing the InnerType types 
			return this.innerType.Equals(type);
		}

		/// <summary>
		/// Compare this Type's instance with another to see if they are the same
		/// </summary>
		public override bool Equals(object obj)
		{
			//Validating if the argument is null
			if(obj==null) throw new ArgumentNullException("obj");
			
			//Validating if the argument is a System.Type
			if (obj is Type)
			{
				return this.innerType.Equals((Type)obj);
			}

			//Validating if the argument is a OKHOSTING.Code.ORM.DataType
			else if (obj is DataType)
			{
				return this.Equals((DataType)obj);
			}

			else
			{
				//The object is not a DataType and is not a System.Type
				return false;
			}
		}

		/// <summary>
		/// Serves as a hash function for DataTypes
		/// </summary>
		/// <remarks>Returns the InnerType.GetHashCode() value</remarks>
		public override int GetHashCode()
		{
			return InnerType.GetHashCode();
		}
		
		/// <summary>
		/// Determines whether an instance of the current 
		/// DataType is assignable from another DataType
		/// </summary>
		public bool IsAssignableFrom(DataType dtype)
		{
			//Validating if the argument is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Validating...
			return this.innerType.IsAssignableFrom(dtype.innerType);
		}

		/// <summary>
		/// Returns a string representation of this DataType
		/// </summary>
		public override string ToString()
		{
			return this.Name;
		}

		/// <summary>
		/// Returns all parent DataTypes ordered from base to child
		/// </summary>
		public List<DataType> GetBaseDataTypes()
		{
			//Local Vars
			List<DataType> types;
			DataType current;

			//Creating list of Types
			types = new List<DataType>();

			//Get all types in ascendent order (from base to child)
			current = this;
			while (current != null)
			{
				//Inserting the current object at the first position
				types.Insert(0, current);

				//Getting the parent of the current object
				current = current.BaseDataType;
			}

			//Returning the List of DataTypes			
			return types;
		}

		/// <summary>
		/// Searches for all DataTypes inherited from this DataType 
		/// </summary>
		/// <returns>
		/// All DataTypes that directly inherit from the current DataType
		/// </returns>
		public List<DataType> GetSubClassDataTypes()
		{
			//Local Vars
			List<DataType> allDataTypes;
			List<DataType> result;

			//Loading collection with the DataTypes defined on the specified
			//namespace
			allDataTypes = DataType.GetAllDataTypes();

			//Creating list of Child DataTypes
			result = new List<DataType>();
			
			//Crossing all loaded DataTypes
			foreach(DataType dt in allDataTypes)
			{
				//Validating if the dataType has a Base Class
				if (dt.BaseDataType != null)
				{
					//Validating if the base class of the DataType
					//is this DataType
					if (dt.BaseDataType.Equals(this))
					{
						result.Add(dt);
					}
				}
			}

			//Retrieving the list of Childs
			return result;
		}

		/// <summary>
		/// Searches for all DataTypes inherited from this DataType in a recursive way
		/// </summary>
		/// <returns>
		/// All DataTypes that directly and indirectly inherit from the current DataType. 
		/// Returns the hole tree of subclasses.
		/// </returns>
		public List<DataType> GetSubClassDataTypesRecursive()
		{
			//Local Vars
			List<DataType> allDataTypes;
			List<DataType> result;

			//Loading collection with the DataTypes defined on the specified
			//namespace
			allDataTypes = DataType.GetAllDataTypes();

			//Creating list of Child DataTypes
			result = new List<DataType>();

			//Crossing all loaded DataTypes
			foreach (DataType dt in allDataTypes)
			{
				//Validating if the dataType has a Base Class
				if (dt.BaseDataType != null)
				{
					//Validating if the base class of the DataType
					//is this DataType
					if (dt.BaseDataType.Equals(this))
					{
						result.Add(dt);
						result.AddRange(dt.GetSubClassDataTypesRecursive());
					}
				}
			}

			//Retrieving the list of Childs
			return result;
		}

		/// <summary>
		/// Searches for all DataValues in all DatTypes that uses the current DataType as a foreign key
		/// </summary>
		/// <returns>
		/// All DataValues in external DataTypes that uses the current DataType as a foreign key
		/// </returns>
		public List<DataValue> GetInboundForeignKeys()
		{
			//Local Vars
			List<DataValue> result;

			//Creating list of datavalues which are foreign keys for this DataType
			result = new List<DataValue>();

			//Crossing all loaded DataTypes
			foreach (DataType dt in DataType.GetAllDataTypes())
			{
				//Validating if the dataType contains a DataValue which ValueType is the current DataType
				foreach (DataValue dv in dt.ForeignKeys)
				{
					//If this DataValue is a foreign key to the current DataType, add the DataType to the result
					if (dv.ValueType.IsAssignableFrom(this.InnerType) && dv.DeclaringDataType.Equals(dt))
					{
						result.Add(dv);
					}
				}
			}

			//Retrieving the list of inbound foreign key DataValues
			return result;
		}

		/// <summary>
		/// Searches for all DataValues in the current instance that are foreign keys, which means they are of Type DataObject
		/// </summary>
		/// <returns>
		/// All DataValues in the current DataTypes that are foreign keys
		/// </returns>
		public List<DataValue> GetOutboundForeignKeys()
		{
			//Local Vars
			List<DataValue> result;

			//Creating list of datavalues which are foreign keys for this DataType
			result = new List<DataValue>();

			//Crossing all loaded DataTypes
			foreach (DataValue dv in this.AllValues)
			{
				//If this DataValue is a foreign key, add the DataType to the result
				if (dv.IsForeignKey)
				{
					result.Add(dv);
				}
			}

			//Retrieving the list of inbound foreign key DataValues
			return result;
		}

		/// <summary>
		/// Indexer of the Member collection accessible by name
		/// </summary>
		/// <param name="name">
		/// Name of DataMember that you want to retrieve
		/// </param>
		/// <returns>
		/// DataMember with the specified name
		/// </returns>
		public DataMember this[string name]
		{
			get
			{
				//Crossing the DataMembers in the collection
				foreach (DataMember dm in this.Members)
				{
					//If the current DataMember is the searched DataMember 
					//retrieve it to the caller and leaving the procedure
					if (dm.Name == name) return dm;
				}

				//If the execution of code reach this line, the DataMember 
				//was not founded and throws the corresponding exception 
				throw new ArgumentException(String.Format("DataType '{0}' does not contain a DataMember named '{1}'", this.FullName, name), "name");
			}
		}

		/// <summary>
		/// Returns a boolean value that indicates if in the Members collection 
		/// exists an DataMember with the specified name
		/// </summary>
		/// <param name="name">
		/// Name of the DataMember searched
		/// </param>
		/// <returns>
		/// true if exists the DataMember, otherwise false
		/// </returns>
		public bool ContainsMember(string name)
		{
			//Crossing the DataMembers on the collection
			foreach (DataMember dm in this.Members)
			{
				//Validating if the current DataMember is the searched
				//and if it's, returning true to the caller
				if (dm.Name == name) return true;
			}

			//If the code execution reach this line, dont exists
			//a DataMember with the specified name and retrieve
			//false to the caller
			return false;
		}

		/// <summary>
		/// Select only the DataMembers of to the specified DataMember type
		/// </summary>
		/// <typeparam name="DMT">
		/// Type of the DataMembers to filter
		/// </typeparam>
		/// <returns>
		/// Collection of DataMembers from the specified DataMember type
		/// </returns>
		public List<DM> GetMembers<DM>() where DM : DataMember
		{
			//Creating the collection of filtered DataMembers
			List<DM> collection = new List<DM>();

			//Crossing the DataMembers on the collection
			foreach (DataMember dm in this.Members)
			{
				//Adding the DataMember if his type is the searched
				if (dm is DM) collection.Add((DM) dm);
			}

			//Retrieving the filtered collection
			return collection;
		}

		/// <summary>
		/// Returns the list of DataValues that fulfills the specified criteria.
		/// </summary>
		/// <param name="category">
		/// Filters by the specified DataValueCategory
		/// </param>
		/// <returns>
		/// Collection of DataValues that fulfills the specified criteria
		/// </returns>
		public List<DataValue> GetValues(bool isPrimaryKey)
		{
			return GetValues(isPrimaryKey, null);
		}

		/// <summary>
		/// Returns the list of DataValues that fulfills the specified criteria.
		/// </summary>
		/// <param name="category">
		/// Filters by the specified DataValueCategory
		/// </param>
		/// <param name="declaringDataType">
		/// DataType that must declare the DataValue to be retrieved, or null if not matters
		/// the DeclaringDataType on the filter
		/// </param>
		/// <returns>
		/// Collection of DataValues that fulfills the specified criteria
		/// </returns>
		public List<DataValue> GetValues(bool isPrimaryKey, DataType declaringDataType)
		{
			//Creating collection of DataValues
			List<DataValue> col = new List<DataValue>();

			//Crossing the DataValues of the collection
			foreach (DataValue dv in this.AllValues)
			{
				//Adding the current DataValue to the collection only if apply
				if (
					(dv.IsPrimaryKey == isPrimaryKey) &&
					(declaringDataType == null || dv.DeclaringDataType.Equals(declaringDataType))
					)
				{
					col.Add(dv);
				}
			}

			//Retrieving the filtered collection
			return col;
		}

		#endregion

		#region Setup

		/// <summary>
		/// Returns a collection of dataobjects that should be inserted in the DataBase on system setup
		/// </summary>
		public DataObjectList GetSetupDataObjects()
		{
			MethodInfo method;

			method = this.InnerType.GetMethod("GetSetupDataObjects");
			if (method == null) return null;
			if (!method.IsStatic) return null;
			if (method.ReturnParameter == null) return null;
			if (method.ReturnParameter.ParameterType != typeof(DataObjectList)) return null;

			return (DataObjectList)method.Invoke(null, null);
		}

		#endregion

		#region Private methods

		/// <summary>
		/// Clears and populates members
		/// </summary>
		/// <param name="type">
		/// Type of the class used to Initialize
		/// </param>
		public void InitFrom(Type type)
		{
			//Local Vars
			List<Type> baseTypes;

			//Validating if the specified type is null
			if (type == null) throw new ArgumentNullException("type");

			//Establishing the inner type of the DataType
			this.innerType = type;

			//Getting all System.Type's that are super classes of the DataType (Excluding the System.Object and DataObject)
			baseTypes = GetBaseTypes();
			
			//Cleaning Members collection
			Members.Clear();

			//Crossing the parent types of the DataType
			foreach(Type current in baseTypes)
			{
				//Crossing the members of the type
				foreach (MemberInfo member in current.GetMembers())
				{
					//Validating if the member of the current type, is defined in the Type 
					//that is Initializing and, if is, continue with the next cycle iteration
					if (!member.DeclaringType.Equals(current)) continue;
					
					//Validating if the member have an DataMember Attribute assigned
					if (DataMember.IsImplementedBy(member))
					{
						//Adding DataMember to the collection
						Members.Add(DataMember.From(member));
					}
				}
			}

			//Custom table name

			//Trying of obtain the and TableNameAttribute defined for the class
			object[] attributes = this.InnerType.GetCustomAttributes(typeof(TableNameAttribute), true);
			TableNameAttribute att = (TableNameAttribute)(attributes.GetLength(0) > 0 ? attributes[0] : null);

			//Validating if the TableNameAttribute was founded 
			if (att != null)
			{
				tableName = att.TableName;
			}
			//if no TableNameAttribute is defined, use Configuration.TableNameFormat
			else
			{
				switch (Configuration.Current.TableNameFormat)
				{
					case TableNameFormat.DataTypeFullName:
						tableName = FullName.Replace('.', '_');
						break;

					case TableNameFormat.DataTypeName:
						tableName = Name;
						break;

					case TableNameFormat.DataTypeLastNameSpaceAndName:
						if (string.IsNullOrWhiteSpace(this.NameSpace))
						{
							tableName = Name;
						}
						else
						{
							string lastNameSpace = NameSpace.Substring(NameSpace.LastIndexOf('.') + 1);
							tableName = lastNameSpace + '_' + Name;
						}
						break;

					default:
						tableName = Name;
						break;
				}
			}
		}

		#endregion

		#region Static methods

		/// <summary>
		/// Creates and DataType's instance from a System.Type
		/// </summary>
		/// <param name="type">
		/// System.Type used to create the DataType to return
		/// </param>
		/// <returns>
		/// DataType's instance from specified System.Type
		/// </returns>
		public static DataType From(Type type)
		{
			//Local Vars
			DataType dt;

			//Validating if the type specified is null
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			//Searching for the type in Cache 
			Cache.TryGetValue(type, out dt);
			if (dt != null) return dt;

			//If not in cache, build DataType;
			//Validating that the specified Type is a DataObject
			if (!DataType.IsDataObject(type))
			{
				throw new InvalidDataTypeException(type, "'" + type.FullName + "' does not inherits from OKHOSTING.DataObject");
			}

			//Creating the DataType and initializing
			dt = new DataType();
			dt.InitFrom(type);

			//Adding to cache and return
			try
			{
				Cache.Add(type, dt);
			}
			catch (Exception e)
			{
				Log.Write(e.Source, e.Message, Log.Exception);
			}

			//Returning the corresponding DataType
			return dt;
		}
		
		/// <summary>
		/// Creates and DataType's instance from a string
		/// </summary>
		/// <param name="value">
		/// String containing the DataType FullName
		/// </param>
		/// <returns>
		/// DataType's instance from specified string
		/// </returns>
		public static DataType Parse(string value)
		{
			//Validating if the type specified is null
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentNullException("value");
			}

			return TypeConverter.ToDataType(value);
		}
		
		/// <summary>
		/// Creates and DataType's instance from a System.Type
		/// </summary>
		/// <param name="type">
		/// System.Type used to create the DataType to return
		/// </param>
		/// <returns>
		/// DataType's instance from specified System.Type
		/// </returns>
		public static implicit operator DataType(Type type)
		{
			//Validating that the type is not null
			if (type == null) return null;

			//Creating the DataType and retrieve it
			return DataType.From(type);
		}
		
		/// <summary>
		/// Returns all DataTypes found in an Assembly
		/// </summary>
		/// <param name="assembly">
		/// An Assembly that will be scanned looking for DataTypes
		/// </param>
		/// <returns>
		/// List of DataTypes found in Assembly
		/// </returns>
		public static List<DataType> GetAllDataTypes(Assembly assembly)
		{
			//Local Vars
			List<DataType> dtypes;
			System.Type[] types = null;

			//Creating array of DataTypes
			dtypes = new List<DataType>();

			try
			{
				types = assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				//throw e.LoaderExceptions[0];
			}
			catch
			{
				//throw;
			}

			if (types == null) return dtypes;

			//Crossing the types of the Assembly
			foreach (Type type in types)
			{
				//Validating if the type is an DataObject
				if (DataType.IsDataObjectSubClass(type))
				{
					dtypes.Add(type);
				}
			}

			//Returning the collection
			return dtypes;
		}

		/// <summary>
		/// Returns all DataTypes found in all the loaded Assemblies
		/// </summary>
		/// <returns>
		/// List of DataTypes found in all loaded Assemblies
		/// </returns>
		public static List<DataType> GetAllDataTypes()
		{
			//Local vars
			List<DataType> dtypes;
			List<string> systemAssemblies;

			//Creating list of DataTypes
			dtypes = new List<DataType>();

			//Have a list of systen assemblies names, so they can be excluded from the search for better performance
			systemAssemblies = new List<string>();
			systemAssemblies.Add("System");
			systemAssemblies.Add("Microsoft");
			systemAssemblies.Add("mscorlib");
			systemAssemblies.Add("vshost");
			systemAssemblies.Add("Interop");
			systemAssemblies.Add("MySql");
			systemAssemblies.Add("OKHOSTING.Tools");

			//load all assemblies
			foreach (string file in System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll"))
			{
				System.IO.FileInfo info = new System.IO.FileInfo(file);

				try
				{
					AppDomain.CurrentDomain.Load(info.Name.Replace(info.Extension, ""));
				}
				catch { }
			}

			//Crossing the assemblies in the current AppDomain
			foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
			{
				bool isSystemAssembly = false;

				//Search the systemAssemblies collection to see if the current assembly is a system assembly
				//and therefore should be excluded from the search
				foreach (string excluded in systemAssemblies)
				{
					//If the assemblies full name starts with an excluded keyword, or is in a folder containing "Microsoft", it is considered a system assembly
					if (a.FullName.StartsWith(excluded))
					{
						isSystemAssembly = true;
						break;
					}
				}

				//If it is a system assembly, continue with the next assembly
				if (isSystemAssembly) continue;

				//Otherwise, get all the DataTypes in the assembly
				dtypes.AddRange(GetAllDataTypes(a));
			}

			//Retrieving the collection
			return dtypes;
		}

		/// <summary>
		/// Indicates wether the specified type is DataObject or a subclass of DataObject
		/// </summary>
		/// <param name="type">Type that will be analized</param>
		/// <returns>True is type is equal to DataObject or a subclass of DataObject, false otherwise</returns>
		public static bool IsDataObject(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return type.IsSubclassOf(typeof(DataObject)) || type.Equals(typeof(DataObject));
		}

		/// <summary>
		/// Indicates wether the specified type is a subclass of DataObject
		/// </summary>
		/// <param name="type">Type that will be analized</param>
		/// <returns>True is type is a subclass of DataObject, false otherwise</returns>
		public static bool IsDataObjectSubClass(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");
			return type.IsSubclassOf(typeof(DataObject));
		}

		#endregion

		#region Xml and string Serialization

		/// <summary>
		/// Deserialize the DataType
		/// </summary>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			//Local Vars
			Type type;
			
			//Reading the type
			reader.MoveToAttribute("InnerType");
			type = Type.GetType(reader.Value, true);
			reader.MoveToElement();
			reader.Read();

			//Initializing the type and retrieve it 
			this.InitFrom(type);
		}

		/// <summary>
		/// Serialize the DataType
		/// </summary>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("InnerType", InnerType.AssemblyQualifiedName);
		}

		/// <summary>
		/// Following online help recommendations, this method allways return null
		/// </summary>
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Returns the XML Schema used to validate DataType serializations
		/// </summary>
		public static XmlQualifiedName GetXMLSchema(XmlSchemaSet xs)
		{
			//Local Vars
			XmlSerializer schemaSerializer;
			XmlSchema s;
			string xsdPath;

			schemaSerializer = new XmlSerializer(typeof(XmlSchema));
			xsdPath = AppDomain.CurrentDomain.BaseDirectory + "DataType.xsd";
			s = (XmlSchema)schemaSerializer.Deserialize(new XmlTextReader(xsdPath), null);
			xs.XmlResolver = new XmlUrlResolver();
			xs.Add(s);

			return new XmlQualifiedName("DataType", "http://okhosting.com/");
		}

		/// <summary>
		/// Creates and populates a XML document including the complete DataType's list of properties
		/// </summary>
		public XmlDocument CreateFullXml()
		{
			//Local Vars
			XmlDocument xml;
			XmlElement element;
			XmlElement node;
			XmlElement dataFieldsNode;

			//Creating the document
			xml = new System.Xml.XmlDocument();

			//Creating the root element
			element = xml.CreateElement(typeof(DataType).Name);
			element.SetAttribute("InnerType", this.UniqueId);
			xml.AppendChild(element);

			//Creating the dataFields child Element
			dataFieldsNode = xml.CreateElement("DataValues", null);
			element.AppendChild(dataFieldsNode);

			//Crossing the DataValues of type and creating the nodes
			foreach (DataValue dvalue in this.AllValues)
			{
				node = xml.CreateElement("DataValue", null);
				node.SetAttribute("Name", dvalue.Name);
				node.SetAttribute("IsPrimaryKey", dvalue.IsPrimaryKey.ToString());
				node.SetAttribute("ValueType", dvalue.ValueType.AssemblyQualifiedName);

				dataFieldsNode.AppendChild(node);
			}

			//Retrieving the xml document
			return xml;
		}

		/// <summary>
		/// Serialize the DataType to a string
		/// </summary>
		string IStringSerializable.SerializeToString()
		{
			return this.UniqueId;
		}

		/// <summary>
		/// Deserializes the DataType from a string containing the DataType's assembly qualified name
		/// </summary>
		/// <param name="s">A string containing the DataType's assembly qualified name</param>
		void IStringSerializable.DeserializeFromString(string s)
		{
			//Local Vars
			Type type = null;

			//validate argument
			if (string.IsNullOrWhiteSpace(s)) throw new ArgumentNullException("s");

			//Reading the type
			try
			{
				type = Type.GetType(s, false);
			}
			catch { }

			//if type was not found, try to load by the DataType full name only
			if (type == null)
			{
				foreach (DataType dtype in GetAllDataTypes())
				{
					if (dtype.FullName == s)
					{
						type = dtype.InnerType;
						break;
					}
				}
			}

			//validate argument
			if (type == null)
			{
				throw new ArgumentException("Argument contains an invalid serialized Type: '" + s + "'", "s");
			}

			//Initializing the type and retrieve it 
			this.InitFrom(type);
		}

		#endregion 

		#region Events

		/// <summary>
		/// Delegate for BeforeSelect event
		/// </summary>
		public delegate void BeforeSelectEventHandler(DataType sender, SelectEventArgs e);

		/// <summary>
		/// Delegate for AfterSelect event
		/// </summary>
		public delegate void AfterSelectEventHandler(DataType sender, SelectEventArgs e);

		/// <summary>
		/// Delegate for BeforeSelectGroup event
		/// </summary>
		public delegate void BeforeSelectGroupEventHandler(DataType sender, SelectGroupEventArgs e);

		/// <summary>
		/// Delegate for AfterSelectGroup event
		/// </summary>
		public delegate void AfterSelectGroupEventHandler(DataType sender, SelectGroupEventArgs e);

		/// <summary>
		/// Delegate for insert, delete, and update operation events
		/// </summary>
		public delegate void OperationEventHandler(DataType sender, DataBaseOperationEventArgs e);

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
		/// Raises the BeforeSelect event
		/// </summary>
		internal void OnBeforeSelect(SelectEventArgs e)
		{
			//Raise the DataType event
			if (BeforeSelect != null) BeforeSelect(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnBeforeSelect(e);
		}

		/// <summary>
		/// Raises the AfterSelect event
		/// </summary>
		internal void OnAfterSelect(SelectEventArgs e)
		{
			//Raise the DataType event
			if (AfterSelect != null) AfterSelect(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnAfterSelect(e);
		}

		/// <summary>
		/// Raises the BeforeSelectGroup event
		/// </summary>
		internal void OnBeforeSelectGroup(SelectGroupEventArgs e)
		{
			//Raise the DataType event
			if (BeforeSelectGroup != null) BeforeSelectGroup(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnBeforeSelectGroup(e);
		}

		/// <summary>
		/// Raises the AfterSelect event
		/// </summary>
		internal void OnAfterSelectGroup(SelectGroupEventArgs e)
		{
			//Raise the DataType event
			if (AfterSelectGroup != null) AfterSelectGroup(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnAfterSelectGroup(e);
		}

		/// <summary>
		/// Raises the BeforeInsert event
		/// </summary>
		internal void OnBeforeInsert(DataBaseOperationEventArgs e)
		{
			//Raise the DataType event
			if (BeforeInsert != null) BeforeInsert(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnBeforeInsert(e);
		}

		/// <summary>
		/// Raises the AfterInsert event
		/// </summary>
		internal void OnAfterInsert(DataBaseOperationEventArgs e)
		{
			//Raise the DataType event
			if (AfterInsert != null) AfterInsert(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnAfterInsert(e);
		}

		/// <summary>
		/// Raises the BeforeUpdate event
		/// </summary>
		internal void OnBeforeUpdate(DataBaseOperationEventArgs e)
		{
			//Raise the DataType event
			if (BeforeUpdate != null) BeforeUpdate(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnBeforeUpdate(e);
		}

		/// <summary>
		/// Raises the AfterUpdate event
		/// </summary>
		internal void OnAfterUpdate(DataBaseOperationEventArgs e)
		{
			//Raise the DataType event
			if (AfterUpdate != null) AfterUpdate(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnAfterUpdate(e);
		}

		/// <summary>
		/// Raises the BeforeDelete event
		/// </summary>
		internal void OnBeforeDelete(DataBaseOperationEventArgs e)
		{
			//Raise the DataType event
			if (BeforeDelete != null) BeforeDelete(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnBeforeDelete(e);
		}

		/// <summary>
		/// Raises the AfterDelete event
		/// </summary>
		internal void OnAfterDelete(DataBaseOperationEventArgs e)
		{
			//Raise the DataType event
			if (AfterDelete != null) AfterDelete(this, e);

			//Raise the BaseDataType event
			if (BaseDataType != null) BaseDataType.OnAfterDelete(e);
		}

		#endregion

		#region IComparable Members

		/// <summary>
		/// Compares the current instance with another object of the same type and returns
		/// an integer that indicates whether the current instance precedes, follows,
		/// or occurs in the same position in the sort order as the other object.
		/// </summary>
		/// <param name="obj">An object to compare with this instance.</param>
		/// <returns>
		/// A 32-bit signed integer that indicates the relative order of the objects
		/// being compared. The return value has these meanings: Value Meaning Less than
		/// zero This instance is less than obj. Zero This instance is equal to obj.
		/// Greater than zero This instance is greater than obj.
		/// </returns>
		int IComparable.CompareTo(object obj)
		{
			if (obj == null) throw new ArgumentNullException("obj");
			if (obj is DataType) throw new ArgumentException("Argument is not a DataType", "obj");

			return this.GetHashCode().CompareTo(obj.GetHashCode());
		}

		#endregion
	}
}