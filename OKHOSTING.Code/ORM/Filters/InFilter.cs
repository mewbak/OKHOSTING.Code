using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Defines a filter in which the DataValue must be part
	/// of a values set
	/// </summary>
	/// <typeparam name="listItemsType">
	/// Type of the items in the values set
	/// </typeparam>
	public class InFilter<listItemsType>: DataValueFilter
	{

		#region Fields

		/// <summary>
		/// List of values of the filter
		/// </summary>
		public List<listItemsType> Values = new List<listItemsType>();

		/// <summary>
		/// Indicates if the filter comparison will be case sensitive
		/// when listItemsType = System.String
		/// </summary>
		public bool CaseSensitive;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		public InFilter() : this(null, null, false) { }

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used in the filter
		/// </param>
		public InFilter(DataValue dataValue) : this(dataValue, null, false) { }

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used in the filter
		/// </param>
		/// <param name="values">
		/// List of possible values of the filter
		/// </param>
		public InFilter(DataValue dataValue, List<listItemsType> values): this(dataValue, values, false)
		{
		}

		/// <summary>
		/// Constructs the In Filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used in the filter
		/// </param>
		/// <param name="values">
		/// List of possible values of the filter
		/// </param>
		/// <param name="caseSensitive">
		/// Indicates if the filter comparison will be case sensitive
		/// when listItemsType = System.String
		/// </param>
		public InFilter(DataValue dataValue, List<listItemsType> values, bool caseSensitive): base(dataValue)
		{
			this.Values = values;
			this.CaseSensitive = caseSensitive;
		}

		#endregion

		#region Filter Implementation

		/// <summary>
		/// Evaluates if the specified DataObject have his DataValue
		/// setted to any of the values of the list
		/// </summary>
		/// <param name="dobj">
		/// DataObject to evaluate
		/// </param>
		/// <returns>
		/// true if the DataObject fulfills the filter, otherwise false
		/// </returns>
		public override bool Match(DataObject dobj)
		{
			//Local vars
			bool Match = false;

			//Validating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}
			
			//Recovering the value of the DataValue member
			listItemsType val = (listItemsType)dobj.GetValue(this.DataValue);

			//Crossing the list of allowed values defined for the filter
			foreach (listItemsType obj in this.Values)
			{
				if (typeof(listItemsType).FullName.ToLower().Trim() == "system.string")
				{
					//Validating if the value is equal to the current value and if 
					//it's, establishing Match to true and breaking the cycle
					if ((CaseSensitive && val.ToString().Trim().Equals(obj.ToString().Trim())) ||
						(!CaseSensitive && val.ToString().ToLower().Trim().Equals(obj.ToString().ToLower().Trim())))
					{
						Match = true;
						break;
					}
				}
				else
				{
					//Validating if the value is equal to the current value and if 
					//it's, establishing Match to true and breaking the cycle
					if (val.Equals(obj))
					{
						Match = true;
						break;
					}
				}
			}

			//Returning value
			return Match;
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the filter
		/// </summary>
		public override void ReadXml(XmlReader reader)
		{
			//Reading the xml elements of the base class
			base.ReadXml(reader);

			//Recovering the CaseSensitive property 
			reader.Read();
			this.CaseSensitive = XmlConvert.ToBoolean(reader.Value);

			//Initializing the inner collection
			Values = new List<listItemsType>();

			//Crossing the reader content
			while (reader.Read())
			{
				//Validating if read must be interrupted
				if (reader.Name == "Values" && reader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				else if (reader.Name == "Value")
				{
					//Loading the type of the list
					reader.MoveToAttribute("Type");
					Type objectType = Type.GetType(reader.Value, true);

					//Reading the content of current value
					reader.Read();
					listItemsType obj = ((listItemsType)TypeConverter.ChangeType(reader.Value, objectType));

					//Adding current value to inner collection
					Values.Add(obj);

					//Moving the cursor to End element of current item
					reader.Read();
				}
			}
		}

		/// <summary>
		/// Serialize the filter
		/// </summary>
		public override void WriteXml(XmlWriter writer)
		{
			//Writing base class elements
			base.WriteXml(writer);

			//Writing CaseSensitive property 
			writer.WriteElementString("CaseSensitive", this.CaseSensitive.ToString().ToLower());

			//Writing the root element of values
			writer.WriteStartElement("Values");
			
			//Crossing the values collection and write them to xml
			foreach (listItemsType value in Values)
			{
				writer.WriteStartElement("Value");
				writer.WriteAttributeString("Type", value.GetType().AssemblyQualifiedName);
				writer.WriteString(value.ToString());
				writer.WriteEndElement();
			}
			
			//Closing the root element
			writer.WriteEndElement();
		}

		#endregion 

	}
}