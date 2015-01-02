using System.ComponentModel.DataAnnotations.Schema;

namespace OKHOSTING.Code
{
	[Table("Property")]
	public class Property : Member
	{
		public virtual Method GetMethod
		{
			get; set;
		}

		public virtual Method SetMethod
		{
			get; set;
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Property;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return (GetMethod != null && GetMethod.IsAbstract) || (SetMethod != null && SetMethod.IsAbstract);
			}
		}

		public bool IsPersistent
		{
			get
			{
				//reverse engineer attributes manually to see if we can deduce if thios property is required or has a string lenght
				foreach (MemberAttribute att in this.Attributes)
				{
					//hardcoded for "Required" attribute
					if (att.Attribute.Type.Name.Contains("NonPersistent") || att.Attribute.Type.Name.Contains("NotMapped"))
					{
						return false;
					}
				}

				return IsPublic && IsReadWrite && !IsAbstract;
			}
		}

		public bool IsList
		{
			get
			{
				return this.ReturnType.IsArray || this.ReturnType.Name.Contains("List") || this.ReturnType.Name.Contains("Collection") || this.ReturnType.Name.Contains("Enumerable");
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return GetMethod != null && SetMethod == null;
			}
		}

		public bool IsWriteOnly
		{
			get
			{
				return SetMethod != null && GetMethod == null;
			}
		}

		public bool IsReadWrite
		{
			get
			{
				return GetMethod != null && SetMethod != null;
			}
		}

		public Property()
		{
		}

		public void CreateGetMethod()
		{
			GetMethod = new Method();
			GetMethod.Name = "get_" + Name;
			GetMethod.Type = this.Type;
			GetMethod.ReturnType = ReturnType;
			GetMethod.Access = AccessModifier.Public;
		}

		public void CreateSetMethod()
		{
			SetMethod = new Method();
			SetMethod.Name = "set_" + Name;
			SetMethod.Type = this.Type;
			SetMethod.ReturnType = null;
			SetMethod.Access = AccessModifier.Public;

			MethodArgument ma = new MethodArgument() { Method = SetMethod, Type = this.ReturnType, Name = "value" };
		}
	}
}