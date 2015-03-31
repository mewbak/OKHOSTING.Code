using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	
	/// <summary>
	/// This class implements the values considered null on each
	/// primitive or common type
	/// </summary>
	public static class NullValues
	{
		/// <summary>
		/// Represents a System.Byte null
		/// </summary>
		public const int Byte = System.Byte.MaxValue;

		/// <summary>
		/// Represents a System.Int16 null
		/// </summary>
		public const short Int16 = System.Int16.MinValue;

		/// <summary>
		/// Represents a System.Int32 null
		/// </summary>
		public const int Int32 = System.Int32.MinValue;

		/// <summary>
		/// Represents a System.Int64 null
		/// </summary>
		public const long Int64 = System.Int64.MinValue;

		/// <summary>
		/// Represents a System.SByte null
		/// </summary>
		public const sbyte SByte = System.SByte.MinValue;

		/// <summary>
		/// Represents a System.UInt16 null
		/// </summary>
		public const ushort UInt16 = System.UInt16.MaxValue;

		/// <summary>
		/// Represents a System.UInt32 null
		/// </summary>
		public const uint UInt32 = System.UInt32.MaxValue;

		/// <summary>
		/// Represents a System.UInt64 null
		/// </summary>
		public const ulong UInt64 = System.UInt64.MaxValue;

		/// <summary>
		/// Represents a System.String null
		/// </summary>
		public const string String = null;

		/// <summary>
		/// Represents a System.Double null
		/// </summary>
		public const double Double = System.Double.MinValue;

		/// <summary>
		/// Represents a System.Single null
		/// </summary>
		public const float Single = System.Single.MinValue;

		/// <summary>
		/// Represents a System.Decimal null
		/// </summary>
		public const decimal Decimal = System.Decimal.MinValue;

		/// <summary>
		/// Represents a System.DateTime null
		/// </summary>
		public static DateTime DateTime
		{ 
			get 
			{ 
				return System.DateTime.MinValue; 
			} 
		}

		/// <summary>
		/// Represents a System.TimeSpan null
		/// </summary>
		public static TimeSpan TimeSpan
		{
			get
			{
				return System.TimeSpan.Zero;
			}
		}

		/// <summary>
		/// Returns a boolean value that indicates if the specified value
		/// is considered a null value
		/// </summary>
		/// <param name="value">
		/// Value to validate
		/// </param>
		/// <returns>
		/// true if the specified value is considered a null value, otherwise false
		/// </returns>
		public static bool IsNull(object value)
		{
			//Local vars
			bool retValue = true;

			//validate regular null
			if (value == null) return true;

			//is this a DataObject?
			if (value is DataObject)
			{
				return IsNull((DataObject)value);
			}
			
			//Getting the type name of the value
			string typeName = value.GetType().FullName.Trim().ToLower();

			//Validating if the value to validate is null
			retValue =
			(
				(value == System.DBNull.Value) ||
				(typeName == "system.string" && (string)value == NullValues.String) ||
				(typeName == "system.byte" && (byte)value == NullValues.Byte) ||
				(typeName == "system.int16" && (short)value == NullValues.Int16) ||
				(typeName == "system.int32" && (int)value == NullValues.Int32) ||
				(typeName == "system.int64" && (long)value == NullValues.Int64) ||
				(typeName == "system.sbyte" && (sbyte)value == NullValues.SByte) ||
				(typeName == "system.uint16" && (ushort)value == NullValues.UInt16) ||
				(typeName == "system.uint32" && (uint)value == NullValues.UInt32) ||
				(typeName == "system.uint64" && (ulong)value == NullValues.UInt64) ||
				(typeName == "system.decimal" && (decimal)value == NullValues.Decimal) ||
				(typeName == "system.double" && (double)value == NullValues.Double) ||
				(typeName == "system.single" && (float)value == NullValues.Single) ||
				(typeName == "system.datetime" && (DateTime)value == NullValues.DateTime) ||
				(typeName == "system.timespan" && (TimeSpan)value == NullValues.TimeSpan)
			);

			return retValue;
		}

		/// <summary>
		/// Returns a boolean value that indicates if the specified DataObject
		/// is considered a null value
		/// </summary>
		/// <param name="value">
		/// DataObject to validate
		/// </param>
		/// <returns>
		/// true if the specified DataObject is considered a null value, otherwise false
		/// </returns>
		/// <remarks>One single null value in the primary key indicates the DataObject must be considered null</remarks>
		public static bool IsNull(DataObject value)
		{
			//validate regular null
			if (value == null) return true;

			///One null value in the primary key indicates the DataObject must be considered null
			foreach (DataValueInstance dvi in value.PrimaryKey)
			{
				if (IsNull(dvi.Value)) return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the initial value for the specified type. The initial 
		/// value is the null value for each type or the value considered
		/// the default value when not exists a null value
		/// </summary>
		/// <param name="type">
		/// Type for get initial value
		/// </param>
		/// <returns>
		/// Initial value for the specified type
		/// </returns>
		public static object GetNullValue(Type type)
		{
			//Local vars
			object nullValue = null;

			//Getting the null value for the specified type
			switch (type.FullName.Trim().ToLower())
			{
				case "system.string": nullValue = NullValues.String; break;
				case "system.byte": nullValue = NullValues.Byte; break;
				case "system.int16": nullValue = NullValues.Int16; break;
				case "system.int32": nullValue = NullValues.Int32; break;
				case "system.int64": nullValue = NullValues.Int64; break;
				case "system.sbyte": nullValue = NullValues.SByte; break;
				case "system.uint16": nullValue = NullValues.UInt16; break;
				case "system.uint32": nullValue = NullValues.UInt32; break;
				case "system.uint64": nullValue = NullValues.UInt64; break;
				case "system.decimal": nullValue = NullValues.Decimal; break;
				case "system.double": nullValue = NullValues.Double; break;
				case "system.single": nullValue = NullValues.Single; break;
				case "system.datetime": nullValue = NullValues.DateTime; break;
				case "system.timespan": nullValue = NullValues.TimeSpan; break;
				case "system.boolean": nullValue = false; break;
				default: nullValue = null; break;
			}

			//Returning the null value
			return nullValue;
		}
	}
}
