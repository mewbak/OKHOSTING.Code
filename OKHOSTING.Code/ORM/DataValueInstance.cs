using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Represents an instance of a DataValue
	/// </summary>
	[Serializable]
	public class DataValueInstance
	{
		/// <summary>
		/// DataValue of the instance
		/// </summary>
		public readonly DataValue DataValue;

		/// <summary>
		/// DataObject that defines the DataValue instance
		/// </summary>
		public readonly DataObject DataObject;

		/// <summary>
		/// Constructs the DataValueInstance
		/// </summary>
		/// <param name="dobj">
		/// DataObject that defines the DataValue instance
		/// </param>
		/// <param name="dataValue">
		/// DataValue of the instance
		/// </param>
		public DataValueInstance(DataObject dobj, DataValue dataValue)
		{
			//Validating if the dobj and dataValue arguments are null
			if (dobj == null) throw new ArgumentNullException("dobj");
			if (dataValue == null) throw new ArgumentNullException("dataValue");

			//Initializing the class
			this.DataObject = dobj;
			this.DataValue = dataValue;
		}

		/// <summary>
		/// Gets or sets the value of the DataValue associated on the 
		/// DataObject referenced
		/// </summary>
		public object Value
		{
			get { return  DataObject.GetValue(DataValue); }
			set { DataObject.SetValue(DataValue, value); }
		}
	}
}