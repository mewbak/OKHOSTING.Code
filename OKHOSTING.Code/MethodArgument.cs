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
		public virtual Type Type
		{
			get; set;
		}

		public System.Guid? TypeId { get; set; }

		[Required]
		public virtual Method Method
		{
			get; set;
		}

		public System.Guid? MethodId { get; set; }

		public MethodArgument()
		{
		}
	}
}