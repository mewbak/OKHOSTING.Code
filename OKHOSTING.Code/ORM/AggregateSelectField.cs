using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Defines a field on a select query that will be processed 
	/// with an aggregate function on a ResultSet grouped
	/// </summary>
	public struct AggregateSelectField : IXmlSerializable
	{

		#region Fields

		/// <summary>
		/// Aggregate function to use for calculate the column
		/// </summary>
		public AggregateFunctions AggregateFunction;

		/// <summary>
		/// Speficy if the DISTINCT modifier must be applied
		/// </summary>
		public bool Distinct;

		/// <summary>
		/// Alias name of the resulting field
		/// </summary>
		public string Alias;

		/// <summary>
		/// DataValue for build the field definition
		/// </summary>
		public DataValue DataValue;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataValue for build the field definition
		/// </param>
		public AggregateSelectField(DataValue dataValue) : 
			this(dataValue, AggregateFunctions.None, false, string.Empty) { }

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataValue for build the field definition
		/// </param>
		/// <param name="alias">
		/// Alias name of the resulting field
		/// </param>
		public AggregateSelectField(DataValue dataValue, string alias) : 
			this(dataValue, AggregateFunctions.None, false, alias) { }

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataValue for build the field definition
		/// </param>
		/// <param name="aggregateFunction">
		/// Aggregate function to use for calculate the column
		/// </param>
		public AggregateSelectField(DataValue dataValue, AggregateFunctions aggregateFunction) : 
			this(dataValue, aggregateFunction, false, string.Empty) { }

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataValue for build the field definition
		/// </param>
		/// <param name="aggregateFunction">
		/// Aggregate function to use for calculate the column
		/// </param>
		/// <param name="alias">
		/// Alias name of the resulting field
		/// </param>
		public AggregateSelectField(DataValue dataValue, AggregateFunctions aggregateFunction, string alias) :
			this(dataValue, aggregateFunction, false, alias) { }

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataValue for build the field definition
		/// </param>
		/// <param name="aggregateFunction">
		/// Aggregate function to use for calculate the column
		/// </param>
		/// <param name="distinct">
		/// Speficy if the DISTINCT modifier must be applied
		/// </param>
		public AggregateSelectField(
			DataValue dataValue,
			AggregateFunctions aggregateFunction,
			bool distinct)
			: this(dataValue, aggregateFunction, distinct, string.Empty) { }

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataValue for build the field definition
		/// </param>
		/// <param name="aggregateFunction">
		/// Aggregate function to use for calculate the column
		/// </param>
		/// <param name="distinct">
		/// Speficy if the DISTINCT modifier must be applied
		/// </param>
		/// <param name="alias">
		/// Alias name of the resulting field
		/// </param>
		public AggregateSelectField(
			DataValue dataValue,
			AggregateFunctions aggregateFunction,
			bool distinct, 
			string alias)
		{
			this.DataValue = dataValue;
			this.AggregateFunction = aggregateFunction;
			this.Distinct = distinct;
			this.Alias = alias;
		}

		#endregion

		#region IXmlSerializable Members

		/// <summary>
		/// Following online help recommendations, this method allways returns null
		/// </summary>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Serialize the current AggregateSelectField
		/// </summary>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("AggregateFunction", AggregateFunction.ToString());
			writer.WriteAttributeString("Distinct", Distinct.ToString());
			writer.WriteAttributeString("Alias", Alias);

			writer.WriteStartElement("DataValue");
			((IXmlSerializable)DataValue).WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Deserialize the current AggregateSelectField
		/// </summary>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToAttribute("AggregateFunction");
			AggregateFunction = (AggregateFunctions)Enum.Parse(typeof(AggregateFunctions), reader.Value, true);

			reader.MoveToAttribute("Distinct");
			Distinct = bool.Parse(reader.Value);

			reader.MoveToAttribute("Alias");
			Alias = reader.Value;

			//read the DataValue
			reader.MoveToElement();
			reader.ReadToDescendant("DataValue");

			//Loading the DataType attribute
			reader.MoveToAttribute("DataType");

			//Loading the DataType from the XML
			DataType dtype = DataType.Parse(reader.Value);

			//Loading the Name attribute
			reader.MoveToAttribute("Name");

			//Recovering the DataValue
			this.DataValue = (DataValue)dtype[reader.Value];

			//Moving to the parent element of Name attribute...
			reader.MoveToElement();

			//Moving to next element 
			reader.Read();
		}

		#endregion

	}
}