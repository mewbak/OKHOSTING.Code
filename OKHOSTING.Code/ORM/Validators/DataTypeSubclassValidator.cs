using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Validates that a DataValue of type DataType contains a DataType 
	/// that is a specific DataType or a subclass of it.
	/// </summary>
	/// <example>
	/// Use this attribute on a DataValues od type DataType where you want the DataType to be a subclass of a specific DataType only.
	/// PE: you have a DataValue Product.ProductInstanceType where you want the selected DataType to be ProductInstance or a subclass of ProductInstance only
	/// </example>
	/// <remarks>
	/// Applies only DataValues of type DataType
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false)]
	public class DataTypeSubclassValidator : DataValueValidator
	{
		/// <summary>
		/// The DataType (or a subclass of it) that must be selected as a value of the DataValue
		/// </summary>
		public readonly DataType Parent;

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="parent">
		/// DataType (or any subclass of it) that can be selected as the Value of the DataValue
		/// </param>
		public DataTypeSubclassValidator(DataType parent)
		{
			//Validating arguments
			if (parent == null) throw new ArgumentNullException("parent");

			//Initializing validator
			this.Parent = parent;
		}

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="parent">
		/// DataType (or any subclass of it) that can be selected as the Value of the DataValue
		/// </param>
		public DataTypeSubclassValidator(Type parent)
		{
			//Validating arguments
			if (parent == null) throw new ArgumentNullException("parent");

			//Initializing validator
			this.Parent = parent;
		}

		/// <summary>
		/// If RegexValidator is defined in the DataValue, the regex pattern is returned.
		/// If no RegexValidator is found, null is returned.
		/// </summary>
		public static DataType GetParent(DataValue dvalue)
		{
			//Validating if the dvalue is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(DataTypeSubclassValidator), false);

			//Evaluating if exists the Attribute in the DataValue and returning the result
			if (attributes.Length == 0)
			{
				return null;
			}
			else
			{
				return ((DataTypeSubclassValidator)attributes[0]).Parent;
			}
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

			//Getting the current value of the associated DataValue
			DataType val = (DataType)this.GetCurrentValue();

			//Do not validate null values
			if (val == null) return null;

			//Verifying if the value is equal to the Parent DataType or is a subclass of it
			if (!val.Equals(Parent) || !val.InnerType.IsSubclassOf(Parent.InnerType))
			{
				error = new ValidationError(this, "DataType " + val + " is not equal or a subclass of " + Parent);
			}

			//Returning the applicable error or null
			return error;
		}

		#endregion
	}
}