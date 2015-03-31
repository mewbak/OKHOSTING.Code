using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{

	/// <summary>
	/// Allows to have logical deletion of DataObjects, instead of phisycal
	/// </summary>
	public interface IDeleted
	{

		/// <summary>
		/// Determines whether the DataObject is logically deleted
		/// </summary>
		bool Deleted { get; set; }
	}
}