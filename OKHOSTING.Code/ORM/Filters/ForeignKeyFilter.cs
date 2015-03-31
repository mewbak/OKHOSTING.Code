using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml;

namespace OKHOSTING.Code.ORM.Filters
{
	/// <summary>
	/// Implements a filter based on the comparison
	/// of a foreign key
	/// </summary>
	public class ForeignKeyFilter: ValueCompareFilter
	{
		/// <summary>
		/// Foreign Key DataObject for comparison
		/// </summary>
		public new DataObject ValueToCompare
		{
			get
			{
				return (DataObject) base.ValueToCompare;
			}
			set
			{
				base.ValueToCompare = value;
			}
		}

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public ForeignKeyFilter() : this(null, null) 
		{
		}

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used to link the local with the foreign DataObject
		/// </param>
		/// <param name="foreignDataObject">
		/// Foreign DataObject
		/// </param>
		public ForeignKeyFilter(DataValue dataValue, DataObject foreignDataObject): base(dataValue, foreignDataObject)
		{
			this.Operator = CompareOperator.Equal;
		}

		/// <summary>
		/// Compares the DataValue on the specified DataObject with the 
		/// ValueToCompare field
		/// </summary>
		/// <param name="dobj"></param>
		/// <returns>
		/// true if the comparison is fulfilled, otherwise false
		/// </returns>
		public override bool Match(DataObject dobj)
		{
			//Evaluating if the DataObject is null
			if (dobj == null) throw new ArgumentNullException("dobj");

			//Evaluate
			return base.Match(dobj, this.ValueToCompare);
		}
	}
}