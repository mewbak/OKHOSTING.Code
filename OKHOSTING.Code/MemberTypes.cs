using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code
{
	// Summary:
	//     Marks each type of member that is defined as a derived class of MemberInfo.
	public enum MemberTypes
	{
		// Summary:
		//     Specifies that the member is a constructor, representing a System.Reflection.ConstructorInfo
		//     member. Hexadecimal value of 0x01.
		Constructor = 1,
		//
		// Summary:
		//     Specifies that the member is an event, representing an System.Reflection.EventInfo
		//     member. Hexadecimal value of 0x02.
		Event = 2,
		//
		// Summary:
		//     Specifies that the member is a field, representing a System.Reflection.FieldInfo
		//     member. Hexadecimal value of 0x04.
		Field = 4,
		//
		// Summary:
		//     Specifies that the member is a method, representing a System.Reflection.MethodInfo
		//     member. Hexadecimal value of 0x08.
		Method = 8,
		//
		// Summary:
		//     Specifies that the member is a property, representing a System.Reflection.PropertyInfo
		//     member. Hexadecimal value of 0x10.
		Property = 16,
		//
		// Summary:
		//     Specifies that the member is a type, representing a MemberTypes.TypeInfo
		//     member. Hexadecimal value of 0x20.
		TypeInfo = 32,
		//
		// Summary:
		//     Specifies that the member is a custom member type. Hexadecimal value of 0x40.
		Custom = 64,
		//
		// Summary:
		//     Specifies that the member is a nested type, extending System.Reflection.MemberInfo.
		NestedType = 128,
		//
		// Summary:
		//     Specifies all member types.
		All = 191,
	}
}
