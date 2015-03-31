using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Exception that is thrown when a database operation is tried to be performed
	/// with a DataType that is not supported with the current DataBase
	/// </summary>
	public class DataTypeNotSupportedException: Exception
	{
		/// <summary>
		/// DataType that is not supported by the current DataBase
		/// </summary>
		public readonly DataType DataType;
		
		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="dtype">DataType that is not supported by the current DataBase</param>
		public DataTypeNotSupportedException(DataType dtype): this(dtype, null)
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="dtype">DataType that is not supported by the current DataBase</param>
		/// <param name="message">Custom exception message</param>
		public DataTypeNotSupportedException(DataType dtype, string message): this(dtype, message, null)
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="dtype">DataType that is not supported by the current DataBase</param>
		/// <param name="message">Custom exception message</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public DataTypeNotSupportedException(DataType dtype, string message, Exception innerException): base(message, innerException)
		{
			DataType = dtype;
		}
	}
}
