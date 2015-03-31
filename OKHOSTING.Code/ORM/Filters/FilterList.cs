using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Tools;
using OKHOSTING.Tools.Extensions;

namespace OKHOSTING.Code.ORM.Filters
{
	/// <summary>
	/// Collection of filters
	/// </summary>
	public class FilterList: List<IFilter>, IXmlSerializable
	{

		#region Constructors

		/// <summary>
		/// Contruct the collection
		/// </summary>
		public FilterList():this(new IFilter[0]){}

		/// <summary>
		/// Constructs the collection
		/// </summary>
		/// <param name="collection">
		/// Array of filters to append to the collection
		/// </param>
		public FilterList(params IFilter[] collection)
		{
			this.AddRange(collection);
		}

		/// <summary>
		/// Contruct the collection using a DataObject as a template for creating filters
		/// </summary>
		public static FilterList FromDataObject(DataObject template)
		{
			FilterList filters;

			filters = new FilterList();

			foreach (DataValueInstance dvi in template.AllValues)
			{
				if (NullValues.IsNull(dvi.Value)) continue;

				if (dvi.Value is string)
					filters.Add(new LikeFilter(dvi.DataValue, "%" + dvi.Value + "%"));
				else if (dvi.DataValue.IsForeignKey)
					filters.Add(new ForeignKeyFilter(dvi.DataValue, (DataObject)dvi.Value));
				else
					filters.Add(new ValueCompareFilter(dvi.DataValue, (IComparable)dvi.Value, CompareOperator.Equal));
			}

			return filters;
		}

		#endregion

		#region Filter implementation

		/// <summary>
		/// Evaluates if the specified DataObject matches with
		/// all the filters defined on the collection
		/// </summary>
		/// <param name="dobj">
		/// DataObject to evaluate
		/// </param>
		/// <returns>
		/// true if the specified DataObject matches with
		/// all the filters defined on the collection, 
		/// otherwise false
		/// </returns>
		public bool Match(DataObject dobj)
		{
			//Validating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Crossing the filters defined
			foreach (IFilter f in this)
			{
				//Validating if the DataObject pass the current filter 
				//and if not, leaving the procedure and returning false
				if (!f.Match(dobj)) return false;
			}

			//If the code execution reach the next line, the DataObject
			//matches with all the filters in the collection and the
			//function retrieve true
			return true;
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the Filter collection from Xml
		/// </summary>
		/// <param name="reader">
		/// Reader used to recovery the collection info
		/// </param>
		public void ReadXml(XmlReader reader)
		{
			//Getting the root node name
			string nodeName = reader.Name;

			//Crossing the reader contents
			while (reader.Read())
			{
				//Validating if must break the cycle (if the collection close item was reached)
				if (reader.NodeType == XmlNodeType.EndElement && reader.Name == nodeName)
				{
					reader.Read();
					break;
				}

				//Validating if this iteration must be omitted
				else if (reader.NodeType != XmlNodeType.Element || reader.Name != "IFilter" || reader.EOF)
				{
					continue;
				}
				else
				{
					//Finding attribute type
					reader.MoveToAttribute("Type");

					//Creating Type for the current filter
					Type filterType = Type.GetType(reader.Value, true);

					//Creating instance of the filter
					IFilter filter = (IFilter) filterType.CreateInstance();

					//Moving to the current parent element
					reader.MoveToElement();

					//Loading on the filter the element
					filter.ReadXml(reader);

					//Adding the filter to the collection
					this.Add(filter);
				}
			}
		}

		/// <summary>
		/// Serialize the collection to Xml
		/// </summary>
		/// <param name="writer">
		/// Writer used to save the collection on disk
		/// </param>
		public void WriteXml(XmlWriter writer)
		{
			//Crossing the filters on the collection
			foreach (IFilter filter in this)
			{
				//Writing the info of the current filter to Xml
				writer.WriteStartElement("IFilter");
				writer.WriteAttributeString("Type", filter.GetType().AssemblyQualifiedName);
				filter.WriteXml(writer);
				writer.WriteEndElement();
			}
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