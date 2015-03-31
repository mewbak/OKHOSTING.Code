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
	/// Defines a sorting order for a DataMember or DataType
	/// </summary>
	/// <remarks>Usefull for sorting DataMembers and DataTypes on user interfaces</remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false)]
	public class SortOrderAttribute: Attribute
	{
		/// <summary>
		/// Sorting order
		/// </summary>
		public readonly uint SortOrder;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="sortOrder">Sorting order</param>
		public SortOrderAttribute(uint sortOrder)
		{
			SortOrder = sortOrder;
		}

		/// <summary>
		/// Returns a sorting order for a DataType
		/// </summary>
		public static uint GetSortOrder(DataType dtype)
		{
			//Validating if the MemberInfo is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dtype.InnerType.GetCustomAttributes(typeof(SortOrderAttribute), false);

			//Evaluating if exists the attribute in the MemberInfo and returning the result
			if (attributes.Length == 0)
				return UInt32.MaxValue;
			else
				return ((SortOrderAttribute)attributes[0]).SortOrder;
		}

		/// <summary>
		/// Returns a sorting order for a DataMember
		/// </summary>
		public static uint GetSortOrder(DataMember dmember)
		{
			//Validating if the MemberInfo is null
			if (dmember == null) throw new ArgumentNullException("dmember");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dmember.InnerMember.GetCustomAttributes(typeof(SortOrderAttribute), false);

			//Evaluating if exists the attribute in the MemberInfo and returning the result
			if (attributes.Length == 0)
				return UInt32.MaxValue;
			else
				return ((SortOrderAttribute)attributes[0]).SortOrder;
		}
	}
}
