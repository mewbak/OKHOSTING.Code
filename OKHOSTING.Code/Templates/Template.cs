using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Templates
{
	[DefaultProperty("FullName")]
	public abstract class Template 
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public Guid Id
		{
			get;
			set;
		}

		[Required]
		public virtual Language Language
		{
			get; set;
		}

		public System.Guid LanguageId { get; set; }

		[StringLength(500)]
		public string NameTemplate
		{
			get; set;
		}
		public String ContentTemplate
		{
			get; set;
		}

		[StringLength(500)]
		public string FilePathTemplate
		{
			get; set;
		}

		public virtual string FullName
		{
			get
			{
				return this.GetType().Name;
			}
		}

		public Template()
		{
		}
	}
}