using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Base class for value-to-compare filters
	/// </summary>
	public abstract class CompareFilter : DataValueFilter
	{

		#region Fields 

		/// <summary>
		/// Operator of the comparison
		/// </summary>
		public CompareOperator Operator;

		#endregion

		#region Constructors

		/// <summary>
		/// Construct the filter
		/// </summary>
		protected CompareFilter() : this(null, CompareOperator.Equal) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue to compare
		/// </param>
		protected CompareFilter(DataValue dataValue) : this(dataValue, CompareOperator.Equal) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue to compare
		/// </param>
		/// <param name="op">
		/// Operator to use in the comparison
		/// </param>
		protected CompareFilter(DataValue dataValue, CompareOperator op): base(dataValue)
		{
			this.Operator = op;
		}

		#endregion

		#region Filter Implementation

		/// <summary>
		/// Evaluates a DataObject and determines 
		/// if it matches with the filter
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be evaluated
		/// </param>
		/// <returns>
		/// true if dobj matches the filter, otherwise false 
		/// </returns>
		public abstract override bool Match(DataObject dobj);

		/// <summary>
		/// Evaluates a DataObject and determines 
		/// if it matches with the filter
		/// </summary>
		/// <param name="dobj">
		/// DataObject to be evaluated
		/// </param>
		/// <param name="valueToCompare">
		/// Value to compare to the specified DataValue 
		/// </param>
		/// <returns>
		/// true if dobj matches the filter, otherwise false
		/// </returns>
		protected bool Match(DataObject dobj, IComparable valueToCompare)
		{
			bool result = true;

			//Validating if the DataObject to compare is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Validating if the Value to compare is null
			if (valueToCompare == null)
			{
				throw new ArgumentNullException("valueToCompare");
			}
			
			//Getting the IComparable interface to compare from the value to compare
			IComparable toMatch = (IComparable)dobj.GetValue(this.DataValue);

			//current value is null
			if (toMatch == null)
			{
				//Determining the returnValue depending of the operator used
				switch (this.Operator)
				{
					case CompareOperator.Equal:
						if (valueToCompare != null) result = false;
						break;

					case CompareOperator.NotEqual:
						if (valueToCompare == null) result = false;
						break;

					case CompareOperator.GreaterThan:
					case CompareOperator.GreaterThanEqual:
					case CompareOperator.LessThan:
					case CompareOperator.LessThanEqual:
						result = false;
						break;
				}

				//exit
				return result;
			}

			//Validating if is an enum
			if ((this.DataValue.InnerMember.MemberType == MemberTypes.Field && ((FieldInfo)this.DataValue.InnerMember).FieldType.IsEnum) ||
				(this.DataValue.InnerMember.MemberType == MemberTypes.Property && ((PropertyInfo)this.DataValue.InnerMember).PropertyType.IsEnum))

			{
				toMatch = (IComparable)Convert.ChangeType(toMatch, ((Enum)toMatch).GetTypeCode());
			}
			
			//Comparing boot values
			int compareResult = toMatch.CompareTo(valueToCompare);

			//Determining the returnValue depending of the operator used
			switch (this.Operator)
			{
				case CompareOperator.Equal:
					if (compareResult != 0) result = false;
					break;

				case CompareOperator.NotEqual:
					if (compareResult == 0) result = false;
					break;

				case CompareOperator.GreaterThan:
					if (compareResult <= 0) result = false;
					break;

				case CompareOperator.GreaterThanEqual:
					if (compareResult < 0) result = false;
					break;

				case CompareOperator.LessThan:
					if (compareResult >= 0) result = false;
					break;

				case CompareOperator.LessThanEqual:
					if (compareResult > 0) result = false;
					break;
			}
			
			//If the code reach the next line, 
			return result;
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the filter
		/// </summary>
		public override void ReadXml(XmlReader reader)
		{
			//Loading the filter info
			base.ReadXml(reader);
			
			//Reading the information of the operator
			reader.Read();
			Operator = (CompareOperator)TypeConverter.ChangeType(reader.Value, typeof(CompareOperator));

			//Moving the cursor to next element
			reader.Read(); reader.Read();
		}

		/// <summary>
		/// Serialize the filter
		/// </summary>
		public override void WriteXml(XmlWriter writer)
		{
			//Writing the filter
			base.WriteXml(writer);
			writer.WriteElementString("CompareOperator", Operator.ToString());
		}

		#endregion
	}
}