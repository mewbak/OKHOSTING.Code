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
	/// Allows to display DataTypes and their members in a categorized way, for better usability in user interfaces.
	/// DataTypes and DataMembers that have the samne category name, are grouped together in every page or form they are displayed
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false)]
	public class Category : Attribute
	{
		/// <summary>
		/// Name of the category
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Defines a category for a DatatType or DataMember, usefull for grouping in user interfaces
		/// </summary>
		/// <param name="name">Name of the category</param>
		public Category(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("name", "Argument can't be null or empty");

			Name = name;
		}

		/// <summary>
		/// Gets the category af a DataType
		/// </summary>
		/// <param name="dtype">DataType which category is needed</param>
		/// <returns>Name of the category the DataType belongs to, null if no category was defined</returns>
		public static string GetCategory(DataType dtype)
		{
			//Validating if the MemberInfo is null
			if (dtype == null) throw new ArgumentNullException("dtype");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dtype.InnerType.GetCustomAttributes(typeof(Category), false);

			//Evaluating if exists the DataMemberAttribute in the MemberInfo and returning the result
			if (attributes.Length == 0)
				return null;
			else
				return ((Category) attributes[0]).Name;
		}

		/// <summary>
		/// Gets the category af a DataMember
		/// </summary>
		/// <param name="dtype">DataMember which category is needed</param>
		/// <returns>Name of the category the DataMember belongs to, null if no category was defined</returns>
		public static string GetCategory(DataMember dmember)
		{
			//Validating if the MemberInfo is null
			if (dmember == null) throw new ArgumentNullException("dmember");

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dmember.InnerMember.GetCustomAttributes(typeof(Category), false);

			//Evaluating if exists the DataMemberAttribute in the MemberInfo and returning the result
			if (attributes.Length == 0)
				return null;
			else
				return ((Category) attributes[0]).Name;
		}
	}
}