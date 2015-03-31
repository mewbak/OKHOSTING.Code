using System;
using System.Reflection;

namespace OKHOSTING.Code.ORM
{

	/// <summary>
	/// Represents a class field or property, equivalent to one column on a table
	/// </summary>
	/// <remarks>
	/// When building DataObjects, each DataObject field or property tagged with this attribute,
	/// represents a column on a table
	/// </remarks>
	[Serializable]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public abstract class DataValue : DataMember
	{
		#region Constructors

		/// <summary>
		/// Constructs the DataValue 
		/// </summary>
		protected DataValue() : this(false, false, null) 
		{
		}

		/// <summary>
		/// Constructs the DataValue 
		/// </summary>
		/// <param name="isPrimaryKey">Indicates if the value is part of the primary key of the entity</param>
		protected DataValue(bool isPrimaryKey): this(isPrimaryKey, false, null)
		{
		}

		/// <summary>
		/// Constructs the DataValue 
		/// </summary>
		/// <param name="isPrimaryKey">Indicates if the value is part of the primary key of the entity</param>
		/// <param name="autoIncrement">Indicates if the value is auto incremental (only works for Integral, primary key DataValues)</param>
		protected DataValue(bool isPrimaryKey, bool autoIncrement): this(isPrimaryKey, autoIncrement, null) 
		{
		}

		/// <summary>
		/// Constructs the DataValue 
		/// </summary>
		/// <param name="columnName">
		/// Name of the column on database table that represents the DataValue
		/// (if null or empty, the default column name is used). The name here
		/// specified, have priority over the standard name and over the type 
		/// prefixes established for the DataValue's DataObject
		/// </param>
		protected DataValue(bool isPrimaryKey, bool autoIncrement, string columnName)
		{
			this.isPrimaryKey = isPrimaryKey;
			this.autoIncrement = autoIncrement;
			this.columnName = columnName;
		}

		#endregion

		#region Fields and Properties

		private readonly bool isPrimaryKey;
		private readonly bool autoIncrement;
		private readonly string columnName;
		
		/// <summary>
		/// Indicates if the value is part of the primary key of the entity
		/// </summary>
		public bool IsPrimaryKey
		{
			get
			{
				return isPrimaryKey;
			}
		}

		/// <summary>
		/// Indicates if the value is auto incremental (only works for Integral, primary key DataValues)
		/// </summary>
		public bool AutoIncrement
		{
			get
			{
				return autoIncrement;
			}
		}

		/// <summary>
		/// Name of the column on database table that represents the DataValue
		/// (if null or empty, the default column name is used). The name here
		/// specified, have priority over the standard name and over the type 
		/// prefixes established for the DataValue's DataObject
		/// </summary>
		
		/// <summary>
		/// Name of the column on database table that represents the DataValue
		/// (if null or empty, the default column name is used). The name here
		/// specified, have priority over the standard name and over the type 
		/// prefixes established for the DataValue's DataObject
		/// </summary>
		public string ColumnName
		{
			get
			{
				if (columnName != null)
					return columnName;
				else
					return Name;
			}
		}

		#endregion 

		#region Abstract Properties

		/// <summary>
		/// Returns the type of the value
		/// </summary>
		public abstract Type ValueType { get; }

		/// <summary>
		/// Returns the value of this DataValue
		/// </summary>
		/// <param name="dobj">
		/// DataObject that will be examined
		/// </param>
		/// <returns>
		/// The value of the current DataValue in the specified DataObject
		/// </returns>
		public abstract object GetValue(DataObject dobj);

		/// <summary>
		/// Sets the value for this DataValue
		/// </summary>
		/// <param name="dobj">
		/// DataObject that will be changed
		/// </param>
		/// <param name="value">
		/// The value that will be set to the DataValue
		/// </param>
		public abstract void SetValue(DataObject dobj, object value);

		#endregion

		/// <summary>
		/// Returns true when the current ValueMember is a DataObject, false otherwise
		/// </summary>
		public bool IsForeignKey
		{
			get
			{
				return DataType.IsDataObject(this.ValueType);
			}
		}

		/// <summary>
		/// Returns true if the current DataValue's is a numeric Type, false otherwise
		/// </summary>
		/// <remarks>
		/// Used for making automatic statistics and calculations
		/// </remarks>
		public bool IsNumeric
		{
			get
			{
				//Validating if is an enum
				if (this.ValueType.IsEnum)
				{
					return false;
				}

				//Validating if is primary key
				else if (IsPrimaryKey)
				{
					return false;
				}
				else
				{
					//Validating if is numeric
					switch (Type.GetTypeCode(ValueType))
					{
						case TypeCode.Byte:
						case TypeCode.Char:
						case TypeCode.Decimal:
						case TypeCode.Double:
						case TypeCode.Int16:
						case TypeCode.Int32:
						case TypeCode.Int64:
						case TypeCode.SByte:
						case TypeCode.Single:
						case TypeCode.UInt16:
						case TypeCode.UInt32:
						case TypeCode.UInt64:
							return true;

						default:
							return false;
					}
				}
			}
		}
	}
}
