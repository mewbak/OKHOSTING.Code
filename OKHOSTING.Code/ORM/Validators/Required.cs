using System;
using System.Reflection;
using OKHOSTING.Code.ORM;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Validate if a Property of Field on a Class can be null
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class Required : DataValueValidator
	{
		/// <summary>
		/// Constructs the attribute
		/// </summary>
		public Required()
		{
		}

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

			//Recover the value of DataValue associated
			object currentValue = this.GetCurrentValue();

			//Validating if the value is null
			if (NullValues.IsNull(currentValue))
			{
				error = new ValidationError(this, this.DataValue.Name + " cannot be null");
			}

			//if this is a string, do not allow null nor empty values
			else if (this.DataValue.ValueType.Equals(typeof(string)))
			{
				if (string.IsNullOrWhiteSpace((string)currentValue))
				{
					error = new ValidationError(this, this.DataValue.Name + " cannot be empty");
				}
			}

			//Returning the error or null
			return error;
		}

		/// <summary>
		/// Indicates if a DataValue is required or not
		/// </summary>
		/// <param name="dvalue">DataValue that will be searched for Required attribute</param>
		/// <returns>True if DataValue is required only, false otherwise</returns>
		public static bool IsRequired(DataValue dvalue)
		{
			//Validating if the MemberInfo is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//if primary key, return true always
			if (dvalue.IsPrimaryKey) return true;

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(Required), false);

			//Evaluating if exists the Attribute in the DataValue and returning the result
			if (attributes.Length == 0)
				return false;
			else
				return true;
		}

		#endregion
	}
}