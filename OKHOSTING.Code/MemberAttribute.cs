using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using OKHOSTING.Code.Templates;

namespace OKHOSTING.Code
{
	[System.ComponentModel.DefaultProperty("Name")]
	public class MemberAttribute 
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}

		[Required]
		public virtual RunTime.Instance Attribute
		{
			get; set;
		}

		public System.Guid? AttributeId { get; set; }

		[Required]
		public virtual Member AppliedTo
		{
			get; set;
		}

		public System.Guid? AppliedToId { get; set; }

		public MemberAttribute()
		{
		}
	}
}