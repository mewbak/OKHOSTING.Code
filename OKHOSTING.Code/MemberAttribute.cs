using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
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
		public RunTime.Instance Attribute
		{
			get; set;
		}

		[Required]
		public Member AppliedTo
		{
			get; set;
		}

		public MemberAttribute()
		{
		}
	}
}