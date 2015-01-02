
using System.ComponentModel.DataAnnotations.Schema;
namespace OKHOSTING.Code
{
	[Table("StringProperty")]
	public class StringProperty : Property
	{
		public int? MinLenght
		{
			get; set;
		}
		
		public int? MaxLenght
		{
			get; set;
		}

		public string RegexPattern
		{
			get; set;
		}

		public string Mask
		{
			get; set;
		}

		public StringProperty()
		{
		}
	}
}