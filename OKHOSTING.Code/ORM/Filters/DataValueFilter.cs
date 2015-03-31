using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Represents a filter based on a value of a DataObject (Field or Property)
	/// </summary>
	public abstract class DataValueFilter : IFilter
	{
		 
		#region Fields

		/// <summary>
		/// DataValue used to filter
		/// </summary>
		public DataValue DataValue;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates the filter
		/// </summary>
		protected DataValueFilter() : this(null) { }

		/// <summary>
		/// Creates the filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used to filter
		/// </param>
		protected DataValueFilter(DataValue dataValue)
		{
			//Establishing the DataValue to filter
			this.DataValue= dataValue;
		}

		#endregion

		#region Abstract methods

		/// <summary>
		/// Evaluates the specified DataObject under the filter criteria
		/// and retuns a value that indicates if the DataObject fulfills
		/// the filter
		/// </summary>
		/// <param name="dataObject">
		/// DataObject to evaluate
		/// </param>
		/// <returns>
		/// true if the DataObject fulfills the filter, otherwise false
		/// </returns>
		public abstract bool Match(DataObject dataObject);

		#endregion
		 
		#region Xml Serialization

		/// <summary>
		/// Deserialize the filter
		/// </summary>
		public virtual void ReadXml(XmlReader reader)
		{
			//Finding the DataField element
			reader.ReadToDescendant("DataValue");

			//Loading the DataType attribute
			reader.MoveToAttribute("DataType");

			//Loading the DataType from the XML
			DataType dtype = DataType.Parse(reader.Value);

			//Loading the Name attribute
			reader.MoveToAttribute("Name");

			//Recovering the DataValue
			this.DataValue = (DataValue) dtype[reader.Value];

			//Moving to the parent element of Name attribute...
			reader.MoveToElement();

			//Moving to next element 
			reader.Read();
		}

		/// <summary>
		/// Serialize the filter
		/// </summary>
		public virtual void WriteXml(XmlWriter writer)
		{
			//Writing the Filter content
			writer.WriteStartElement("DataValue");
			writer.WriteAttributeString("DataType", DataValue.DeclaringDataType.UniqueId);
			writer.WriteAttributeString("Name", DataValue.Name);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Following online help recommendations, this method allways return null
		/// </summary>
		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		#endregion
	}
}