using System;
using System.Reflection;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Represents a class field, equivalent to one column on a table
	/// </summary>
	/// <remarks>When building DataObjects, each DataObject field represents a column on a table</remarks>
	[Serializable]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DataField: DataValue
	{
		#region Constructors


		/// <summary>
		/// Constructs the DataField 
		/// </summary>
		public DataField(): base()
		{
		}

		/// <summary>
		/// Constructs the DataField 
		/// </summary>
		/// <param name="isPrimaryKey">Indicates if the value is part of the primary key of the entity</param>
		public DataField(bool isPrimaryKey): base(isPrimaryKey)
		{
		}

		/// <summary>
		/// Constructs the DataField 
		/// </summary>
		/// <param name="isPrimaryKey">Indicates if the value is part of the primary key of the entity</param>
		/// <param name="autoIncrement">Indicates if the value is auto incremental (only works for Integral, primary key DataValues)</param>
		public DataField(bool isPrimaryKey, bool autoIncrement): base(isPrimaryKey, autoIncrement) 
		{
		}

		/// <summary>
		/// Constructs the DataField 
		/// </summary>
		/// <param name="columnName">
		/// Name of the column on database table that represents the DataValue
		/// (if null or empty, the default column name is used). The name here
		/// specified, have priority over the standard name and over the type 
		/// prefixes established for the DataValue's DataObject
		/// </param>
		public DataField(bool isPrimaryKey, bool autoIncrement, string columnName): base(isPrimaryKey, autoIncrement, columnName)
		{
		}


		#endregion

		#region Implementation

		/// <summary>
		/// Returns the FieldInfo that this instance is representing
		/// </summary>
		public new FieldInfo InnerMember
		{
			get
			{
				return (FieldInfo)base.InnerMember;
			}
		}

		/// <summary>
		/// Returns the type of the value
		/// </summary>
		public override Type ValueType
		{
			get
			{
				return InnerMember.FieldType;
			}
		}

		/// <summary>
		/// Indicates if the current DataField is static
		/// </summary>
		public override bool IsStatic
		{
			get
			{
				return this.InnerMember.IsStatic;
			}
		}

		/// <summary>
		/// Returns whether DataField's attibute is declared in a class's FieldInfo	
		/// </summary>
		public static bool IsImplementedBy(FieldInfo field)
		{
			return DataMember.IsImplementedBy(field, typeof(DataField));
		}

		/// <summary>
		/// Returns the value of the DataField
		/// </summary>
		/// <param name="dobj">
		/// DataObject that will be examined
		/// </param>
		/// <returns>
		/// The value of the current DataField in the specified DataObject
		/// </returns>
		public override object GetValue(DataObject dobj)
		{
			//Validating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Returning the value
			return this.InnerMember.GetValue(dobj);
		}

		/// <summary>
		/// Sets the value of the DataField
		/// </summary>
		/// <param name="dobj">
		/// DataObject that will be changed
		/// </param>
		/// <param name="value">
		/// The value that will be set to the DataField
		/// </param>
		public override void SetValue(DataObject dobj, object value)
		{
			//Validating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}
			
			//Establishing the value
			this.InnerMember.SetValue(dobj, value);
		}

		#endregion

	}
}