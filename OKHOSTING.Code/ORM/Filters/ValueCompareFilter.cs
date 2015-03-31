using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Code.ORM.Filters
{
	/// <summary>
	/// Compare a DataValue with a value
	/// </summary>
	public class ValueCompareFilter: CompareFilter
	{

		#region Fields

		IComparable valueToCompare;

		/// <summary>
		/// Value for comparison
		/// </summary>
		public IComparable ValueToCompare
		{
			get
			{
				return valueToCompare;
			}
			set
			{
				valueToCompare = value;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public ValueCompareFilter() : this(null, null, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataValue">
		///  DataValue for the comparison
		/// </param>
		public ValueCompareFilter(DataValue dataValue) : this(dataValue, null, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataValue">
		///  DataValue for the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value for comparison
		/// </param>
		public ValueCompareFilter(DataValue dataValue, IComparable valueToCompare) : this(dataValue, valueToCompare, CompareOperator.Equal) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dataValue">
		///  DataValue for the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value for comparison
		/// </param>
		/// <param name="op">
		/// Operator for comparison
		/// </param>
		public ValueCompareFilter(DataValue dataValue, IComparable valueToCompare, CompareOperator op) : base(dataValue, op)
		{
			this.ValueToCompare = valueToCompare;
		}

		#endregion

		#region Filter Implementation

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
			//Eañuating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}
	
			//Evaluating...
			return base.Match(dobj, this.ValueToCompare);
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the filter 
		/// </summary>
		public override void ReadXml(XmlReader reader)
		{
			//Reading the type of the value to compare
			base.ReadXml(reader);
			reader.MoveToAttribute("Type");
			Type type = Type.GetType(reader.Value, true);

			//Reading the value to compare
			reader.MoveToAttribute("Value");
			ValueToCompare = (IComparable)TypeConverter.ChangeType(reader.Value, type);
			
			//Posicionando el cursor en el elemento de finalización del filtro
			reader.Read();
		}

		/// <summary>
		/// Serialize the filter
		/// </summary>
		public override void WriteXml(XmlWriter writer)
		{
			//Writing the filter
			base.WriteXml(writer);
			writer.WriteStartElement("ValueToCompare");
			writer.WriteAttributeString("Type", ValueToCompare.GetType().AssemblyQualifiedName);
			writer.WriteAttributeString("Value", TypeConverter.SerializeToString(ValueToCompare));
			writer.WriteEndElement();
		}

		#endregion
	}
}