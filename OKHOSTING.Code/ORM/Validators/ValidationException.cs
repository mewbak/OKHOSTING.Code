using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OKHOSTING.Code.ORM.Validators
{

	/// <summary>
	/// Defines an exception caused by a validator
	/// </summary>
	public class ValidationException : Exception
	{

		#region Fields

		/// <summary>
		/// Array of errors that causes the exception throw
		/// </summary>
		public readonly List<ValidationError> ValidationErrors;

		/// <summary>
		/// Referece to DataObject that fails on it validation
		/// </summary>
		public readonly DataObject ValidatedDataObject;

		#endregion

		#region Constructors 

		/// <summary>
		/// Constructs the exception
		/// </summary>
		/// <param name="validationError">
		/// Error that causes the exception throw
		/// </param>
		/// <param name="validatedDataObject">
		/// Referece to DataObject that fails on it validation
		/// </param>
		public ValidationException(ValidationError validationError, DataObject validatedDataObject) :this(new List<ValidationError> { validationError }, validatedDataObject, string.Empty)
		{ 
		}

		/// <summary>
		/// Constructs the exception
		/// </summary>
		/// <param name="validationError">
		/// Error that causes the exception throw
		/// </param>
		/// <param name="validatedDataObject">
		/// Referece to DataObject that fails on it validation
		/// </param>
		/// <param name="message">
		/// Custom Error message
		/// </param>
		public ValidationException(ValidationError validationError, DataObject validatedDataObject, string message) :this(new List<ValidationError> { validationError }, validatedDataObject, message) 
		{ 
		}

		/// <summary>
		/// Constructs the exception
		/// </summary>
		/// <param name="validationErrors">
		/// Array of errors that causes the exception throw
		/// </param>
		/// <param name="validatedDataObject">
		/// Referece to DataObject that fails on it validation
		/// </param>
		public ValidationException(List<ValidationError> validationErrors, DataObject validatedDataObject) : this(validationErrors, validatedDataObject, string.Empty) 
		{ 
		}
	
		/// <summary>
		/// Constructs the exception
		/// </summary>
		/// <param name="validationErrors">
		/// Array of errors that causes the exception throw
		/// </param>
		/// <param name="validatedDataObject">
		/// Referece to DataObject that fails on it validation
		/// </param>
		/// <param name="message">
		/// Custom Error message
		/// </param>
		public ValidationException(List<ValidationError> validationErrors, DataObject validatedDataObject, string message): base(message)
		{
			this.ValidationErrors = validationErrors;
			this.ValidatedDataObject = validatedDataObject;
		}

		#endregion

		#region Exception Overrides

		/// <summary>
		/// Returns the exception represented as a string
		/// </summary>
		/// <returns>
		/// Equivalent to this.Message
		/// </returns>
		public override string ToString()
		{
			return this.Message;
		}

		/// <summary>
		/// Returns the complete error message of the exception 
		/// (from all Errors contained on ValidationErrors array)
		/// </summary>
		public override string Message
		{
			get
			{
				//Local Vars
				string msg;

				//Initializing error message
				msg = base.Message + "\n";

				//Crossing all the exceptions and completing the message
				foreach (ValidationError error in this.ValidationErrors)
				{
					msg += error.Description + "\n";
				}
				
				msg += "\n DataObject:\n" + TypeConverter.SerializeToString((IXmlSerializable) this.ValidatedDataObject);

				//Returning the message
				return msg;
			}
		}

		#endregion
	
	}
}