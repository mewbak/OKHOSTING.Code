using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code
{
	[Table("Field")]
	public class Field : Member
	{
		/// <summary>
		/// Is this a constant?
		/// </summary>
		public bool IsLiteral
		{
			get; set;
		}
		
		/// <summary>
		/// Is this a readonly field?
		/// </summary>
		public bool IsInitOnly
		{
			get; set;
		}

		public object LiteralValue
		{
			get; set;
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Field;
			}
		}

		public Field()
		{
		}
	}
}