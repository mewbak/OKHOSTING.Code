using System;

namespace OKHOSTING.Code.ORM
{

	/// <summary>
	/// Exception that throws when a DataType is not correctly defined
	/// </summary>
	public class InvalidDataTypeException: Exception
	{

		/// <summary>
		/// Type that is not implemented as a DataType
		/// </summary>
		public readonly Type type;

		/// <summary>
		/// Constructs the exception
		/// </summary>
		/// <param name="type">
		/// Type that is not implemented as a DataType
		/// </param>
		/// <param name="message">
		/// Error message
		/// </param>
		public InvalidDataTypeException(Type type, string message): base(message)
		{
			this.type= type;
		}

	}
}