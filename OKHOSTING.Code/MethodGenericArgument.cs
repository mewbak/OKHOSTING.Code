using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code
{
	/// <summary>
	/// An argument for a generic member
	/// </summary>
	public class MethodGenericArgument 
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}

		/// <summary>
		/// Generic member that defines this argument
		/// </summary>
		public Method AppliedTo
		{
			get; set;
		}

		/// <summary>
		/// The Type of this argument itself, if this is a non constructed generic type, this will contain the "base" or "abstract"
		/// type of the generic type definition, otherwise, this will contain the defined types applied to this
		/// generic, in a final or runtime type
		/// </summary>
		public Type ArgumentType
		{
			get; set;
		}

		/// <summary>
		/// Position in the generic argument list
		/// </summary>
		public int Position
		{
			get; set;
		}

		public MethodGenericArgument()
		{
		}
	}
}