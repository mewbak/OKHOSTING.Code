using System;
using OKHOSTING.Code.ORM.Filters;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Base class for comparison validators
	/// </summary>
	public abstract class CompareValidator: DataValueValidator
	{

		#region Fields 

		/// <summary>
		/// Operator to use in the validation 
		/// </summary>
		public readonly CompareOperator Operator;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs th validator
		/// </summary>
		/// <param name="op">
		/// Operator to use in the validation
		/// </param>
		protected CompareValidator(CompareOperator op)
		{
			this.Operator = op;
		}

		#endregion

		#region Abstract methods

		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		public abstract override ValidationError Validate();

		#endregion

		#region Support functions for validation implementation

		/// <summary>
		/// Compare the value of the associated DataValue with the 
		/// specified value and using the indicated operator, returns
		/// an ValidationError if the validation fails, or null if it's success
		/// </summary>
		/// <param name="valueToCompare">
		/// Value for comparison
		/// </param>
		/// <returns>
		/// ValidationError if the validation fails, or null if it's success
		/// </returns>
		protected ValidationError Validate(IComparable valueToCompare)
		{
			//Local Vars
			ValidationError error = null;

			//Validating if the valueToCompare is null
			if (valueToCompare == null) throw new ArgumentNullException("valueToCompare");

			//Loading the value of associated DataValue and comparing with the specified value
			IComparable toValidate = (IComparable)this.DataValue.GetValue(this.DataObject);
			int compareResult = toValidate.CompareTo(valueToCompare);
			
			//Perform the validation in function of the established operator
			switch(this.Operator)
			{
				case CompareOperator.Equal:
					if(compareResult != 0)
						error = new ValidationError(this, this.DataValue.Name + " value must be equal than " + valueToCompare);
					break;
				
				case CompareOperator.NotEqual:
					if(compareResult == 0)
						error = new ValidationError(this, this.DataValue.Name + " value must be different than " + valueToCompare);
					break;

				case CompareOperator.GreaterThan:
					if(compareResult <= 0)
						error = new ValidationError(this, this.DataValue.Name + " value must be greater than than " + valueToCompare);
					break;

				case CompareOperator.GreaterThanEqual:
					if(compareResult < 0)
						error = new ValidationError(this, this.DataValue.Name + " value must be greater or equal than " + valueToCompare);
					break;

				case CompareOperator.LessThan:
					if(compareResult >= 0)
						error = new ValidationError(this, this.DataValue.Name + " value must be less than " + valueToCompare);
					break;

				case CompareOperator.LessThanEqual:
					if(compareResult > 0)
						error = new ValidationError(this, this.DataValue.Name + " value must be less or equal than " + valueToCompare);
					break;
			}
			
			return error;
		}

		#endregion

	}
}