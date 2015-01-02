using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code
{
	public class Array: Class
	{
		[System.ComponentModel.DataAnnotations.Required]
		public Type ElementType
		{
			get; set;
		}

		[System.ComponentModel.DataAnnotations.Required]
		public int Rank
		{
			get; set;
		}

		public override string FullName
		{
			get
			{
				StringBuilder sb = new StringBuilder(base.FullName.Length + 2);

				sb.Append(base.FullName);
				sb.Append('[');
				
				if (Rank > 1)
				{
					sb.Append(',', Rank - 1);
				}

				sb.Append(']');

				return sb.ToString();
			}
		}

		public Array()
		{
		}
	}
}