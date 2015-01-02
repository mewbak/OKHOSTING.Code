using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Templates
{
	[Table("GenericMemberTemplate")]
	public class GenericMemberTemplate : Template
	{
		/// <summary>
		/// This template will only be used for this specific kind of member
		/// </summary>
		[Required]
		public MemberTypes MemberSubClass
		{
			get; set;
		}

		/// <summary>
		/// If ReturnTypeFilter is set, this template will only be used for members that have this specific ReturnType
		/// Otherwise, the template will be used as a general template for all members
		/// </summary>
		public virtual Type ReturnType
		{
			get; set;
		}

		public System.Guid? ReturnTypeId { get; set; }

		public override string FullName
		{
			get
			{
				return base.FullName + "[MemberSubClass=" + MemberSubClass + "]" + (ReturnType != null? "[ReturnType=" + ReturnType.Name + "]" : "");
			}
		}

		public GenericMemberTemplate()
		{
		}
	}
}