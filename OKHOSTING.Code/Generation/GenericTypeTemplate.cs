using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code.Generation
{
	[Table("GenericTypeTemplate")]
	public class GenericTypeTemplate : Template
	{
		/// <summary>
		/// This template will only be used for this specific kind of type
		/// </summary>
		[Required]
		public TypeSubClass TypeSubClass
		{
			get; set;
		}

		public override string FullName
		{
			get
			{
				return base.FullName + "[TypeSubClass=" + TypeSubClass + "]";
			}
		}

		public GenericTypeTemplate()
		{
		}
	}
}