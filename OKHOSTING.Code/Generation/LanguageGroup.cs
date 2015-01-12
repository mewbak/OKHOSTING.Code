using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace OKHOSTING.Code.Generation
{
	[DefaultProperty("Name")]
	public class LanguageGroup 
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
		public string Name
		{
			get; set;
		}

		public List<LanguageGroupMember> Languages
		{
			get; set;
		}

		public LanguageGroup()
		{
		}

		public void RenderToFile(Module module)
		{
			string basePath = Path.Combine(OKHOSTING.Tools.DefaultPaths.Custom, "Softosis", Name, module.Name);

			if (!Directory.Exists(basePath))
			{
				Directory.CreateDirectory(basePath);
			}
			
			foreach (LanguageGroupMember langMember in Languages)
			{
				//set language for all modules
				foreach (var mod in DataBase.Current.Set<Module>())
				{
					((ITemplatable) mod).ActiveLanguage = langMember.Language;
				}

				string filePath = langMember.Language.RenderFilePath(module);
				string fullPath = Path.Combine(basePath, filePath);
				string directoryPath = Path.GetDirectoryName(fullPath);
				
				if (!string.IsNullOrWhiteSpace(filePath))
				{
					string content = langMember.Language.RenderContent(module);
					
					if (!Directory.Exists(directoryPath))
					{
						Directory.CreateDirectory(directoryPath);
					}

					System.IO.File.WriteAllText(fullPath, content);
				}

				foreach (Type type in module.Types)
				{
					filePath = langMember.Language.RenderFilePath(type);
					fullPath = Path.Combine(directoryPath, type.NameSpace.Replace(module.Name, string.Empty).Replace('.', '\\').TrimStart('\\'), filePath);
					directoryPath = Path.GetDirectoryName(fullPath);
					string content = langMember.Language.RenderContent(type);

					if (!string.IsNullOrWhiteSpace(filePath))
					{
						if (!Directory.Exists(directoryPath))
						{
							Directory.CreateDirectory(directoryPath);
						}

						System.IO.File.WriteAllText(fullPath, content);
					}
				}
			}
		}

		public void RenderToFile(Type type)
		{
			string basePath = Path.Combine(OKHOSTING.Tools.DefaultPaths.Custom, "Softosis", Name, type.Module.Name);

			foreach (LanguageGroupMember lang in Languages)
			{
				//set language for all modules
				foreach (var mod in DataBase.Current.Set<Module>())
				{
					((ITemplatable)mod).ActiveLanguage = lang.Language;
				}

				string filePath = lang.Language.RenderFilePath(type);
				string fullPath = Path.Combine(basePath, type.NameSpace.Replace('.', '\\'), filePath);
				string directoryPath = Path.GetDirectoryName(fullPath);
				string content = lang.Language.RenderContent(type);

				if (!string.IsNullOrWhiteSpace(filePath))
				{
					if (!Directory.Exists(directoryPath))
					{
						Directory.CreateDirectory(directoryPath);
					}

					System.IO.File.WriteAllText(fullPath, content);
				}
			}
		}
	}
}