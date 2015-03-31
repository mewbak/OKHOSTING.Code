using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Defines a custom table name for a DataType. 
	/// If no TableName attribute is defined in a DataType, 
	/// the default TableName is used as defined in Configuration.TableNameFormat
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class TableNameAttribute : System.Attribute
	{
		/// <summary>
		/// Table name of the DataObject, or null 
		/// if the default name is used
		/// </summary>
		public readonly string TableName;

		/// <summary>
		/// Constructs the attribute
		/// </summary>
		/// <param name="tableName">
		/// Custom table name of the DataType
		/// </param>
		public TableNameAttribute(string tableName)
		{
			if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException("tableName", "Argument can't be empty or null");

			TableName = tableName;
		}
	}
}