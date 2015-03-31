using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Code.ORM.Filters
{
	/// <summary>
	/// Compares two DataValue's on the same DataObject
	/// </summary>
	public class DataValueCompareFilter: CompareFilter
	{

		#region Fields

		/// <summary>
		/// DataValue to compare with the DataValue defined in the 
		/// Field with the same name
		/// </summary>
		public DataValue DataValueToCompare;

		#endregion

		#region Contructors

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public DataValueCompareFilter() :
			this(null, null, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataValue">
		/// First DataValue used to comparison
		/// </param>
		public DataValueCompareFilter(DataValue dataValue) :
			this(dataValue, null, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataValue">
		/// First DataValue used to comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// Second DataValue used to comparison
		/// </param>
		public DataValueCompareFilter(DataValue dataValue, DataValue dataValueToCompare) : 
			this(dataValue, dataValueToCompare, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataValue">
		/// First DataValue used to comparison
		/// </param>
		/// <param name="dataValueToCompare">
		/// Second DataValue used to comparison
		/// </param>
		/// <param name="op">
		/// Operator of comparison
		/// </param>
		public DataValueCompareFilter(DataValue dataValue, DataValue dataValueToCompare, CompareOperator op)
			: base(dataValue, op)
		{
			this.DataValueToCompare = dataValueToCompare;
		}

		#endregion

		#region Filter Implementation

		/// <summary>
		/// Evaluates on the specified DataObject the members
		/// defined on DataValue and DataValueToCompare fields
		/// </summary>
		/// <param name="dobj">
		/// DataObject used to the comparison
		/// </param>
		/// <returns>
		/// true if the comparison is successfully fulfilled, 
		/// otherwise false
		/// </returns>
		public override bool Match(DataObject dobj)
		{
			//Validating if the DataObject specified is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Validating...
			return base.Match(dobj, (IComparable)dobj.GetValue(this.DataValueToCompare));
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the filter
		/// </summary>
		public override void ReadXml(XmlReader reader)
		{
			//Reading the Type of DataObject 
			base.ReadXml(reader);
			reader.MoveToAttribute("DataType");
			DataType dtype = DataType.Parse(reader.Value);

			//Reading the name of value to compare
			reader.MoveToAttribute("Name");
			DataValueToCompare = (DataValue) dtype[reader.Value];
			reader.MoveToElement();
			reader.Read();
		}

		/// <summary>
		/// Serialize the Filter
		/// </summary>
		public override void WriteXml(XmlWriter writer)
		{
			//Writing the filter to xml
			base.WriteXml(writer);
			writer.WriteStartElement("DataValueToCompare");
			writer.WriteAttributeString("DataType", DataValueToCompare.DeclaringDataType.UniqueId);
			writer.WriteAttributeString("Name", DataValueToCompare.Name);
			writer.WriteEndElement();
		}

		#endregion

	}
}
