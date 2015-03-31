using OKHOSTING.Code.Generation;
using System;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace OKHOSTING.Code
{
	/// <summary>
	/// Base type for classes and enumerations
	/// </summary>
	[System.ComponentModel.DefaultProperty("FullName")]
	public abstract class Type: ITemplatable
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
					return (from t in DataBase.Current.Set<Type>() where t.FullName == name select t).SingleOrDefault<Type>();
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

		public List<SpecificTypeTemplate> SpecificTypeTemplates
		{
			get; set;
		}

		#endregion

		#region ITemplatable

		[System.ComponentModel.Browsable(false)]
		public string NameResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderName(this);
			}
		}

		[System.ComponentModel.Browsable(false)]
		public string ContentResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderContent(this);
			}
		}

		[System.ComponentModel.Browsable(false)]
		public string FilePathResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderFilePath(this);
			}
		}

		private Language _ActiveLanguage = null;

		[System.ComponentModel.Browsable(false)]
		[NotMapped]
		public Language ActiveLanguage
		{
			get
			{
				if (_ActiveLanguage == null && ((ITemplatable)this.Module).ActiveLanguage != null)
				{
					return ((ITemplatable)this.Module).ActiveLanguage;
				}

				return _ActiveLanguage;
			}
			set
			{
				_ActiveLanguage = value;
			}
		}

		#endregion

		#region constructors
		
		public Type()
		{
		}

		#endregion

		#region static 

		//public static System.Collections.Generic.List<BaseObject> GetSetupObjects()
		//{
		//	//if users already exist in database, return null
		//	if (Objects<Type>().Count > 0) return null;

		//	List<BaseObject> data = new System.Collections.Generic.List<BaseObject>();

		//	Module mscorlib = new Module();
		//	mscorlib.Name = typeof(System.String).Module.Name;
		//	mscorlib.Version = typeof(System.String).Assembly.GetName().Version.ToString();
		//	mscorlib.Location = typeof(System.String).Assembly.Location;
		//	data.Add(mscorlib);

		//	foreach(TypeCode tc in Enum.GetValues(typeof(TypeCode)))
		//	{
		//		Struct st = new Struct();
		//		st.Name = tc.ToString();
		//		st.NameSpace = "System";
		//		st.Module = mscorlib;
		//		data.Add(st);
		//	}

		//	return data;
		//}

		#endregion
	}
}