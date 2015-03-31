using System;
using OKHOSTING.Code.ORM.Filters;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Defines a validation based on the comparison between
	/// two DataValue's
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=true)]
	public class DataValueCompareValidator: CompareValidator
	{

		#region Fields

		/// <summary>
		/// DataValue to compare with the DataValue of the validator
		/// </summary>
		public readonly DataValue DataValueToCompare;

		#endregion 

		#region Constructors

		/// <summary>
		/// Constructs the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// DataValue to compare with the DataValue of the validator
		/// </param>
		public DataValueCompareValidator(CompareOperator op, DataValue dataValueToCompare): base(op)
		{
			//Validating if dataValueToCompare argument is null
			if (dataValueToCompare == null) throw new ArgumentNullException("dataValueToCompare");

			//Initializing validator
			this.DataValueToCompare = dataValueToCompare;
		}

		#endregion

		#region Validation Implementation

		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		public override ValidationError Validate()
		{
			//Local Vars
			ValidationError error = null;

			//Validating if the DataObject instance to validate was specified
			if (this.DataObject == null)
				throw new InvalidOperationException("Current validater DataObject property can't be null");

			//Getting the Value to compare
			object DataValueToCompareValue = this.DataObject.GetValue(this.DataValueToCompare);

			//Converting the value to an IComparable interface
			IComparable toCompare = (IComparable)DataValueToCompareValue;

			//Validating
			error = base.Validate(toCompare);
			
			//Returning the applicable error or null...
			return error;
		}

		#endregion

	}
}