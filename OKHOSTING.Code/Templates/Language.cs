using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OKHOSTING.Code.Templates
{
	[System.ComponentModel.DefaultProperty("FullName")]
	public class Language 
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Required]
		public Guid Id
		{
			get;
			set;
		}
		
		[StringLength(250)]
		[Required]
		public String Name
		{
			get; set;
		}

		public virtual Language Parent
		{
			get; set;
		}

		public System.Guid? ParentId { get; set; }

		public string DelimiterStart
		{
			get; set;
		}

		public string DelimiterStop
		{
			get; set;
		}

		public string FullName
		{
			get
			{
				var sb = new System.Text.StringBuilder();
				Language l = this;

				while (l != null)
				{
					sb.Insert(0, l.Name + "->");
					l = l.Parent;
				}

				//remove last dot
				sb.Remove(sb.Length - 2, 2);
				
				return sb.ToString();
			}
		}

		public virtual List<Language> SubLanguages
		{
			get; set;
		}

		public virtual List<Template> Templates
		{
			get; set;
		}

		public virtual List<LanguageGroupMember> Groups
		{
			get; set;
		}

		public Language()
		{
		}

		public string RenderName(ITemplatable templatable)
		{
			return Render(templatable, GetActiveTemplate(templatable).NameTemplate);
		}

		public string RenderContent(ITemplatable templatable)
		{
			return Render(templatable, GetActiveTemplate(templatable).ContentTemplate);
		}

		public string RenderFilePath(ITemplatable templatable)
		{
			return Render(templatable, GetActiveTemplate(templatable).FilePathTemplate);
		}

		protected void OnBeforeSave()
		{
			if (Parent != null && string.IsNullOrWhiteSpace(DelimiterStart) && string.IsNullOrWhiteSpace(DelimiterStop))
			{
				DelimiterStart = Parent.DelimiterStart;
				DelimiterStop = Parent.DelimiterStop;
			}
		}

		#region Protected code generation support members

		protected string Render(ITemplatable templatable, string template)
		{
			InitDelimiters();

			//change now for the temp delimiters
			template = template.Replace(DelimiterStart, TempDelimiterStart).Replace(DelimiterStop, TempDelimiterStop);
			
			string argumentName;
			if (templatable is StringProperty)
			{
				argumentName = typeof(Property).Name;
			}
			else
			{
				argumentName = templatable.GetType().BaseType.Name;
			}

			var stringTemplateGroup = new Antlr4.StringTemplate.TemplateGroup(TempDelimiterStart[0], TempDelimiterStop[0]);
			stringTemplateGroup.RegisterRenderer(typeof(AccessModifier), new LowerCaseRenderer());

			try
			{
				stringTemplateGroup.DefineTemplate("0", template, new string[] { argumentName });
			}
			catch (Exception e)
			{
				return null;
			}

			var stringTemplate = stringTemplateGroup.GetInstanceOf("0");
			stringTemplate.Add(argumentName, templatable);
			
			var result = stringTemplate.Render();
			return result;
		}
		
		protected Antlr4.StringTemplate.TemplateGroup TemplateGroup { get; set; }
		protected string TempDelimiterStart { get; set; }
		protected string TempDelimiterStop { get; set; }

		protected void InitDelimiters()
		{
			if (TempDelimiterStart != null && TempDelimiterStop != null)
			{
				return;
			}

			if (DelimiterStart.Length == 1)
			{
				TempDelimiterStart = DelimiterStart;
			}
			else
			{
				TempDelimiterStart = "$";
			}

			if (DelimiterStop.Length == 1)
			{
				TempDelimiterStop = DelimiterStart;
			}
			else
			{
				TempDelimiterStop = "$";
			}
		}

		protected Template GetActiveTemplate(ITemplatable templatable)
		{
			Template result = null;

			if (templatable is Module)
			{
				result = GetActiveTemplate((Module) templatable);
			}
			else if (templatable is Type)
			{
				result = GetActiveTemplate((Type)templatable);
			}
			else if (templatable is Member)
			{
				result = GetActiveTemplate((Member)templatable);
			}

			//create an empty template
			if (result == null)
			{
				if (templatable is Module)
				{
					result = new GenericModuleTemplate();
				}
				else if (templatable is Type)
				{
					result = new GenericTypeTemplate();
				}
				else if (templatable is Member)
				{
					result = new GenericMemberTemplate();
				}

				result.ContentTemplate = result.FilePathTemplate = result.NameTemplate = string.Empty;
			}

			return result;
		}

		protected Template GetActiveTemplate(Module module)
		{
			Language language = this;
			Template template = null;

			if (language.Templates == null)
			{
				DataBase.Current.Entry(language).Collection("Templates").Load();
			}

			do
			{
				//look for a specific type template 
				template =
					(
						from stt in language.Templates
						where (stt is SpecificModuleTemplate && ((SpecificModuleTemplate)stt).Module != null && ((SpecificModuleTemplate)stt).Module == module)
						select stt
					).FirstOrDefault<Template>();

				if (template != null)
				{
					return template;
				}

				language = language.Parent;
			}
			while (language != null);

			language = this;
			do
			{
				//look for a generic type template
				template =
					(
						from gtt in language.Templates
						where gtt is GenericModuleTemplate
						select gtt
					).FirstOrDefault<Template>();

				if (template != null)
				{
					return template;
				}

				language = language.Parent;
			}
			while (language != null);

			return null;
		}

		protected Template GetActiveTemplate(Type type)
		{
			Language language = this;
			Template template = null;

			do
			{
				//look for a specific type template 
				template =
					(
						from stt in language.Templates
						where (stt is SpecificTypeTemplate && ((SpecificTypeTemplate)stt).Type != null && ((SpecificTypeTemplate)stt).Type == type)
						select stt
					).FirstOrDefault<Template>();

				if (template != null)
				{
					return template;
				}

				language = language.Parent;
			}
			while (language != null);

			language = this;
			do
			{
				//look for a generic type template
				template =
					(
						from gtt in language.Templates
						where (gtt is GenericTypeTemplate && ((GenericTypeTemplate)gtt).TypeSubClass == type.SubClass)
						select (GenericTypeTemplate)gtt
					).FirstOrDefault<Template>();

				if (template != null)
				{
					return template;
				}

				language = language.Parent;
			}
			while (language != null);

			return null;
		}

		protected Template GetActiveTemplate(Member member)
		{
			Language language = this;
			Template template = null;

			do
			{
				//look for a specific member template
				template =
					(
						from smt in language.Templates
						where (smt is SpecificMemberTemplate && ((SpecificMemberTemplate)smt).Member != null && ((SpecificMemberTemplate)smt).Member == member)
						select smt
					).FirstOrDefault<Template>();

				if (template != null)
				{
					return template;
				}

				language = language.Parent;
			}
			while (language != null);

			language = this;
			do
			{
				//look for a generic member tempalte, specific for this memer's subclass and ReturnType
				template =
					(
						from gmt in language.Templates
						where (gmt is GenericMemberTemplate && ((GenericMemberTemplate)gmt).MemberSubClass == member.MemberType && ((GenericMemberTemplate)gmt).ReturnType == member.ReturnType)
						select gmt
					).FirstOrDefault<Template>();

				if (template != null)
				{
					return template;
				}

				language = language.Parent;
			}
			while (language != null);

			language = this;
			do
			{
				//look for a generic member tempalte, specific for this memer's subclass
				template =
					(
						from gmt in language.Templates
						where (gmt is GenericMemberTemplate && ((GenericMemberTemplate)gmt).MemberSubClass == member.MemberType)
						select gmt
					).FirstOrDefault<Template>();

				if (template != null)
				{
					return template;
				}

				language = language.Parent;
			}
			while (language != null);

			return null;
		}

		/// <summary>
		/// Replaces dots and <> with underlines to be able to use this name inside the TemplateGroup
		/// </summary>
		/// <param name="regularName"></param>
		protected static string GetTemplateName(string regularName)
		{
			return regularName.Replace('.', '_').Replace(' ', '_').Replace("<", "__").Replace(">", "__");
		}

		#endregion

		public static void ReadFromFiles()
		{
			//create languages for each directory
			foreach (var dir in new System.IO.DirectoryInfo(System.IO.Path.Combine(OKHOSTING.Tools.DefaultPaths.Custom, "Samples")).GetDirectories())
			{
				ReadFromFiles(dir);
			}
		}

		private static Language ReadFromFiles(System.IO.DirectoryInfo dir)
		{
			if (dir.Name == "Samples")
			{
				return null;
			}

			Language language = (from lang in DataBase.Current.Set<Language>() where lang.Name == dir.Name select lang).SingleOrDefault<Language>();

			if (language == null)
			{
				language = new Language();
				language.Name = dir.Name;
			}
			else
			{
				return language;
			}

			language.Parent = ReadFromFiles(dir.Parent);
			DataBase.Current.Set<Language>().Add(language);
			DataBase.Current.SaveChanges();

			foreach(TypeSubClass typeSubClass in Enum.GetValues(typeof(TypeSubClass)))
			{
				var filePath = System.IO.Path.Combine(dir.FullName, typeSubClass.ToString() + ".template");
				
				if(System.IO.File.Exists(filePath))
				{
					GenericTypeTemplate template = new GenericTypeTemplate();
					template.Language = language;
					template.TypeSubClass = typeSubClass;
					template.ContentTemplate = System.IO.File.ReadAllText(filePath);
					
					DataBase.Current.Set<GenericTypeTemplate>().Add(template);
				}
			}

			foreach(MemberTypes memberType in Enum.GetValues(typeof(MemberTypes)))
			{
				var filePath = System.IO.Path.Combine(dir.FullName, memberType.ToString() + ".template");
				
				if(System.IO.File.Exists(filePath))
				{
					GenericMemberTemplate template = new GenericMemberTemplate();
					template.Language = language;
					template.MemberSubClass = memberType;
					template.ContentTemplate = System.IO.File.ReadAllText(filePath);
					
					DataBase.Current.Set<GenericMemberTemplate>().Add(template);
				}
			}

			foreach (var subDir in dir.GetDirectories())
			{
				ReadFromFiles(subDir);
			}

			DataBase.Current.SaveChanges();

			return language;
		}
	}
}