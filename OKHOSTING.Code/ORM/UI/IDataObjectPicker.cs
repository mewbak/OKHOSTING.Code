using System;

namespace OKHOSTING.Code.ORM.UI
{
	/// <summary>
	/// Represents a control for selecting a DataObject from a list
	/// </summary>
	public interface IDataObjectPicker
	{
		DataObject Selected
		{
			get;
			set;
		}
		
		DataType ListType
		{
			get;
			set;
		}
	}
}