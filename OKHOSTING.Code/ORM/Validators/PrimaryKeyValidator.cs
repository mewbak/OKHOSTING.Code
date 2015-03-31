using System;
using System.Collections.Generic;

namespace OKHOSTING.Code.ORM.Validators
{

	/// <summary>
	/// Validate if the primary key of an DataObject is correctly defined
	/// </summary>
	public class PrimaryKeyValidator: IValidator
	{

		#region Fields 

		/// <summary>
		/// DataObject for primary key validation
		/// </summary>
		public readonly DataObject DataObject;

		/// <summary>
		/// DataBaseOperation that will be performed. Affects the way validation is done.
		/// </summary>
		public readonly DataBaseOperation Operation;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the validator
		/// </summary>
		/// <param name="dobj">
		/// DataObject for primary key validation
		/// </param>
		/// <param name="operation">DataBaseOperation that will be performed. Affects the way validation is done.</param>
		public PrimaryKeyValidator(DataObject dobj, DataBaseOperation operation)
		{
			//Validating arguments
			if (dobj == null) throw new ArgumentNullException("dobj");

			//Initializing the validator
			this.DataObject = dobj;
			this.Operation = operation;
		}

		#endregion

		#region Validation Implementation
		
		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// Error information if validation fails, otherwise null
		/// </returns>
		public ValidationError Validate()
		{
			//Local Vars
			NullPrimaryKeyError error = null;

			//Crossing the DataValue's on the collection
			foreach (DataValue dv in this.DataObject.DataType.PrimaryKey)
			{
				bool isNull;

				//if this is a string, do not allow null nor empty values
				if (dv.ValueType.Equals(typeof(string)))
				{
					isNull = string.IsNullOrWhiteSpace((string) dv.GetValue(this.DataObject));
				}
				else
				{
					isNull = NullValues.IsNull(dv.GetValue(this.DataObject));
				}

				if (!dv.AutoIncrement && isNull)
				{
					error = new NullPrimaryKeyError(dv, this, "PrimaryKey contains a null value");
				}
				
				//If an error exists, break and return the error
				if (error != null) return error;
			}
			
			//If no error was found, return null
			return null;
		}

		#endregion
	}
}