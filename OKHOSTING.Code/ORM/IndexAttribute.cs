using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Defines an Index on a DataObject
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class IndexAttribute : System.Attribute
	{
		#region Fields

		/// <summary>
		/// Name of the Index
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Names of the DataValues that conforms the Index
		/// </summary>
		public string[] DataValues;

		/// <summary>
		/// Determines if the index is a unique index
		/// </summary>
		public bool Unique;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the attribute
		/// </summary>
		/// <param name="name">
		/// Name of the Index
		/// </param>
		/// <param name="unique">
		/// Determines if the index is a unique index
		/// </param>
		/// <param name="dataValues">
		/// Names of the DataValues that conforms the Index
		/// </param>
		public IndexAttribute(string name, params string[] dataValues) : this(name, false, dataValues) { }

		/// <summary>
		/// Constructs the attribute
		/// </summary>
		/// <param name="name">
		/// Name of the Index
		/// </param>
		/// <param name="unique">
		/// Determines if the index is a unique index
		/// </param>
		/// <param name="dataValues">
		/// Names of the DataValues that conforms the Index
		/// </param>
		public IndexAttribute(string name, bool unique, params string[] dataValues)
		{
			if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name", "Argument can't be null or empty");
			if (dataValues == null) throw new ArgumentNullException("dataValues");
			if (dataValues.Length == 0) throw new ArgumentException("dataValues", "Argument can't be empty");
			
			this.Name = name;
			this.Unique = unique;
			this.DataValues = dataValues;
		}

		#endregion
	}
}
