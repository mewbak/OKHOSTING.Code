using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Represents a custom filter
	/// </summary>
	public class CustomFilter: IFilter
	{

		#region Constructors 

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public CustomFilter() : this(null) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="filtering">
		/// Event handler of custom filter
		/// </param>
		public CustomFilter(FilterEventHandler filtering)
		{
			this.Filtering = filtering;
		}

		#endregion

		#region Events

		/// <summary>
		/// Event that is raised when the custom filter must be applied
		/// </summary>
		public event FilterEventHandler Filtering;

		#endregion

		#region Filter Implementation

		/// <summary>
		/// Evaluates if the specified DataObject matches the filter or not
		/// </summary>
		/// <param name="dobj">
		/// DataObject to evaluate
		/// </param>
		/// <returns>
		/// true if the DataObject pass successfully the filter, otherwise false
		/// </returns>
		public bool Match(DataObject dobj)
		{
			//Local Vars 
			bool Match = false;

			//Validating if the DataObject is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Validating if exists subscriptors to validation event 
			if (this.Filtering != null)
			{
				//Creating the information argument
				FilterEventArgs e = new FilterEventArgs(dobj);

				//Throwing filter event...
				this.Filtering(this, e);

				//Getting value that indicates if the filter was succesfully passed
				Match = e.Match;
			}
			else
			{
				//Non exists subscriptors to validation event; Throwing exception
				throw new NullReferenceException(this.GetType().FullName + ".Filtering event must contain at least one event handler");
			}

			//Returning the corresponding value
			return Match;
		}

		#endregion

		#region Xml Serialization
		  
		/// <summary>
		/// Deserialize the filter
		/// </summary>
		public void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException("CustomFilter can't be serialized");
		} 

		/// <summary>
		/// Serialize the filter
		/// </summary>
		public void WriteXml(XmlWriter writer)
		{
			throw new NotImplementedException("CustomFilter can't be serialized");
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