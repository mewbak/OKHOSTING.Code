using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Represents a Filter by Primary Key
	/// </summary>
	public class PrimaryKeyFilter: IFilter
	{ 

		#region Fields 

		/// <summary>
		/// Inner DataObject that will be compared in the Match method
		/// </summary>
		public DataObject DataObject;

		#endregion 

		#region Constructors

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public PrimaryKeyFilter() : this(null) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="dobj">
		/// Inner DataObject that will be compared in the Match method
		/// </param>
		public PrimaryKeyFilter(DataObject dobj)
		{
			this.DataObject = dobj;
		}

		#endregion

		#region Filter Implementation

		/// <summary>
		/// Returns true if the dobj argument have the same DataType and 
		/// the same primary key values that the inner DataObject of the 
		/// filter
		/// </summary>
		/// <param name="dobj">
		/// DataObject to compare with the Inner DataObject of the class
		/// </param>
		/// <returns>
		/// true if the dobj argument have the same DataType and the same 
		/// primary key values that the inner DataObject of the filter
		/// </returns>
		public bool Match(DataObject dobj)
		{
			//Validating if the argument dobj is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Comparing both DataObjects and returning boolean value
			return this.DataObject.Equals(dobj);
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the Filter
		/// </summary>
		public void ReadXml(XmlReader reader)
		{
			//Loading the DataType attribute
			reader.Read();
			reader.MoveToAttribute(typeof(DataType).Name);
			DataType dtype = DataType.Parse(reader.Value);
			reader.MoveToElement();

			//Initializing the type
			this.DataObject = DataObject.From(dtype);
			
			//Reading next elements
			while (reader.Read())
			{
				//Validating if the end was reached
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == this.GetType().Name)
				{
					break;
				}
				else if (reader.NodeType == XmlNodeType.Element)
				{
					//Loading the property value
					string propName = reader.Name;
					reader.Read();
					object currentValue = reader.Value;

					//Establishing the primary key property
					this.DataObject.SetValue(
						propName,
						TypeConverter.ChangeType(
						currentValue,
						this.DataObject[propName].DataValue.ValueType));
				}
			}
			////Deserializing the xml
			//XmlSerializer serializer = new XmlSerializer(dtype.InnerType);
			//this.DataObject = (DataObject)serializer.Deserialize(reader);			
		}
		 
		/// <summary>
		/// Serialize the filter
		/// </summary>
		public void WriteXml(XmlWriter writer)
		{
			//Escribiendo elemento raíz y su atributo DataType
			writer.WriteStartElement("DataObject");
			writer.WriteAttributeString("DataType", this.DataObject.DataType.UniqueId);

			//Loading primary key of DataObject
			DataValueInstanceAtomizedDictionary properties = new DataValueInstanceAtomizedDictionary();
			properties.AddRange(this.DataObject.PrimaryKey);

			//Crossing the DataValues of the primary key
			foreach (KeyValuePair<string, DataValueInstance> entry in properties)
			{
				//Getting the current DataValue
				DataValueInstance dvi = entry.Value;

				//Writing the element of the member
				writer.WriteStartElement(dvi.DataValue.Name);

				//Writing the value of the member (only if not null)
				if (dvi.DataValue.GetValue(this.DataObject) != null)
				{
					writer.WriteString(dvi.DataValue.GetValue(this.DataObject).ToString());
				}

				//Writing the close element
				writer.WriteEndElement();
			}
			
			//Writing the close element
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