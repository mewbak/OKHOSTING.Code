using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OKHOSTING.Code.ORM.Filters
{
	
	/// <summary>
	/// Base class for filters that contains a collection of filters and
	/// that compares them with a logical operator
	/// </summary>
	[Serializable]
	public class LogicalOperatorFilter: IFilter
	{ 

		#region Fields & Properties 

		/// <summary>
		/// Collection of conditions or filters that will be merged 
		/// with the and operator
		/// </summary>
		private FilterList innerFilters;

		/// <summary>
		/// Logical operator used in the filter
		/// </summary>
		public readonly LogicalOperator LogicalOperator;

		/// <summary>
		/// Collection of conditions or filters that will be merged 
		/// with the and operator
		/// </summary>
		public FilterList InnerFilters
		{ get { return innerFilters; } }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public LogicalOperatorFilter(LogicalOperator logicalOperator) : this(LogicalOperator.And, new FilterList()) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="logicalOperator">
		/// Logical operator used in the filter
		/// </param>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the and operator
		/// </param>
		public LogicalOperatorFilter(LogicalOperator logicalOperator, FilterList innerFilters)
		{
			this.LogicalOperator = logicalOperator;
			this.innerFilters = innerFilters;
		}

		#endregion

		#region Filter Implementation

		/// <summary>
		/// Implements the evaluation of the filter, retrieving the resultant boolean 
		/// value of marge all the filters defined in the InnerFilters collection with 
		/// the logical operator specified on LogicalOperator field
		/// </summary>
		/// <param name="dobj">
		/// DataObject in wich will be evaluated all the filters
		/// </param>
		/// <returns>
		/// Resultant boolean value of marge all the filters defined in the InnerFilters 
		/// collection with the logical operator specified on LogicalOperator field
		/// </returns>
		public bool Match(DataObject dobj)
		{
			//Validating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}
			
			//Crossing the filters collection
			foreach (IFilter f in this.InnerFilters)
			{
				//Validating the class of the filter
				if (LogicalOperator == LogicalOperator.And)
				{
					//If one of the filters is not successfully evaluated,
					//the function returns false...
					if (!f.Match(dobj)) return false;
				}
				else
				{
					//If only one of the filters is successfully evaluated,
					//the function returns true...
					if (f.Match(dobj)) return true;
				}
			}

			//Validating the class of the filter
			if (LogicalOperator == LogicalOperator.And)
			{
				//If the code execution reach the next line, then the DataObject
				//pass all the filters defined in the InnerFilters collection
				return true;
			}
			else
			{
				//If the code execution reach the next line, then the DataObject
				//fails all the filters defined in the InnerFilters collection
				return false;
			}
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the filter
		/// </summary>
		/// <param name="reader">
		/// Reader used to desearilize the filter
		/// </param>
		public void ReadXml(XmlReader reader)
		{ 
			//Creating the internal collection
			this.innerFilters = new FilterList();

			//Finding the InnerFilters parent
			reader.ReadToDescendant("InnerFilters");

			//Reading the collection contents
			this.innerFilters.ReadXml(reader);
			reader.Read();
		}

		/// <summary>
		/// Serialize the filter
		/// </summary>
		/// <param name="writer">
		/// Writer used to save the filter on Xml
		/// </param>
		public void WriteXml(XmlWriter writer)
		{
			//Writing the filter content to Xml
			writer.WriteStartElement("InnerFilters");
			this.innerFilters.WriteXml(writer);
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
