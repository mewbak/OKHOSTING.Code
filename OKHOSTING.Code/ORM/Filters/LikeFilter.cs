using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Represents a Filter with the Like operator
	/// </summary>
	public class LikeFilter : DataValueFilter
	{

		#region Fields

		/// <summary>
		/// Defines the pattern for the filter
		/// </summary>
		public string Pattern;

		/// <summary>
		/// Indicates if the filter comparison will be case sensitive
		/// </summary>
		public bool CaseSensitive;

		#endregion

		#region Constructors

		/// <summary>
		/// Construct the filter
		/// </summary>
		public LikeFilter() : this(null, string.Empty, false) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used to filter
		/// </param>
		public LikeFilter(DataValue dataValue) : this(dataValue, string.Empty, false) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used to filter
		/// </param>
		/// <param name="pattern">
		/// Defines the pattern for the filter
		/// </param>
		public LikeFilter(DataValue dataValue, string pattern) : this(dataValue, pattern, false) { }

		/// <summary>
		/// Construct the filter
		/// </summary>
		/// <param name="dataValue">
		/// DataValue used to filter
		/// </param>
		/// <param name="pattern">
		/// Defines the pattern for the filter
		/// </param>
		/// <param name="caseSensitive">
		/// Indicates if the filter comparison will be case sensitive
		/// </param>
		public LikeFilter(DataValue dataValue, string pattern, bool caseSensitive) : base(dataValue)
		{
			this.Pattern = pattern;
			this.CaseSensitive = caseSensitive;
		}

		#endregion

		#region Filter Implementation

		/// <summary>
		/// Evaluates if the specified DataObject fulfills the filter
		/// </summary>
		/// <param name="dobj">
		/// DataObject to evaluate
		/// </param>
		/// <returns>
		/// true if the DataObject fulfills the filter, otherwise null
		/// </returns>
		public override bool Match(DataObject dobj)
		{
			//Local Vars
			bool Match = false;

			//Validating if the dobj argument is null
			if (dobj == null)
			{
				throw new ArgumentNullException("dobj");
			}

			//Getting the value of the DataValue to evaluate
			string dataPropertyValue = (string)dobj.GetValue(this.DataValue);

			//Make an unformatted pattern, only containing the value to search
			string unformattedPattern = Pattern.Replace("%", "");

			//Making upper case the pattern and the value, for a case - insensitive comparation
			dataPropertyValue = (CaseSensitive? dataPropertyValue: dataPropertyValue.ToUpper());
			unformattedPattern = (CaseSensitive? unformattedPattern: unformattedPattern.ToUpper());
			
			//Validating the pattern an comparing with the value
			if(Pattern.StartsWith("%") && Pattern.EndsWith("%"))
			{
				Match = (dataPropertyValue.IndexOf(unformattedPattern) != -1);
			}
			else if(Pattern.StartsWith("%"))
			{
				Match = (dataPropertyValue.EndsWith(unformattedPattern));
			}
			else if(Pattern.EndsWith("%"))
			{
				Match = (dataPropertyValue.StartsWith(unformattedPattern));
			}
			else if(Pattern.IndexOf("%") != -1)
			{
				throw new 
					FormatException(
					"A wildcard ('%') is allowed only at the beginning and end of the Pattern string, " +
					"or at the end of a the Pattern string, or at the beginning of the Pattern string");
			}
			else
			{
				Match = (dataPropertyValue == unformattedPattern);
			}

			//Returning the value
			return Match;
		}

		#endregion

		#region Xml Serialization

		/// <summary>
		/// Deserialize the filter
		/// </summary>
		public override void ReadXml(XmlReader reader)
		{
			//Reading the xml 
			base.ReadXml(reader);
			
			//Recovering the pattern
			reader.Read();
			Pattern = reader.Value;
			
			//Moving the cursor to the end of LikeFilter element
			reader.Read(); reader.Read();
		}

		/// <summary>
		/// Serialize the filter
		/// </summary>
		public override void WriteXml(XmlWriter writer)
		{
			//Writing the xml
			base.WriteXml(writer);
			writer.WriteElementString("Pattern", Pattern);
		}

		#endregion 

	}
}