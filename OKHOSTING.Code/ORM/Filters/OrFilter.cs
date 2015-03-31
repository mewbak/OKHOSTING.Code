using System;
using System.Collections.Generic;
using OKHOSTING.Code.ORM.Sql.Generators;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Filter defined with several conditions merged between them,
	/// with a logical Or operator
	/// </summary>
	public class OrFilter : LogicalOperatorFilter
	{

		#region Constructors

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public OrFilter() : base(LogicalOperator.Or, new FilterList()) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the Or operator
		/// </param>
		public OrFilter(FilterList innerFilters) : base(LogicalOperator.Or, innerFilters) { }

		#endregion

	}
}