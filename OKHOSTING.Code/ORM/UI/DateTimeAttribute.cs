/*

Copyright 2003-2010 OK HOSTING S.C.
info@okhosting.com
okhosting.com
 
Authors: Edgard David Iván Muñoz Chávez, Leopoldo Arenas Flores

This file is part of Softosis.

Softosis is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

Softosis is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with Softosis.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM.UI
{
	/// <summary>
	/// Defines wether a DateTime Datavalue should include time in user interfaces
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class DateTimeAttribute : Attribute
	{
		/// <summary>
		/// Defines wether or not the DataValue should be displayed with time or not
		/// </summary>
		public readonly bool IncludeTime;

		/// <summary>
		/// Defines a category for a DatatType or DataMember, usefull for grouping in user interfaces
		/// </summary>
		/// <param name="includeTime">Defines wether or not the DataValue should be displayed with time or not</param>
		public DateTimeAttribute(bool includeTime)
		{
			IncludeTime = includeTime;
		}

		/// <summary>
		/// Gets a value indicating if a DataValue contains CurrencyAttribute, and should be displayed as currency
		/// </summary>
		public static bool GetIncludeTime(DataValue dvalue)
		{
			//validating if the MemberInfo is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");
			
			//validating if DataValue is DateTime
			if (!dvalue.ValueType.Equals(typeof(DateTime))) return false; ;

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(DateTime), false);

			//Evaluating if exists the DataMemberAttribute in the MemberInfo and returning the result
			if (attributes.Length == 0)
				return true;
			else
				return ((DateTimeAttribute)attributes[0]).IncludeTime;
		}
	}
}
