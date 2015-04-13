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
		public Class Parent
		{
			get; set;
		}

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
		public List<Class> ParentOf
		{
			get; set;
		}

		/// <summary>
		/// Types defined inside this class
		/// </summary>
		public List<Type> SubTypes
		{
			get; set;
		}

		public List<Interface> ImplementedInterfaces
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

		/// <summary>
		/// Returns all parent Types ordered from base to child
		/// </summary>
		public List<Class> GetBaseClasses()
		{
			//Local Vars
			List<Class> types;
			Class current;

			//Creating list of Types
			types = new List<Class>();

			//Get all types in ascendent order (from base to child)
			current = this;
			while (current != null)
			{
				//Inserting the current object at the first position
				types.Insert(0, current);

				//Getting the parent of the current object
				current = current.Parent;
			}

			//Returning the List of DataTypes			
			return types;
		}

		public Class()
		{
		}
	}
}