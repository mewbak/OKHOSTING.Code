using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OKHOSTING.Code
{
	[Table("Enumeration")]
	public class Enumeration : Type
	{
		public Type UnderlyingType
		{
			get;
			set;
		}

		//public override MemberTypes SupportedMemberTypes
		//{
		//	get
		//	{
		//		return MemberTypes.Field;
		//	}
		//}

		public IQueryable<Field> Fields
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Field
					   select (Field)m;
			}
		}

		public override TypeSubClass SubClass
		{
			get
			{
				return TypeSubClass.Enumeration;
			}
		}

		public Enumeration()
		{
		}
	}
}