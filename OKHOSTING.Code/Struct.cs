using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code
{
	[Table("Struct")]
	public class Struct : Type
	{
		//public override MemberTypes SupportedMemberTypes
		//{
		//	get
		//	{
		//		return MemberTypes.Field | MemberTypes.Property | MemberTypes.Method;
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

		public IQueryable<Method> Constructors
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Method && m.Name == Name && m.ReturnType == this
					   select (Method)m;
			}
		}

		public override TypeSubClass SubClass
		{
			get 
			{ 
				return TypeSubClass.Struct; 
			}
		}

		public Struct()
		{
		}
	}
}