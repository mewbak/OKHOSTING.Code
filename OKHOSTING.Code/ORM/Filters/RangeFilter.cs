using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace OKHOSTING.Code.ORM.Filters
{
	/// <summary>
	/// Implements a filter criterion based on a value range
	/// </summary>
	public class RangeFilter: DataValueFilter
	{

		#region Fields

		/// <summary>
		/// Minimum value of the allowed range
		/// </summary>
		public IComparable MinValue;
		
		/// <summary>
		/// Maximum value of the allowed range
		/// </summary>
		public IComparable MaxValue;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the filter 
		/// </summary>
		public RangeFilter() : this(null, null, null) { }

		/// <summary>
		/// Constructs the filter 
		/// </summary>
		/// <param name="dataValue">
		/// DataValue that must be on the specified range
		/// </param>
		public RangeFilter(DataValue dataValue) : this(dataValue, null, null) { }

		/// <summary>
		/// Constructs the filter 
		/// </summary>
		/// <param name="dataValue">
		/// DataValue that must be on the specified range
		/// </param>
		/// <param name="minValue">
		/// Minimum value of the allowed range
		/// </param>
		/// <param name="maxValue">
		/// Maximum value of the allowed range
		/// </param>
		public RangeFilter(DataValue dataValue, IComparable minValue, IComparable maxValue)
			: base(dataValue)
		{
			this.MinValue = minValue;
			this.MaxValue = maxValue;
		}

		#endregion

		#region Filter Implementation

		/// <summary>
		/// Implements the range filter
		/// </summary>
		/// <param name="dobj">
		/// DataObject used to validate
		/// </param>
		/// <returns>
		/// true if the filter is fulfilled, otherwise false
		/// </returns>
		public override bool Match(DataObject dobj)
		{
			//Validating if the dobj argument is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}
			
			//Validatnig throught ValueCompareFilter's
			return
				new ValueCompareFilter(this.DataValue, MinValue, CompareOperator.GreaterThanEqual).Match(dobj)
				&&
				new ValueCompareFilter(this.DataValue, MaxValue, CompareOperator.LessThanEqual).Match(dobj);
		}

		#endregion 

		#region Xml Serialization

		/// <summary>
		/// Deserialize the filter
		/// </summary>
		public override void ReadXml(XmlReader reader)
		{
			//Reading the base data
			base.ReadXml(reader);

			//Reading the type and value of MinValue field
			reader.MoveToAttribute("Type");
			Type type = Type.GetType(reader.Value, true);
			reader.Read();
			MinValue = (IComparable)TypeConverter.ChangeType(reader.Value, type);
			reader.Read();

			//Reading the type and value of MaxValue field
			reader.Read();
			reader.MoveToAttribute("Type");
			type = Type.GetType(reader.Value, true);
			reader.Read();
			MaxValue = (IComparable)TypeConverter.ChangeType(reader.Value, type);
			reader.Read();

			//Moving the cursor to the end element of filter
			reader.Read();
		}

		/// <summary>
		/// Serialize the filter
		/// </summary>
		public override void WriteXml(XmlWriter writer)
		{
			//Writing th base data
			base.WriteXml(writer);

			//Writing the type and value of MinValue field
			writer.WriteStartElement("MinValue");
			writer.WriteAttributeString("Type", MinValue.GetType().AssemblyQualifiedName);
			writer.WriteString(MinValue.ToString());
			writer.WriteEndElement();

			//Writing the type and value of MaxValue field
			writer.WriteStartElement("MaxValue");
			writer.WriteAttributeString("Type", MaxValue.GetType().AssemblyQualifiedName);
			writer.WriteString(MaxValue.ToString());
			writer.WriteEndElement();
		}

		#endregion 

	}
}