using System;
using System.Reflection;
using System.Xml.Serialization;
using OKHOSTING.Tools;
using OKHOSTING.Tools.Extensions;
using System.Collections;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Defines methods for converting objects from one Type to another,
	/// as well as serialization and deserialization methods
	/// </summary>
	public static class TypeConverter
	{
		#region From object to object

		/// <summary>
		/// This method try to convert the specified source value on the 
		/// indicated Type. This class implements converting methods for internal use, 
		/// use it to convert values from database to objetc instance and viceversa,
		/// as well as creating URLs for datatypes, dataobjects or datamemebrers,
		/// or string representations of objects
		/// </summary>
		/// <param name="sourceValue">
		/// Value that you desire to convert
		/// </param>
		/// <param name="destinyType">
		/// Destiny type
		/// </param>
		/// <returns>
		/// The reference to the converted object
		/// </returns>
		public static object ChangeType(object value, Type destinyType)
		{
			//null values
			if (value == null) return null;

			//no need for conversion
			if (destinyType.IsAssignableFrom(value.GetType())) return value;

			//from string to object
			if (value is string) return ToObject((string)value, destinyType);

			//from object to TimeSpan
			if (destinyType.Equals(typeof(TimeSpan))) return ToTimeSpan(value);

			//from object to string
			if (destinyType.Equals(typeof(string))) return ToString(value);

			//from object to enumeration
			if (destinyType.IsEnum) return ToEnum(value, destinyType);

			//Trying to convert throught IConvertible interface
			return Convert.ChangeType(value, destinyType);
		}

		/// <summary>
		/// Converts a Enum object representantion into an actual Enum instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to Enum
		/// </param>
		/// <returns>
		/// A Enum deserialized from the object
		/// </returns>
		public static Enum ToEnum(object value, Type enumType)
		{
			//validate arguments
			if (value == null) return null;
			if (enumType == null) throw new ArgumentNullException("enumType");

			//Parsing a string...
			if (value is string) return ToEnum((string)value, enumType);
			
			//parsing an object
			return (Enum)System.Enum.ToObject(enumType, value);
		}

		/// <summary>
		/// Converts a string value containing a timespan into a TimeSpan object
		/// </summary>
		/// <param name="value">
		/// String value to be converted to TimeSpan
		/// </param>
		/// <returns>
		/// A TimeSpan obtained from value
		/// </returns>
		public static TimeSpan ToTimeSpan(string value)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return NullValues.TimeSpan;

			//converto to TimeSpan from ticks
			return TimeSpan.Parse(value);
		}

		/// <summary>
		/// Converts a long value containing Ticks into a TimeSpan
		/// </summary>
		/// <param name="value">
		/// Value to be converted to TimeSpan
		/// </param>
		/// <returns>
		/// A TimeSpan obtained from the number of ticks
		/// </returns>
		public static TimeSpan ToTimeSpan(object ticks)
		{
			//validate arguments
			if (NullValues.IsNull(ticks)) return NullValues.TimeSpan;

			//converto to TimeSpan from ticks
			return ToTimeSpan((long)Convert.ChangeType(ticks, typeof(long)));
		}

		/// <summary>
		/// Converts a long value containing Ticks into a TimeSpan
		/// </summary>
		/// <param name="value">
		/// Value to be converted to TimeSpan
		/// </param>
		/// <returns>
		/// A TimeSpan obtained from the number of ticks
		/// </returns>
		public static TimeSpan ToTimeSpan(long ticks)
		{
			//validate arguments
			if (ticks <= 0) return NullValues.TimeSpan;

			//converto to TimeSpan from ticks
			return TimeSpan.FromTicks(ticks);
		}

		#endregion
		
		#region From object to string, string serialization

		/// <summary>
		/// Converts a value to it's string representantion
		/// </summary>
		/// <param name="value">
		/// Value to be serialized as string
		/// </param>
		/// <returns>
		/// A string representation of value
		/// </returns>
		public static string ToString(object value)
		{
			if (NullValues.IsNull(value)) return null;
			return value.ToString();
		}

		/// <summary>
		/// Converts a value to it's string representantion, so it can be deserialized back in the future
		/// </summary>
		/// <param name="value">
		/// Value to be serialized as string
		/// </param>
		/// <returns>
		/// A string representation of value
		/// </returns>
		public static string SerializeToString(object value)
		{
			if (NullValues.IsNull(value)) 
				return null;

			else if (value is string)
				return (string) value;

			else if (value is System.Enum)
				return SerializeToString((System.Enum)value);

			else if (value is IStringSerializable)
				return SerializeToString((IStringSerializable)value);

			else if (value is IXmlSerializable)
				return SerializeToString((IXmlSerializable)value);

			else if (value is IEnumerable)
				return SerializeToString((IEnumerable)value);

			else if (value is DateTime)
				return SerializeToString((DateTime)value);

			return value.ToString();
		}

		/// <summary>
		/// Converts a value to it's database-specific string representantion, so it can be included in a SQL script
		/// </summary>
		/// <param name="value">
		/// Value to be converted to string
		/// </param>
		/// <returns>
		/// A database-specific string representation of value
		/// </returns>
		public static string SerializeToString(Enum value)
		{
			//null values
			if (value == null) return null;
			
			return Convert.ChangeType(value, value.GetTypeCode()).ToString();
		}

		/// <summary>
		/// Converts a value to it's string representantion
		/// </summary>
		/// <param name="value">
		/// Value to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of value
		/// </returns>
		public static string SerializeToString(IStringSerializable value)
		{
			//null values
			if (value == null) return null;
			
			return value.SerializeToString();
		}

		/// <summary>
		/// Converts a value to it's string representantion
		/// </summary>
		/// <param name="value">
		/// Value to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of value
		/// </returns>
		public static string SerializeToString(IXmlSerializable value)
		{
			//null values
			if (value == null) return null;

			//Creating Serializer of corresponding type and Serializing
			XmlSerializer serializer = new XmlSerializer(value.GetType());
			StringWriter writer = new StringWriter();
			serializer.Serialize(writer, value);

			//Return xml code
			return writer.ToString();
		}

		/// <summary>
		/// Converts a value to it's string representantion, so it can be included in a SQL script
		/// </summary>
		/// <param name="value">
		/// Value to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of value
		/// </returns>
		public static string SerializeToString(IEnumerable value)
		{
			//null values
			if (value == null) return null;

			//Creating Serializer of corresponding type and Serializing
			XmlSerializer serializer = new XmlSerializer(value.GetType());
			StringWriter writer = new StringWriter();
			serializer.Serialize(writer, value);

			//Return xml code
			return writer.ToString();
		}

		/// <summary>
		/// Returns the string representation of the specified NameValueCollection
		/// </summary>
		/// <param name="values">List that will be parsed as a string</param>
		/// <returns>
		/// The properties stored on values on the format
		/// Property1=ValueOfProperty1&Property2=ValueOfProperty2...
		/// </returns>
		public static string SerializeToString(NameValueCollection value)
		{
			//Local Vars
			string nameValues = string.Empty;

			//Crossing the name / value pairs 
			foreach (string name in value.AllKeys)
			{
				nameValues += name + "=" + value[name] + "&";
			}

			//Remove last &
			nameValues = nameValues.TrimEnd('&');

			//Returning the string to the caller
			return nameValues;
		}

		/// <summary>
		/// Converts a value to it's string representantion
		/// </summary>
		/// <param name="value">
		/// Assembly to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of value
		/// </returns>
		public static string SerializeToString(Assembly value)
		{
			//null values
			if (value == null) return null;

			return value.FullName;
		}

		/// <summary>
		/// Converts a value to it's string representantion
		/// </summary>
		/// <param name="value">
		/// Assembly to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of value
		/// </returns>
		public static string SerializeToString(DateTime value)
		{
			//null values
			if (value == null) return null;

			return value.ToString("yyyy/MM/dd HH:mm:ss");
		}

		#endregion

		#region From string to object

		/// <summary>
		/// Converts a value to it's string representantion, so it can be deserialized back in the future
		/// </summary>
		/// <param name="value">
		/// Value to be serialized as string
		/// </param>
		/// <returns>
		/// A string representation of value
		/// </returns>
		public static object ToObject(string value, Type destinyType)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;
			if (destinyType == null) throw new ArgumentNullException("destinyType");

			//no need for conversion
			if (destinyType.IsAssignableFrom(value.GetType())) return value;

			//TimeSpan
			if (destinyType.Equals(typeof(TimeSpan))) return ToTimeSpan(value);

			//DateTime
			if (destinyType.Equals(typeof(DateTime))) return ToDateTime(value); 
			
			//enum
			if (destinyType.IsEnum) return ToEnum(value, destinyType);

			//dataobject
			if (DataType.IsDataObject(destinyType)) return ToDataObject(value);

			//IStringSerializable
			if (destinyType.GetInterface(typeof(IStringSerializable).FullName) != null) return ToIStringSerializable(value, destinyType);

			//IXmlSerializable
			if (destinyType.GetInterface(typeof(IXmlSerializable).FullName) != null) return ToIXmlSerializable(value, destinyType);

			//IEnumerable
			if (destinyType.GetInterface(typeof(System.Collections.IEnumerable).FullName) != null) return ToIEnumerable(value, destinyType);

			//generic
			return Convert.ChangeType(value, destinyType);
		}

		/// <summary>
		/// Converts a Enum string representantion into an actual Enum instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to Enum
		/// </param>
		/// <returns>
		/// A Enum object deserialized from the string
		/// </returns>
		public static Enum ToEnum(string value, Type enumType)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;
			if (enumType == null) throw new ArgumentNullException("enumType");

			//Parsing the string...
			return (Enum) System.Enum.Parse(enumType, value);
		}

		/// <summary>
		/// Converts a IStringSerializable string representantion into an actual IStringSerializable instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to IStringSerializable
		/// </param>
		/// <returns>
		/// A IStringSerializable object deserialized from the string
		/// </returns>
		public static IStringSerializable ToIStringSerializable(string value, Type destinyType)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;
			if (destinyType == null) throw new ArgumentNullException("destinyType");

			//Deserializing the source value to destiny type
			IStringSerializable result = (IStringSerializable) destinyType.CreateInstance();
			((IStringSerializable)result).DeserializeFromString(value);

			return result;
		}

		/// <summary>
		/// Converts a IXmlSerializable string representantion into an actual IXmlSerializable instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to IXmlSerializable
		/// </param>
		/// <returns>
		/// A IXmlSerializable object deserialized from the string
		/// </returns>
		public static IXmlSerializable ToIXmlSerializable(string value, Type destinyType)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;
			if (destinyType == null) throw new ArgumentNullException("destinyType");

			//Creating serializer with the destiny type
			XmlSerializer serializer = new XmlSerializer(destinyType);

			//Creating reader with the source value string representation
			System.IO.StringReader reader = new System.IO.StringReader(value);

			//Deserializing the source value to destiny type
			return (IXmlSerializable) serializer.Deserialize(reader);
		}

		/// <summary>
		/// Converts a IEnumerable string representantion into an actual IEnumerable instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to IEnumerable
		/// </param>
		/// <returns>
		/// A IEnumerable object deserialized from the string
		/// </returns>
		public static IEnumerable ToIEnumerable(string value, Type destinyType)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;
			if (destinyType == null) throw new ArgumentNullException("destinyType");

			//try to unparse as xml
			return (IEnumerable) ToIXmlSerializable(value, destinyType);
		}

		/// <summary>
		/// Creates a NameValueCollection filed with the values of the string
		/// </summary>
		/// <param name="value">
		/// String which will be atomized and represented in the NameValueCollection.
		/// Format must be Key1=Value1&Key2=Value2&Key3=Value3...
		/// </param>
		/// <returns>
		/// NameValueCollection filed with the atomized values of the string
		/// </returns>
		public static NameValueCollection ToNameValues(string queryString)
		{
			//null or empty values
			if (string.IsNullOrWhiteSpace(queryString)) return null;

			//split value by & characters to get a string of Key=Valye pairs
			//example: { "Key1=Value1", "Key2=Value2", "Key3=Value3" }
			string[] pairs = queryString.Split('&');

			//create collection
			NameValueCollection result = new NameValueCollection();

			//separate each key and value
			foreach (string pair in pairs)
			{
				string key, val;
				int equalSymbol = pair.IndexOf('=');

				//separate key and value by the = character
				//key = pair.Split('=')[0];
				//val = pair.Split('=')[1];
				key = pair.Substring(0, equalSymbol);
				val = pair.Substring(equalSymbol + 1);

				//add pair to collection
				result.Add(key, val);
			}

			//return result
			return result;
		}

		/// <summary>
		/// Gets the value of a key inside a string
		/// </summary>
		/// <param name="values">String that contains key value pairs. Must be formatted as Key1=Value1&Key2=Value2&Key3=Value3...</param>
		/// <param name="key">Key which value will be obtained from the string</param>
		/// <returns>Value of the specified key</returns>
		public static string GetValueFromQueryString(string queryString, string key)
		{
			//Validating arguments
			if (string.IsNullOrWhiteSpace(queryString)) throw new ArgumentNullException("queryString");
			if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");

			int i = queryString.IndexOf(key);

			if (i != -1)
			{
				int j = queryString.IndexOf('=', i);
				int k = queryString.IndexOf('&', j);

				if (j != -1)
				{
					if (k != -1)
					{
						return queryString.Substring(j + 1, k - (j + 1));
					}
					else
					{
						return queryString.Substring(j + 1);
					}
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Converts a Assembly string representantion into an actual Assembly instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to Assembly
		/// </param>
		/// <returns>
		/// A Assembly object deserialized from the string
		/// </returns>
		public static Assembly ToAssembly(string value)
		{
			if (string.IsNullOrWhiteSpace(value)) return null;

			return Assembly.LoadFrom
			(
				System.AppDomain.CurrentDomain.RelativeSearchPath + value
			);
		}

		/// <summary>
		/// Converts a DateTime string representantion into an actual DateTime instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to DateTime
		/// </param>
		/// <returns>
		/// A DateTime object deserialized from the string
		/// </returns>
		public static DateTime ToDateTime(string value)
		{
			DateTime result;

			if (string.IsNullOrWhiteSpace(value)) return NullValues.DateTime;

			if (DateTime.TryParseExact(value, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out result))
			{
				return result;
			}

			return result;
		}

		#endregion

		#region DataType, DataMember and DataObject

		/// <summary>
		/// Converts a DataType string representantion into an actual DataType instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to DataType
		/// </param>
		/// <returns>
		/// A DataType object deserialized from the string
		/// </returns>
		public static DataType ToDataType(string value)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;

			//try to unparse as xml
			return (DataType)ToIStringSerializable(value, typeof(DataType));
		}

		/// <summary>
		/// Returns a DataType created by deserializing an xml element
		/// </summary>
		/// <param name="xml">
		/// XmlElement containing a serialized DataType
		/// </param>
		/// <returns>
		/// A deserialized DataType
		/// </returns>
		public static DataType ToDataType(XmlElement xml)
		{
			DataType dtype;

			//Validating if the argumetn is null
			if (xml == null) return null;
			if (string.IsNullOrWhiteSpace(xml.InnerXml)) return null;

			//retrieve DataType
			dtype = DataType.Parse(xml.GetAttribute("InnerType"));

			//deserialize
			return dtype;
		}

		/// <summary>
		/// Returns a DataType created by deserializing an XElement
		/// </summary>
		/// <param name="xml">
		/// XElement containing a serialized DataType
		/// </param>
		/// <returns>
		/// A deserialized DataType
		/// </returns>
		public static DataType ToDataType(XElement xml)
		{
			return ToDataType(xml.ToXmlElement());
		}

		/// <summary>
		/// Converts a DataMember string representantion into an actual DataMember instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to DataMember
		/// </param>
		/// <returns>
		/// A DataMember object deserialized from the string
		/// </returns>
		public static DataMember ToDataMember(string value)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;

			//try to unparse as xml
			return (DataMember)ToIStringSerializable(value, typeof(DataMember));
		}
		
		/// <summary>
		/// Returns a DataMember created by deserializing an xml element
		/// </summary>
		/// <param name="xml">
		/// XmlElement containing a serialized DataMember
		/// </param>
		/// <returns>
		/// A deserialized DataMember
		/// </returns>
		public static DataMember ToDataMember(XmlElement xml)
		{
			DataType dtype;
			string name;

			//Validating if the argumetn is null
			if (xml == null) return null;
			if (string.IsNullOrWhiteSpace(xml.InnerXml)) return null;

			//retrieve DataType
			dtype = DataType.Parse(xml.GetAttribute("DataType"));

			//retrieve DataMemeber's name
			name = xml.GetAttribute("Name");

			//return DataMember
			return dtype[name];
		}

		/// <summary>
		/// Returns a DataMember created by deserializing an XElement
		/// </summary>
		/// <param name="xml">
		/// XElement containing a serialized DataMember
		/// </param>
		/// <returns>
		/// A deserialized DataMember
		/// </returns>
		public static DataMember ToDataMember(XElement xml)
		{
			return ToDataMember(xml.ToXmlElement());
		}
		
		/// <summary>
		/// Creates and populates a DataObject based on a query string values
		/// </summary>
		/// <param name="value">
		/// Query string containing a serialized DataObject
		/// </param>
		/// <returns>
		/// A populated DataObject
		/// </returns>
		public static DataObject ToDataObject(string value)
		{
			DataObject dobj;
			DataType dtype;

			//Validating if the sting is null
			if (string.IsNullOrWhiteSpace(value)) return null;

			//deserialize datatype
			dtype = ToDataType(GetValueFromQueryString(value, "DataType"));

			//create new DataObject
			dobj = DataObject.From(dtype);

			//deserialize values
			((IStringSerializable)dobj).DeserializeFromString(value);

			return dobj;
		}

		/// <summary>
		/// Returns a DataObject populated by deserializing an xml element
		/// </summary>
		/// <param name="xml">
		/// XmlElement containing a serialized DataObject
		/// </param>
		/// <returns>
		/// A populated DataObject
		/// </returns>
		public static DataObject ToDataObject(XmlElement xml)
		{
			DataType dtype;

			//Validating if the argumetn is null
			if (xml == null) return null;
			if (string.IsNullOrWhiteSpace(xml.InnerXml)) return null;

			//retrieve DataType
			dtype = DataType.Parse(xml.GetAttribute("DataType"));

			//deserialize
			return (DataObject) ToIXmlSerializable(xml.OuterXml, dtype.InnerType);
		}

		/// <summary>
		/// Returns a DataObject populated by deserializing an XElement
		/// </summary>
		/// <param name="xml">
		/// XElement containing a serialized DataObject
		/// </param>
		/// <returns>
		/// A populated DataObject
		/// </returns>
		public static DataObject ToDataObject(XElement xml)
		{
			return ToDataObject(xml.ToXmlElement());
		}

		/// <summary>
		/// Returns a DataObject with all the DataValues
		/// whose values exists in the values collection
		/// </summary>
		/// <param name="dtype">
		/// DataType of the DataObject whose DataValues will be loaded
		/// </param>
		/// <param name="valuesString">
		/// NameValueCollection with the next structure:
		/// 
		///		- Name: Name of the DataValue referenced by the item
		///		- Value: String representation of the value of referenced DataValue
		///		
		/// </param>
		/// <returns>
		/// DataObject with all the DataValues
		/// of the specified DataType whose values exists in the values
		/// collection
		/// </returns>
		public static DataObject ToDataObject(NameValueCollection values)
		{
			return ToDataObject(SerializeToString(values));
		}

		/// <summary>
		/// Converts a DataType to it's string representantion
		/// </summary>
		/// <param name="value">
		/// DataType to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of DataType
		/// </returns>
		public static string ToString(DataType value)
		{
			//null values
			if (value == null) return null;

			return ((IStringSerializable)value).SerializeToString();
		}

		/// <summary>
		/// Converts a DataMember to it's string representantion
		/// </summary>
		/// <param name="value">
		/// DataMember to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of DataMember
		/// </returns>
		public static string ToString(DataMember value)
		{
			//null values
			if (value == null) return null;

			return ((IStringSerializable)value).SerializeToString();
		}

		/// <summary>
		/// Converts a DataObject to it's string representantion
		/// </summary>
		/// <param name="value">
		/// DataObject to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of DataObject
		/// </returns>
		public static string ToString(DataObject value)
		{
			//null values
			if (value == null) return null;

			return ((IStringSerializable)value).SerializeToString();
		}

		/// <summary>
		/// Creates a string representation of a List
		/// </summary>
		/// <param name="dataValuesInstances">List that will be parsed as a string</param>
		/// <returns>String containing all values in dataValuesInstances</returns>
		public static string ToString(List<DataValueInstance> value)
		{
			//First, convert to NameValueCollection
			NameValueCollection nameValues = ToNameValues(value);

			//Now convert to string
			return SerializeToString(nameValues);
		}

		/// <summary>
		/// Creates a NameValueCollection filed with the atomized values of the DataObject
		/// </summary>
		/// <param name="dobj">
		/// DataObject which will be atomized and represented in the NameValueCollection
		/// </param>
		/// <returns>
		/// NameValueCollection filed with the atomized values of the List
		/// </returns>
		public static NameValueCollection ToNameValues(DataObject value)
		{
			if (value == null) return null;

			NameValueCollection values = new NameValueCollection();

			values.Add("DataType", ((IStringSerializable)value.DataType).SerializeToString());
			values.Add(ToNameValues(value.PrimaryKey));

			return values;
		}

		/// <summary>
		/// Creates a NameValueCollection filed with the atomized values of the List
		/// </summary>
		/// <param name="dataValuesInstances">
		/// List which will be atomized and represented in the NameValueCollection
		/// </param>
		/// <returns>
		/// NameValueCollection filed with the atomized values of the List
		/// </returns>
		public static NameValueCollection ToNameValues(List<DataValueInstance> values)
		{
			//Validating if the dataProperties argument is null
			if (values == null) return null;

			//Creating the result
			NameValueCollection result = new NameValueCollection();

			//Filling the atomized dictionary
			DataValueInstanceAtomizedDictionary atomized;
			atomized = new DataValueInstanceAtomizedDictionary(values);

			//Crossing the DataValues candidates to load
			foreach (KeyValuePair<string, DataValueInstance> dvi in atomized)
			{
				//Validating if the value was specified on dicResult collection
				if (!NullValues.IsNull(dvi.Value.Value))
				{
					//Adding the value to the collection
					result.Add(dvi.Key, TypeConverter.SerializeToString(dvi.Value.Value));
				}
			}

			//Returning the NameValueCollection
			return result;
		}

		/// <summary>
		/// Populates a List&lt;DataValueInstance&gt; with the atomized fields in queryString
		/// </summary>
		/// <param name="queryString">
		/// QueryString that contains atomized names and values of DataValues
		/// </param>
		/// <param name="instances">
		/// Collection that will be populated from queryString
		/// </param>
		public static void ToDataValueInstances(string queryString, List<DataValueInstance> instances)
		{
			//Validating if the dataProperties argument is null
			if (string.IsNullOrWhiteSpace(queryString)) return;
			if (instances == null) throw new ArgumentNullException("instances");

			NameValueCollection nameValues = ToNameValues(queryString);

			//populating instances
			ToDataValueInstances(nameValues, instances);
		}

		/// <summary>
		/// Populates a List&lt;DataValueInstance&gt; with the atomized fields in values
		/// </summary>
		/// <param name="values">
		/// Collection that contains atomazed names and values of DataValues
		/// </param>
		/// <param name="instances">
		/// Collection that will be populated from values
		/// </param>
		public static void ToDataValueInstances(NameValueCollection values, List<DataValueInstance> instances)
		{
			//Validating if the dataProperties argument is null
			if (values == null) return;
			if (instances == null) throw new ArgumentNullException("instances");

			//Filling the atomized dictionary
			DataValueInstanceAtomizedDictionary atomized;
			atomized = new DataValueInstanceAtomizedDictionary(instances);

			//copy values from values to atomized
			foreach (KeyValuePair<string, DataValueInstance> dvi in atomized)
			{
				//Validating if the value was specified on dicResult collection
				if (!NullValues.IsNull(values[dvi.Key]))
				{
					//Adding the value to the collection
					dvi.Value.Value = TypeConverter.ChangeType(values[dvi.Key], dvi.Value.DataValue.ValueType);
				}
			}
		}

		#endregion
	}
}