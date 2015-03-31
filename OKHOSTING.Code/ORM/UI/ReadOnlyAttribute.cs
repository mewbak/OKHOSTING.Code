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
using System.Text;

namespace OKHOSTING.Code.ORM.UI
{
	/// <summary>
	/// Defines a DataValue as readonly
	/// </summary>
	/// <remarks>
	/// When a DataValue is readonly, no editing is allow by the user, and in Inset operations the DataValue is not askes to the user in interfaces
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class ReadOnlyAttribute : Attribute
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public ReadOnlyAttribute()
		{
		}

		/// <summary>
		/// Indicates if a DataValue is readonly or not
		/// </summary>
		/// <param name="dvalue">DataValue that will be searched for ReadOnlyAttribute attribute</param>
		/// <returns>True if DataValue is read only, false otherwise</returns>
		public static bool IsReadOnly(DataValue dvalue)
		{
			//Validating if the dvalue is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//if datavalue is an auto-increment primary key, return true
			if (dvalue.AutoIncrement) return true;

			//If DataType is Ideleted, return true for "Deleted" DataValue
			if (dvalue.DeclaringDataType.InnerType.GetInterface(typeof(IDeleted).FullName) != null && dvalue.Name == "Deleted") return true;

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(ReadOnlyAttribute), false);

			//Evaluating if exists the Attribute in the DataValue and returning the result
			if (attributes.Length == 0)
				return false;
			else
				return true;
		}
	}
}