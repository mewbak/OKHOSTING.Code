using System;
using System.Reflection;
using System.Xml.Serialization;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Represents an element of a DataType, such as DataField, DataProperty, DataMethod or DataEvent.
	/// This is the base class for all datatypes members, including DataFields, ListDataFields and DataMethods
	/// </summary>
	[Serializable]
	public abstract class DataMember : Member, IXmlSerializable, IStringSerializable
	{
		#region IXmlSerializable Members

		/// <summary>
		/// Following online help recommendations, this method allways returns null
		/// </summary>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Deserialize the current DataMember
		/// </summary>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			DataType dtype;

			reader.MoveToAttribute("DataType");
			dtype = DataType.Parse(reader.Value);
			reader.MoveToAttribute("Name");
			innerMember = dtype[reader.Value].InnerMember;
		}

		/// <summary>
		/// Serialize the current DataMember
		/// </summary>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("DataType", DeclaringDataType.UniqueId);
			writer.WriteAttributeString("Name", Name);
		}

		#endregion

		#region IStringSerializable Members

		/// <summary>
		/// Serialize the DataMember to a string
		/// </summary>
		string IStringSerializable.SerializeToString()
		{
			return "DeclaringDataType=" + ((IStringSerializable) DeclaringDataType).SerializeToString() + "&Name=" + this.Name;
		}

		/// <summary>
		/// Deserializes the DataMember from a string containing the DataMember's assembly qualified name and DataMember's name
		/// </summary>
		/// <param name="s">A string containing the DataMember's assembly qualified name and DataMember's name</param>
		void IStringSerializable.DeserializeFromString(string s)
		{
			DataType dtype;
			DataMember dmember;

			//Validating if the sting is null
			if (s == null) throw new ArgumentNullException("s");

			//deserialize datatype
			dtype = DataType.Parse(TypeConverter.GetValueFromQueryString(s, "DataType"));

			//obtain datamember by name
			dmember = dtype[TypeConverter.GetValueFromQueryString(s, "Name")];
			this.innerMember = dmember.InnerMember;
		}

		#endregion
	}
}