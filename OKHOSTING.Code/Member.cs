using OKHOSTING.Code.Generation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OKHOSTING.Code
{
	[DefaultProperty("Name")]
	public abstract class Member : ITemplatable
	{
		#region members

		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}

		[StringLength(50)]
		[Required]
		public String Name
		{
			get; set;
		}

		/// <summary>
		/// Class that contains this member
		/// </summary>
		[Required]
		public Type Type
		{
			get; set;
		}

		[InverseProperty("Members")]
		[Required]
		public AccessModifier Access
		{
			get; set;
		}

		[Required]
		public bool IsStatic
		{
			get; set;
		}

		[StringLength(250)]
		public String Notes
		{
			get; set;
		}

		public Type ReturnType
		{
			get; set;
		}

		//read only 

		public string FullName
		{
			get
			{
				StringBuilder sb = new StringBuilder();

				sb.Append(Type.FullName);
				sb.Append('.');
				sb.Append(Name);

				return sb.ToString();
			}
		}

		public abstract MemberTypes MemberType
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

		public override string ToString()
		{
			return Name;
		}

		#endregion

		#region collections

		public List<MemberAttribute> Attributes
		{
			get; set;
		}

		public List<SpecificMemberTemplate> SpecificMemberTemplates
		{
			get; set;
		}

		#endregion

		#region itemplatable

		[Browsable(false)]
		public string NameResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderName(this);
			}
		}

		[Browsable(false)]
		public string ContentResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderContent(this);
			}
		}

		[Browsable(false)]
		public string FilePathResult
		{
			get
			{
				return ((ITemplatable)this).ActiveLanguage.RenderFilePath(this);
			}
		}

		private Language _ActiveLanguage = null;

		[NotMapped]
		[Browsable(false)]
		public Language ActiveLanguage
		{
			get
			{
				if (_ActiveLanguage == null && ((ITemplatable)this.Type).ActiveLanguage != null)
				{
					return ((ITemplatable)this.Type).ActiveLanguage;
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

		public Member()
		{
		}

		#endregion
	}
}