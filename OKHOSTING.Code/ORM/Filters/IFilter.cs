using System;
using System.Xml.Serialization;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Represents a Filter to apply on a DataObject
	/// </summary>
	public interface IFilter : IXmlSerializable
	{

		/// <summary>
		/// Indicates if a DataObject matches the filter or not
		/// </summary>
		/// <param name="dataObject">
		/// The DataObject to be evaluated
		/// </param>
		/// <returns>
		/// A System.Boolean that indicates if the DataObject matches the filter
		/// </returns>
		bool Match(DataObject dataObject);

	}
}