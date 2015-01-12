using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code
{
	public class TypeAttribute
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}

		[Required]
		public Type AppliedTo
		{
			get;
			set;
		}

		public Guid AppliedToId { get; set; }

		[Required]
		public OKHOSTING.Code.RunTime.Instance Attribute
		{
			get;
			set;
		}

		public System.Guid AttributeId { get; set; }

		public TypeAttribute()
		{
		}
	}
}