using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code
{
	/// <summary>
	/// Base type for classes and enumerations
	/// </summary>
	[System.ComponentModel.DefaultProperty("FullName")]
	public abstract class Type
	{
		#region members

		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}

		[StringLength(100)]
		[Required]
		public string Name
		{
			get; set;
		}

		[StringLength(255)]
		public string NameSpace
		{
			get; set;
		}

		[Required]
		public AccessModifier Access
		{
			get; set;
		}

		/// <summary>
		/// Support for subtype definitions
		/// </summary>
		public Class Container
		{
			get; set;
		}

		public Module Module
		{
			get; set;
		}

		[StringLength(250)]
		public string Notes
		{
			get; set;
		}

		public bool IsArray
		{
			get; set;
		}

		public bool IsGenericParameter
		{
			get; set;
		}

		public bool IsGeneric
		{
			get
			{
				return GenericArguments.Count() > 0;
			}
		}

		public bool IsGenericTypeDefinition
		{
			get
			{
				if (!IsGeneric)
				{
					return false;
				}

				foreach (TypeGenericArgument arg in GenericArguments)
				{
					if (!arg.ArgumentType.IsGenericParameter)
					{
						return false;
					}
				}

				return true;
			}
		}

		public bool ContainsGenericParameters
		{
			get
			{
				foreach (TypeGenericArgument arg in GenericArguments)
				{
					if (arg.ArgumentType.IsGenericParameter)
					{
						return true;
					}
				}

				return false;
			}
		}

		public bool IsConstructedGenericType
		{
			get
			{
				return IsGeneric && !ContainsGenericParameters;
			}
		}

		public abstract TypeSubClass SubClass
		{
			get;
		}

		public bool IsPublic
		{
			get
			{
				return Access == AccessModifier.Public;
			}
		}

		//public abstract MemberTypes SupportedMemberTypes
		//{
		//	get;
		//}

		public string FullName
		{
			get
			{
				StringBuilder sb = new StringBuilder();

				sb.Append(NameSpace);
				sb.Append('.');
				sb.Append(Name);
				
				return sb.ToString();
			}
		}

		/// <summary>
		/// Returns the elementary type of this Type, in case this is an array or a generic type.
		/// For example, System.Int32[] would return System.Int32, and IList<System.String> would return System.String
		/// </summary>
		public Type ElementType
		{
			get
			{
				if (IsArray)
				{
					string name = FullName.TrimEnd('[', ' ', ',', ']');
					//return (from t in DataBase.Current.Set<Type>() where t.FullName == name select t).SingleOrDefault<Type>();
					return null;
				}
				else if (IsGeneric)
				{
					return GenericArguments.First().ArgumentType;
				}
				else
				{
					return null;
				}
			}
		}

		public override string ToString()
		{
			return FullName;
		}

		#endregion

		#region collections

		[InverseProperty("Type")]
		public List<Member> Members
		{
			get; set;
		}

		public List<TypeAttribute> Attributes
		{
			get; set;
		}

		[InverseProperty("AppliedTo")]
		public List<TypeGenericArgument> GenericArguments
		{
			get; set; //TODO: sort by position
		}

		#endregion
		
		public Type()
		{
		}
	}
}