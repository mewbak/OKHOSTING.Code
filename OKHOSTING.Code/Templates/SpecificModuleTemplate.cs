using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Templates
{
	[Table("SpecificModuleTemplate")]
	public class SpecificModuleTemplate : Template
	{
		/// <summary>
		/// This template will only be used for this specific member
		/// </summary>
		[Required]
		public virtual Module Module
		{
			get; set;
		}

		public System.Guid? ModuleId { get; set; }

		public override string FullName
		{
			get
			{
				return base.FullName + "[Module=" + Module + "]";
			}
		}

		public SpecificModuleTemplate()
		{
		}
	}
}