using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Templates
{
	[Table("SpecificTypeTemplate")]
	public class SpecificTypeTemplate : Template
	{
		/// <summary>
		/// This template will only be used for this specific type
		/// </summary>
		[Required]
		public virtual Type Type
		{
			get; set;
		}

		public System.Guid? TypeId { get; set; }

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