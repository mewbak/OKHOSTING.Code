using System;
using System.Collections.Generic;
using System.Text;

namespace OKHOSTING.Code.ORM.UI
{
	/// <summary>
	/// Specifies as DataValue that must be displayed in a "Summary" view nof the DataObject, in a list or detail page
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class SummaryViewAttribute: Attribute
	{
		/// <summary>
		/// Determines that a DataValue should be included in a Summary view of the DataObject
		/// </summary>
		public SummaryViewAttribute()
		{
		}

		/// <summary>
		/// Indicates if a DataValue has SummaryViewAttribute defined
		/// </summary>
		/// <param name="dvalue">DataValue that will be searched for SummaryViewAttribute attribute</param>
		/// <returns>True if DataValue has SummaryViewAttribute, false otherwise</returns>
		public static bool IsSummaryView(DataValue dvalue)
		{
			//Validating if the MemberInfo is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(SummaryViewAttribute), false);

			//Evaluating if exists the Attribute in the DataValue and returning the result
			if (attributes.Length == 0)
				return false;
			else
				return true;
		}
	}
}