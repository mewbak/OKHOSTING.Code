using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Event arguments used before and after every select group operation over a DataType
	/// </summary>
	public class SelectGroupEventArgs
	{
		/// <summary>
		/// The DataType which is affected by the operation
		/// </summary>
		public readonly DataType DataType;
		
		/// <summary>
		/// Aggregate fields definitions
		/// </summary>
		public List<AggregateSelectField> AggegateSelectFields;

		/// <summary>
		/// List of values selected by the operation
		/// </summary>
		public readonly List<DataValue> DataValuesToGroup;

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
		public DataTable Result;

		/// <summary>
		/// Constructs the argument
		/// </summary>
		public SelectGroupEventArgs(DataType dtype, List<AggregateSelectField> aggegateSelectFields, List<DataValue> dataValuesToGroup)
			: this(dtype, aggegateSelectFields, dataValuesToGroup, null, null)
		{
		}
		
		/// <summary>
		/// Constructs the argument
		/// </summary>
		public SelectGroupEventArgs(DataType dtype, List<AggregateSelectField> aggegateSelectFields, List<DataValue> dataValuesToGroup, Filters.FilterList filters, List<OrderByItem> orderBy)
		{
			this.DataType = dtype;
			this.AggegateSelectFields = aggegateSelectFields;
			this.DataValuesToGroup = dataValuesToGroup;
			this.Filters = filters;
			this.OrderBy = orderBy;
		}
	}
}