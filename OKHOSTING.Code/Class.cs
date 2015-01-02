using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace OKHOSTING.Code
{
	[Table("Class")]
	[DefaultProperty("FullName")]
	public class Class : Type
	{
		/// <summary>
		/// Support for inhetitance
		/// </summary>
		public virtual Class Parent
		{
			get; set;
		}

		public System.Guid? ParentId { get; set; }

		public bool IsAbstract
		{
			get; set;
		}

		public bool IsSealed
		{
			get; set;
		}

		public override TypeSubClass SubClass
		{
			get
			{
				return TypeSubClass.Class;
			}
		}

		//public override MemberTypes SupportedMemberTypes
		//{
		//	get 
		//	{ 
		//		return MemberTypes.All; 
		//	}
		//}

		/// <summary>
		/// Classes that inherit from this class
		/// </summary>

		public virtual List<Class> ParentOf
		{
			get; set;
		}

		/// <summary>
		/// Types defined inside this class
		/// </summary>

		public virtual List<Type> SubTypes
		{
			get; set;
		}

		public virtual List<Interface> ImplementedInterfaces
		{
			get; set;
		}

		public IQueryable<Field> Fields
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Field
					   orderby m.Name
					   select (Field)m;
			}
		}

		public IQueryable<Field> Constants
		{
			get
			{
				return from m in Fields.AsQueryable()
					   where m.IsLiteral
					   orderby m.Name
					   select m;
			}
		}

		public IQueryable<Property> Properties
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Property
					   orderby m.Name
					   select (Property)m;
			}
		}

		public IQueryable<Property> ForeignKeys
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Property && m.ReturnType is Class && m.ReturnType.Name != "System.String"
					   orderby m.Name
					   select (Property)m;
			}
		}

		public IQueryable<Method> Methods
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Method
					   orderby m.Name
					   select (Method)m;
			}
		}

		public IQueryable<Method> Constructors
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Method && ((Method) m).IsConstructor
					   orderby m.Name
					   select (Method)m;
			}
		}

		public IQueryable<Method> Destructors
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Method && m.Name == "~" + Name
					   orderby m.Name
					   select (Method)m;
			}
		}

		public IQueryable<Event> Events
		{
			get
			{
				return from m in Members.AsQueryable()
					   where m is Event
					   orderby m.Name
					   select (Event)m;
			}
		}

		public IQueryable<Property> PrimaryKey
		{
			get
			{
				return from p in Properties.AsQueryable()
					   where p.Key
					   orderby p.Name
					   select p;
			}
		}

		public virtual IQueryable<Property> ExternalForeignKeys
		{
			get 
			{
				//return new System.Collections.Generic.List<Property>(Session, new BinaryOperator("ReturnType", this));
				return from p in DataBase.Current.Set<Property>() where p.ReturnType == this select p;
			}
		}

		public bool IsPersistent
		{
			get
			{
				//reverse engineer attributes manually to see if we can deduce if thios property is required or has a string lenght
				foreach (TypeAttribute att in this.Attributes)
				{
					//hardcoded for "Required" attribute
					if (att.Attribute.Type.Name.Contains("NonPersistent") || att.Attribute.Type.Name.Contains("NotMapped"))
					{
						return false;
					}
				}

				return IsPublic && PrimaryKey.Count<Property>() > 0;
			}
		}

		public Class()
		{
		}
	}
}