using System;
using System.Collections.Generic;
using System.Text;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Event arguments used before and after every select operation over a DataType
	/// </summary>
	public class SelectEventArgs
	{
		/// <summary>
		/// The DataType which is affected by the operation
		/// </summary>
		public readonly DataType DataType;

		/// <summary>
		/// List of values selected by the operation
		/// </summary>
		public readonly List<DataValue> SelectedValues;

		/// <summary>
		/// Filters used to make the select operation
		/// </summary>
		public readonly Filters.FilterList Filters;

		/// <summary>
		/// Order by items that will determine the order of the result
		/// </summary>
		public readonly List<OrderByItem> OrderBy;

		/// <summary>
		/// When it is set to true, it cancells the select operation and the DataBase returns an empty collection
		/// </summary>
		/// <remarks>Usefull to perform custom Select operations, use custom sql scripts or use an external data source like a web service</remarks>
		public bool Cancel;

		/// <summary>
		/// The result of the select operation
		/// </summary>
		public DataObjectList Result;

		/// <summary>
		/// Constructs the argument
		/// </summary>
		public SelectEventArgs(DataType dtype, Filters.FilterList filters, List<OrderByItem> orderBy): this(dtype, filters, orderBy, dtype.AllValues)
		{
		}
		
		/// <summary>
		/// Constructs the argument
		/// </summary>
		public SelectEventArgs(DataType dtype, Filters.FilterList filters, List<OrderByItem> orderBy, List<DataValue> selectedValues)
		{
			this.DataType = dtype;
			this.SelectedValues = selectedValues;
			this.Filters = filters;
			this.OrderBy = orderBy;
		}
	}
}