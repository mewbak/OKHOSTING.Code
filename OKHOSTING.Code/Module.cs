using OKHOSTING.Code.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code
{
	[System.ComponentModel.DefaultProperty("Name")]
	public class Module: ITemplatable
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

		public virtual List<Module> References
		{
			get; set;
		}

		public virtual List<Module> ReferencedBy
		{
			get; set;
		}

		public virtual List<Type> Types
		{
			get; set;
		}

		public virtual List<SpecificModuleTemplate> SpecificModuleTemplates
		{
			get; set;
		}

		#region ITemplatable

		[System.ComponentModel.Browsable(false)]
		public string NameResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderName(this);
			}
		}

		[System.ComponentModel.Browsable(false)]
		public string ContentResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderContent(this);
			}
		}

		[System.ComponentModel.Browsable(false)]
		public string FilePathResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderFilePath(this);
			}
		}

		[System.ComponentModel.Browsable(false)]
		[NotMapped]
		public Language ActiveLanguage
		{
			get; set;
		}

		#endregion

		public Module()
		{
		}
	}
}