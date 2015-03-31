using System;
using System.Reflection;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Represents a class property, equivalent to one column on a table
	/// </summary>
	/// <remarks>When building DataObjects, each DataObject property represents a column on a table</remarks>
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DataProperty : DataValue
	{
		#region Constructors

		/// <summary>
		/// Constructs the DataProperty 
		/// </summary>
		public DataProperty() : base()
		{
		}

		/// <summary>
		/// Constructs the DataProperty 
		/// </summary>
		/// <param name="isPrimaryKey">Indicates if the value is part of the primary key of the entity</param>
		public DataProperty(bool isPrimaryKey): base(isPrimaryKey)
		{
		}

		/// <summary>
		/// Constructs the DataProperty 
		/// </summary>
		/// <param name="isPrimaryKey">Indicates if the value is part of the primary key of the entity</param>
		/// <param name="autoIncrement">Indicates if the value is auto incremental (only works for Integral, primary key DataValues)</param>
		public DataProperty(bool isPrimaryKey, bool autoIncrement): base(isPrimaryKey, autoIncrement) 
		{
		}

		/// <summary>
		/// Constructs the DataProperty 
		/// </summary>
		/// <param name="columnName">
		/// Name of the column on database table that represents the DataValue
		/// (if null or empty, the default column name is used). The name here
		/// specified, have priority over the standard name and over the type 
		/// prefixes established for the DataValue's DataObject
		/// </param>
		public DataProperty(bool isPrimaryKey, bool autoIncrement, string columnName): base(isPrimaryKey, autoIncrement, columnName)
		{
		}

		#endregion

		#region Implementation

		/// <summary>
		/// Returns the PropertyInfo that this instance is representing
		/// </summary>
		public new PropertyInfo InnerMember
		{
			get
			{
				return (PropertyInfo) base.InnerMember;
			}
		}

		/// <summary>
		/// Returns the type of the value
		/// </summary>
		public override Type ValueType
		{
			get
			{
				return InnerMember.PropertyType;
			}
		}

		/// <summary>
		/// Indicates if the current DataProperty is static
		/// </summary>
		public override bool IsStatic
		{
			get
			{
				return
				  (InnerMember.CanRead && InnerMember.GetGetMethod().IsStatic)
				  ||
				  (InnerMember.CanWrite && InnerMember.GetSetMethod().IsStatic);
			}
		}

		/// <summary>
		/// Returns whether DataProperty's attibute is declared in a class's PropertyInfo	
		/// </summary>
		public static bool IsImplementedBy(PropertyInfo propertyInfo)
		{
			return DataMember.IsImplementedBy(propertyInfo, typeof(DataProperty));
		}

		/// <summary>
		/// Returns the value of the DataProperty
		/// </summary>
		/// <param name="dobj">
		/// DataObject that will be examined
		/// </param>
		/// <returns>
		/// The value of the current DataProperty in the specified DataObject
		/// </returns>
		public override object GetValue(DataObject dobj)
		{
			//Validating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Returning the value
			return this.InnerMember.GetValue(dobj, null);
		}

		/// <summary>
		/// Sets the value of the DataProperty
		/// </summary>
		/// <param name="dobj">
		/// DataObject that will be changed
		/// </param>
		/// <param name="value">
		/// The value that will be set to the DataProperty
		/// </param>
		public override void SetValue(DataObject dobj, object value)
		{
			//Validating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Establishing the value
			this.InnerMember.SetValue(dobj, value, null);
		}

		#endregion
	}
}