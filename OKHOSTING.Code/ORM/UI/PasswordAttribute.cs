using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM.UI
{
	/// <summary>
	/// Defines a DataValue as a password, which will cause it will be 
	/// displayed as a password text field in user interfaces (text will be hidden from user)
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class PasswordAttribute : Attribute
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public PasswordAttribute()
		{
		}

		/// <summary>
		/// Indicates if a DataValue is a password or not
		/// </summary>
		/// <param name="dvalue">DataValue that will be searched for PasswordAttribute attribute</param>
		/// <returns>True if DataValue is a password, false otherwise</returns>
		public static bool IsPassword(DataValue dvalue)
		{
			//Validating if the dvalue is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(PasswordAttribute), false);

			//Evaluating if exists the Attribute in the DataValue and returning the result
			if (attributes.Length == 0)
				return false;
			else
				return true;
		}
	}
}
