using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code
{
	[Table("Method")]
	public class Method : Member
	{
		[Required]
		public bool IsAbstract
		{
			get; set;
		}

		[Required]
		public bool IsVirtual
		{
			get; set;
		}

		public String MethodCode
		{
			get; set;
		}

		public String MethodBodyCode
		{
			get 
			{
				if (string.IsNullOrWhiteSpace(MethodCode))
				{
					return null;
				}

				return new System.Text.RegularExpressions.Regex(@"(?<=\{).*(?=\})", System.Text.RegularExpressions.RegexOptions.Singleline).Match(MethodCode).Value.Trim();
			}
		}
		
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Method;
			}
		}

		public bool IsGeneric
		{
			get
			{
				return GenericArguments.Count<MethodGenericArgument>() > 0;
			}
		}

		public bool IsConstructor
		{
			get
			{
				return this.Name == Type.Name && this.ReturnType == Type;
			}
		}

		public bool IsPropertyMethod
		{
			get
			{
				return Name.StartsWith("get_") || Name.StartsWith("set_");
			}
		}

		public override string FullName
		{
			get
			{
				//string args = string.Empty;
				
				//foreach (MethodArgument a in Arguments)
				//{
				//	args += a.Type.Name + ", ";
				//}

				//args = args.TrimEnd(' ', ',');

				//return string.Format("{0}.{1}({2})", Type.FullName, Name, args);

				StringBuilder sb = new StringBuilder();

				sb.Append(base.FullName);
				sb.Append("(");
				
				foreach (MethodArgument a in Arguments)
				{
					sb.Append(a.Type.Name + ", ");
				}
				
				sb.Append(")");

				if (IsGeneric)
				{
					sb.Append('<');
					
					foreach (MethodGenericArgument arg in from ga in GenericArguments orderby ga.Position select ga)
					{
						sb.Append(arg);
						
						if (GenericArguments.ToList<MethodGenericArgument>().IndexOf(arg) < GenericArguments.Count<MethodGenericArgument>())
						{
							sb.Append(',');
						}
					}

					sb.Append('>');
				}

				return sb.ToString();
			}
		}

		public virtual List<MethodArgument> Arguments
		{
			get; set;
		}

		public virtual List<MethodGenericArgument> GenericArguments
		{
			get; set;
		}

		public Method()
		{
		}
	}
}