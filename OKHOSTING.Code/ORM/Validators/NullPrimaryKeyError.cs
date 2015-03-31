using System;

namespace OKHOSTING.Code.ORM.Validators
{

	/// <summary>
	/// Defines a primary key validation error
	/// </summary>
	public class NullPrimaryKeyError: ValidationError
	{

		/// <summary>
		/// DataValue that is wrong defined and that 
		/// is part of the primary key
		/// </summary>
		public readonly DataValue DataValue;

		/// <summary>
		/// Constructs the error object
		/// </summary>
		/// <param name="dataValue">
		/// DataValue that is wrong defined and that 
		/// is part of the primary key
		/// </param>
		/// <param name="validator">
		/// Reference to the validator that fails
		/// </param>
		/// <param name="message">
		/// Description of the error
		/// </param>
		public NullPrimaryKeyError(DataValue dataValue, IValidator validator, string message): base(validator, message)
		{
			//Validating if the dataValue argument is null
			if (dataValue == null) throw new ArgumentNullException("dataValue");

			//Initializing the error
			this.DataValue = dataValue;
		}

	}
}