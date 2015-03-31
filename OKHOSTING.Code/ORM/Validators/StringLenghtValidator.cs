using System;
using OKHOSTING.Code.ORM.Filters;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Indicates if a Property of Field of type string, must have 
	/// an specific length
	/// </summary>
	/// <remarks>Applies only to string DataValues</remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=true)]
	public class StringLengthValidator: DataValueValidator
	{
		#region Fields 

		/// <summary>
		/// Specify the length that the value must have
		/// in conjunction with the Operator Field
		/// </summary>
		public readonly int Length;

		/// <summary>
		/// Specify the Operator to use in the validation 
		/// of the length 
		/// </summary>
		public readonly CompareOperator Operator;

		/// <summary>
		/// Defines if an string.Empty value is valid
		/// </summary>
		public readonly bool AllowEmpty;

		#endregion 

		#region Constructors 

		/// <summary>
		/// Constructs the validator 
		/// </summary>
		/// <param name="op">
		/// Operator used in the validation
		/// </param>
		/// <param name="length">
		/// Length for the validation
		/// </param>
		public StringLengthValidator(CompareOperator op, int length) : this(op, length, true) { }
		
		/// <summary>
		/// Constructs the validator 
		/// </summary>
		/// <param name="op">
		/// Operator used in the validation
		/// </param>
		/// <param name="length">
		/// Length for the validation
		/// </param>
		/// <param name="allowEmpty">
		/// Defines if an string.Empty value is valid
		/// </param>
		public StringLengthValidator(CompareOperator op, int length, bool allowEmpty)
		{
			this.Length = length;
			this.Operator = op;
			this.AllowEmpty = allowEmpty;
		}

		#endregion

		#region Methods

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

			//Getting the value of the DataValue
			string currentValue = (string) this.GetCurrentValue();

			//if it's null, omit validation
			if (currentValue == null) return null;

			//Perform the applicable validation for the specified operator
			switch (this.Operator)
			{
				case CompareOperator.Equal:
					if (!(currentValue.Length == this.Length))
						error = new ValidationError(this, "String length must be equal than " + this.Length + " on field " + this.DataValue.Name);
					break;

				case CompareOperator.NotEqual:
					if (!(currentValue.Length != this.Length))
						error = new ValidationError(this, "String length must be different than " + this.Length + " on field " + this.DataValue.Name);
					break;

				case CompareOperator.GreaterThan:
					if (!(currentValue.Length > this.Length))
						error = new ValidationError(this, "String length must be greater than than " + this.Length + " on field " + this.DataValue.Name);
					break;

				case CompareOperator.GreaterThanEqual:
					if (!(currentValue.Length >= this.Length))
						error = new ValidationError(this, "String length must be greater or equal than " + this.Length + " on field " + this.DataValue.Name);
					break;

				case CompareOperator.LessThan:
					if (!(currentValue.Length < this.Length))
						error = new ValidationError(this, "String length must be less than " + this.Length + " on field " + this.DataValue.Name);
					break;

				case CompareOperator.LessThanEqual:
					if (!(currentValue.Length <= this.Length))
						error = new ValidationError(this, "String length must be less or equal than " + this.Length + " on field " + this.DataValue.Name);
					break;
			}

			//Validating if the string.Empty value is a valid value (only if dont exists errors))
			if (error == null && !this.AllowEmpty)
			{
				if (currentValue.Trim() == string.Empty)
					error = new ValidationError(this, "String can't be an empty string on field " + this.DataValue.Name);
			}

			//Returning the error or null
			return error;

		}

		/// <summary>
		/// Gets the max lenght of a string DataValue
		/// </summary>
		/// <param name="dvalue">String DataValue that has a StringLengthValidator attribute</param>
		/// <returns>Maximum lenght of the string DataValue. Null if no max lenght is defined.</returns>
		public static int GetMaxLenght(DataValue dvalue)
		{
			//Validating if the MemberInfo is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(StringLengthValidator), false);

			//Evaluating if exists the DataMemberAttribute in the MemberInfo and returning the result
			if (attributes.Length == 0) return NullValues.Int32;

			foreach (StringLengthValidator validator in attributes)
			{
				if (validator.Operator == CompareOperator.Equal)
					return validator.Length;
				else if (validator.Operator == CompareOperator.LessThan)
					return validator.Length - 1;
				else if (validator.Operator == CompareOperator.LessThanEqual)
					return validator.Length;
			}

			//if operator is not one of the previous, return null
			return NullValues.Int32;
		}
		/// <summary>
		/// Gets the min lenght of a string DataValue
		/// </summary>
		/// <param name="dvalue">String DataValue that has a StringLengthValidator attribute</param>
		/// <returns>Maximum lenght of the string DataValue. Null if no max lenght is defined.</returns>
		public static int GetMinLenght(DataValue dvalue)
		{
			//Validating if the MemberInfo is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(StringLengthValidator), false);

			//Evaluating if exists the DataMemberAttribute in the MemberInfo and returning the result
			if (attributes.Length == 0) return NullValues.Int32;

			foreach (StringLengthValidator validator in attributes)
			{
				if (validator.Operator == CompareOperator.Equal)
					return validator.Length;
				else if (validator.Operator == CompareOperator.GreaterThan)
					return validator.Length + 1;
				else if (validator.Operator == CompareOperator.GreaterThanEqual)
					return validator.Length;
			}

			//if operator is not one of the previous, return null
			return NullValues.Int32;
		}

		#endregion 
	}
}