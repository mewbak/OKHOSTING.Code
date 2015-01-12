using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code
{
	/// <summary>
	/// An argument of a method
	/// </summary>
	public class MethodArgument
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}

		[StringLength(250)]
		[Required]
		public string Name
		{
			get; set;
		}

		[Required]
		public Type Type
		{
			get; set;
		}

		[Required]
		public Method Method
		{
			get; set;
		}

		public MethodArgument()
		{
		}
	}
}