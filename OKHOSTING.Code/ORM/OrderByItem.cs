using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// An item of an "Order By" clause for Select operations
	/// </summary>
	public class OrderByItem : IXmlSerializable
	{
		/// <summary>
		/// DataValue which determines the sorting
		/// </summary>
		public DataValue OrderBy;

		/// <summary>
		/// The direction of the sorting
		/// </summary>
		public SortDirection Direction;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public OrderByItem()
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public OrderByItem(DataValue orderBy): this(orderBy, SortDirection.Ascending)
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public OrderByItem(DataValue orderBy, SortDirection direction)
		{
			this.OrderBy = orderBy;
			this.Direction = direction;
		}

		#region IXmlSerializable Members

		/// <summary>
		/// Following online help recommendations, this method allways returns null
		/// </summary>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Deserialize the current OrderByItem
		/// </summary>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			//Direction
			reader.MoveToAttribute("Direction");
			Direction = (SortDirection) Enum.Parse(typeof(SortDirection), reader.Value);

			//Finding the DataField element
			reader.ReadToDescendant("OrderBy");

			//Loading the DataType attribute
			reader.MoveToAttribute("DataType");

			//Loading the DataType from the XML
			DataType dtype = DataType.Parse(reader.Value);

			//Loading the Name attribute
			reader.MoveToAttribute("Name");

			//Recovering the DataValue
			this.OrderBy = (DataValue) dtype[reader.Value];

			//Moving to the parent element of Name attribute...
			reader.MoveToElement();

			//Moving to next element 
			reader.Read();
		}

		/// <summary>
		/// Serialize the current OrderByItem
		/// </summary>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			//Direction
			writer.WriteAttributeString("Direction", Direction.ToString());
			
			//OrderBy
			writer.WriteStartElement("OrderBy");
			writer.WriteAttributeString("DataType", OrderBy.DeclaringDataType.UniqueId);
			writer.WriteAttributeString("Name", OrderBy.Name);
			writer.WriteEndElement();
		}

		#endregion
	}
}