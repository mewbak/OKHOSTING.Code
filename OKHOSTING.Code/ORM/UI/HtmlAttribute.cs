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
	/// Defines that a string DataValue should be parsed as HTML on user interface
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class HtmlAttribute : Attribute
	{
		/// <summary>
		/// Indicates if a DataValue has HtmlAttribute defined
		/// </summary>
		/// <param name="dvalue">DataValue that will be searched for HtmlViewAttribute attribute</param>
		/// <returns>True if DataValue has HtmlViewAttribute, false otherwise</returns>
		public static bool IsHtml(DataValue dvalue)
		{
			//Validating if the MemberInfo is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.InnerMember.GetCustomAttributes(typeof(HtmlAttribute), false);
			
			//Evaluating if exists the Attribute in the DataValue and returning the result
			if (attributes.Length == 0)
				return false;
			else
				return true;
		}
	}
}