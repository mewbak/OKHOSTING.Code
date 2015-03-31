using System;
using System.Collections.Generic;
using OKHOSTING.Code.ORM.Sql.Generators;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Filter defined with several conditions merged between them,
	/// with a logical And operator
	/// </summary>
	public class AndFilter : LogicalOperatorFilter
	{

		#region Constructors

		/// <summary>
		/// Constructs the filter
		/// </summary>
		public AndFilter() : base(LogicalOperator.And, new FilterList()) { }

		/// <summary>
		/// Constructs the filter
		/// </summary>
		/// <param name="innerFilters">
		/// Collection of conditions or filters that will be merged 
		/// with the And operator
		/// </param>
		public AndFilter(FilterList innerFilters) : base(LogicalOperator.And, innerFilters) { }

		#endregion

	}
}