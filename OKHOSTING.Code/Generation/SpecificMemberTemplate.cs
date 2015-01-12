using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Generation
{
	[Table("SpecificMemberTemplate")]
	public class SpecificMemberTemplate : Template
	{
		/// <summary>
		/// This template will only be used for this specific member
		/// </summary>
		[Required]
		public Member Member
		{
			get; set;
		}

		public override string FullName
		{
			get
			{
				return base.FullName + "[Member=" + Member + "]";
			}
		}

		public SpecificMemberTemplate()
		{
		}
	}
}