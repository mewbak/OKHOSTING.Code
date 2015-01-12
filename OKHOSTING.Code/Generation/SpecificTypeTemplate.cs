using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Generation
{
	[Table("SpecificTypeTemplate")]
	public class SpecificTypeTemplate : Template
	{
		/// <summary>
		/// This template will only be used for this specific type
		/// </summary>
		[Required]
		public Type Type
		{
			get; set;
		}

		public override string FullName
		{
			get
			{
				return base.FullName + "[Type=" + Type + "]";
			}
		}

		public SpecificTypeTemplate()
		{
		}
	}
}