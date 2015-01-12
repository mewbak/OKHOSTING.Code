using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Generation
{
	[Table("SpecificModuleTemplate")]
	public class SpecificModuleTemplate : Template
	{
		/// <summary>
		/// This template will only be used for this specific member
		/// </summary>
		[Required]
		public Module Module
		{
			get; set;
		}

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