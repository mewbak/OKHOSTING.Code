using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OKHOSTING.Code
{
	[Table("Event")]
	public class Event : Member
	{
		[Required]
		public Delegate Delegate
		{
			get; set;
			//TODO: set { ReturnType = value.InvokeMethod.ReturnType; }
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Event;
			}
		}

		public Event()
		{
		}
	}
}