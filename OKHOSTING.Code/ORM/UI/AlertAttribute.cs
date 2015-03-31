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
	/// Sets an alert if a field has certain value.
	/// Allows UI to show a DataValue in "alert" (red or yellow for example)
	/// if the condition is evaluated as true
	/// </summary>
	/// <remarks>Only works for IComparable DataValues (numeric, dates, etc)</remarks>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class AlertAttribute : Attribute
	{
		/// <summary>
		/// Operator that will be used  to determine if a value is in "Alert" state
		/// </summary>
		public CompareOperator Operator;

		/// <summary>
		/// Value that will be compared to determine if a value is in "Alert" state
		/// </summary>
		public IComparable ValueToCompare;

		/// <summary>
		/// Indicates if a DataValueInstance is in "Alert" state
		/// </summary>
		/// <param name="dvalue">DataValue that will be searched for ReadOnlyAttribute attribute</param>
		/// <returns>True if DataValue is read only, false otherwise</returns>
		public static bool IsAlert(DataValueInstance dvalue)
		{
			//Validating if the dvalue is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");
			
			//if value is null, return false
			if (NullValues.IsNull(dvalue.Value)) return false;

			//if value is not IComparable, return false
			if (!(dvalue.Value is IComparable)) return false;

			//Recovering the attributes of type DataMemberAttribute declared in the MemberInfo
			object[] attributes = dvalue.DataValue.InnerMember.GetCustomAttributes(typeof(AlertAttribute), false);

			//Evaluating if exists the Attribute in the DataValue and returning the result
			if (attributes.Length == 0) return false;

			//get alert attribute
			AlertAttribute alert = ((AlertAttribute)attributes[0]);

			//Comparing boot values
			int compareResult = ((IComparable)dvalue.Value).CompareTo(alert.ValueToCompare);

			//Determining the returnValue depending of the operator used
			bool returnValue = false;

			switch (alert.Operator)
			{
				case CompareOperator.Equal:
					if (compareResult != 0) returnValue = true;
					break;

				case CompareOperator.NotEqual:
					if (compareResult == 0) returnValue = true;
					break;

				case CompareOperator.GreaterThan:
					if (compareResult <= 0) returnValue = true;
					break;

				case CompareOperator.GreaterThanEqual:
					if (compareResult < 0) returnValue = true;
					break;

				case CompareOperator.LessThan:
					if (compareResult >= 0) returnValue = true;
					break;

				case CompareOperator.LessThanEqual:
					if (compareResult > 0) returnValue = true;
					break;
			}

			//return result
			return returnValue;
		}

		#region Constructors

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, short valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, int valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, long valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, ushort valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, uint valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, ulong valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, byte valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, sbyte valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, float valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, double valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, decimal valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, DateTime valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, string valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, bool valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Creates a new Alert attribute
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, char valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Constructs the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public AlertAttribute(CompareOperator op, IComparable valueToCompare)
		{
			//Validating if valueToCompare argument is null
			if (valueToCompare == null) throw new ArgumentNullException("valueToCompare");

			//Initializing validator
			this.Operator = op;
			this.ValueToCompare = valueToCompare;
		}

		#endregion 
	}
}