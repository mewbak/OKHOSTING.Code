using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code
{
	public enum AccessModifier
	{
		/// <summary>
		/// Only typess whitin the declaring Module can access this members
		/// </summary>
		Internal,

		/// <summary>
		/// Only the declaring class can access this member
		/// </summary>
		Private,

		/// <summary>
		/// Only subclassescan access this member
		/// </summary>
		Protected,

		/// <summary>
		/// Everyone can access this member
		/// </summary>
		Public,
	}
}