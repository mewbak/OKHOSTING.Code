using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code
{
	[Table("Interface")]
	public class Interface : Type
	{
		//public override MemberTypes SupportedMemberTypes
		//{
		//	get
		//	{
		//		return MemberTypes.Field | MemberTypes.Property | MemberTypes.Method | MemberTypes.Event;
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

		public IQueryable<Property> Properties
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Property
					   select (Property)m;
			}
		}

		public IQueryable<Method> Methods
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Method
					   select (Method)m;
			}
		}

		public IQueryable<Event> Events
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Event
					   select (Event)m;
			}
		}

		public List<Class> ImplementedBy
		{
			get; set;
		}

		public List<Interface> Requires
		{
			get; set;
		}

		public List<Interface> RequiredBy
		{
			get; set;
		}

		public override TypeSubClass SubClass
		{
			get
			{
				return TypeSubClass.Interface;
			}
		}

		public Interface()
		{
		}
	}
}