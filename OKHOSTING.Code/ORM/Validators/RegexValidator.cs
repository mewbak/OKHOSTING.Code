using System;
using System.Text.RegularExpressions;

namespace OKHOSTING.Code.ORM.Validators
{
	
	/// <summary>
	/// Implements a validation of regular expressions
	/// </summary>
	/// <remarks>Applies only to string DataValues</remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class RegexValidator: DataValueValidator
	{
		#region Fields

		/// <summary>
		/// Regular expression used to validate
		/// </summary>
		public readonly string Pattern;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the validaror
		/// </summary>
		/// <param name="pattern">
		/// Regular expression used to validate
		/// </param>
		public RegexValidator(string pattern)
		{
			//Validating if the Pattern is null 
			if (pattern == null) throw new ArgumentNullException("pattern");

			//Initializing the validator
			this.Pattern = pattern;
		}

		#endregion

		#region Validator Implementation

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
			string currentValue = (string) this.GetCurrentValue();

			//if null, exit
			if (string.IsNullOrWhiteSpace(currentValue)) return null;

			//Performing the validation
			Regex regEx = new Regex(this.Pattern);
			
			//if doesnt match..
			if (!regEx.IsMatch(currentValue)) 
				error = new ValidationError(this, "The DataValue " + this.DataValue.Name + " doesn't match with the regular expression " + this.Pattern);

			//Returning the error or null
			return error;
		}

		/// <summary>
		/// Determines whether RegexValidator attibute is defined in a DataValue
		/// </summary>
		public static bool IsImplemented(DataValue dvalue)
		{
			//Validating if the dvalue is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");
			
			return DataValueValidator.IsDefined(dvalue.InnerMember, typeof(RegexValidator));
		}

		/// <summary>
		/// If RegexValidator is defined in the DataValue, the regex pattern is returned.
		/// If no RegexValidator is found, null is returned.
		/// </summary>
		public static string GetRegexPattern(DataValue dvalue)
		{
			//Validating if the dvalue is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(RegexValidator), false);

			//Evaluating if exists the Attribute in the DataValue and returning the result
			if (attributes.Length == 0)
				return null;
			else
				return ((RegexValidator)attributes[0]).Pattern;
		}

		#endregion
	}
}