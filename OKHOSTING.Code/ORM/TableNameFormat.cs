using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Defines the way in wich the DataType tables will be named in the DataBase.
	/// This format is used 
	/// </summary>
	public enum TableNameFormat
	{
		/// <summary>
		/// DataType.Name will be used by default, as the table name
		/// </summary>
		/// <example>OKHOSTING.Code.ORM.Core.Person => Person</example>
		DataTypeName,

		/// <summary>
		/// DataType.FullName will be used by default, as the table name, replacing character '.' with '_'
		/// </summary>
		/// <example>OKHOSTING.Code.ORM.Core.Person => OKHOSTING.Code.ORM_Core_Person</example>
		DataTypeFullName,

		/// <summary>
		/// DataType last namespace and name will be used by default, as the table name, replacing character '.' with '_'
		/// </summary>
		/// <example>OKHOSTING.Code.ORM.Core.Person => Core_Person</example>
		DataTypeLastNameSpaceAndName,
	}
}