using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code
{
	[System.ComponentModel.DefaultProperty("Name")]
	public class Module
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}

		[StringLength(100)]
		[Required]
		public String Name
		{
			get; set;
		}

		[Required]
		public string Version
		{
			get; set;
		}

		public string Description
		{
			get; set;
		}

		/// <summary>
		/// If a location is set, and the file exist, no source code is generated for this module, instead, this is interpreted and used as an already implemented library
		/// just for proxying
		/// </summary>
		public string Location
		{
			get; set;
		}

		public List<Module> References
		{
			get; set;
		}

		public List<Module> ReferencedBy
		{
			get; set;
		}

		public List<Type> Types
		{
			get; set;
		}

		public Module()
		{
		}
	}
}