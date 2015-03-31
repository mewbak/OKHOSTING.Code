using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Defines the method that a class must implement to be serialized and deserialized as a simple string
	/// </summary>
	public interface IStringSerializable
	{
		/// <summary>
		/// Must return a string representation of the object
		/// </summary>
		/// <returns>String representation of the object</returns>
		string SerializeToString();

		/// <summary>
		/// Deserializes an string a converts it to an object
		/// </summary>
		/// <param name="s">String that will be deserialized and converted to an object</param>
		void DeserializeFromString(string s);
	}
}
