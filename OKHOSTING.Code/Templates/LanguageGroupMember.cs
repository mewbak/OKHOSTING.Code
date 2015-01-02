using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Templates
{
	public class LanguageGroupMember 
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}

		public virtual Language Language
		{
			get;
			set;
		}

		public Guid LanguageId { get; set; }

		public virtual LanguageGroup Group
		{
			get;
			set;
		}

		public Guid GroupId { get; set; }

		public LanguageGroupMember()
		{
		}
	}
}