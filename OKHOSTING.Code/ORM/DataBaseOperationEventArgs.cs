using System;
using System.Collections.Generic;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Argument on events of database interaction
	/// </summary>
	public class DataBaseOperationEventArgs: EventArgs
	{
		/// <summary>
		/// The Dataobject which is affected by the operation
		/// </summary>
		public readonly DataObject DataObject;

		/// <summary>
		/// List of values affected by the operation
		/// </summary>
		public readonly List<DataValue> AffectedValues;

		/// <summary>
		/// Operation performed against the database
		/// </summary>
		public readonly DataBaseOperation Operation;

		/// <summary>
		/// When it is set to true, it cancells the operation and the DataBase returns an empty result
		/// </summary>
		/// <remarks>Usefull to perform custom operations, use custom sql scripts or use an external data source like a web service</remarks>
		public bool Cancel;

		/// <summary>
		/// Constructs the argument
		/// </summary>
		public DataBaseOperationEventArgs(DataBaseOperation operation, DataObject dobj, List<DataValue> affectedValues)
		{
			this.Operation = operation;
			this.DataObject = dobj;
			this.AffectedValues = affectedValues;
		}
		
		/// <summary>
		/// Constructs the argument
		/// </summary>
		public DataBaseOperationEventArgs(DataBaseOperation operation, DataObject dobj)
		{
			this.Operation = operation;
			this.DataObject = dobj;
			this.AffectedValues = dobj.DataType.AllValues;
		}
	}
}