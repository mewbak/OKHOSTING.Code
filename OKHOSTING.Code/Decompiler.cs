// This file is part of the OWASP O2 Platform (http://www.owasp.org/index.php/OWASP_O2_Platform) and is released under the Apache 2.0 License (http://www.apache.org/licenses/LICENSE-2.0)
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.ILSpy;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OKHOSTING.Tools.Extensions;
using OKHOSTING.Code.Generation;

namespace OKHOSTING.Code
{
	public static class Decompiler
	{
		#region GetSourceCode

		private static AssemblyDefinition LoadAssembly(string path)
		{
			var resolver = new DefaultAssemblyResolver();
			resolver.AddSearchDirectory(Path.GetDirectoryName(path));
			
			var reader = new ReaderParameters()
			{
				AssemblyResolver = resolver
			};

			return AssemblyDefinition.ReadAssembly(path, reader);
		}

		private static void Decompile(AssemblyDefinition assemblyDefinition, string pathToSaveDecompiledSourceCode)
		{
			foreach (AssemblyNameReference reference in assemblyDefinition.MainModule.AssemblyReferences)
			{
				assemblyDefinition.MainModule.AssemblyResolver.Resolve(reference);
			}

			try
			{
				foreach (TypeDefinition definition in assemblyDefinition.MainModule.Types)
				{
					definition.ResolveOrThrow();

					StringBuilder builder = new StringBuilder();
					//builder.AppendLine(this.getSourceCode(definition));

					foreach (MethodDefinition definition2 in definition.Methods)
					{
						builder.AppendLine(GetSourceCode(definition2));
					}

					if (builder.Length > 0)
					{
						File.WriteAllText(Path.Combine(pathToSaveDecompiledSourceCode, definition.Name + ".cs"), builder.ToString());
					}
				}
			}
			catch (Exception exception)
			{
				//throw exception;
			}
		}

		private static string FormatControlFlowGraph(ControlFlowGraph cfg)
		{
			StringWriter writer = new StringWriter();
			writer.WriteLine("To Convert");
			/*foreach (InstructionBlock block in cfg.Blocks)
			{
				writer.WriteLine("block {0}:", block.Index);
				writer.WriteLine("\tbody:");
				foreach (Instruction instruction in block)
				{
					writer.Write("\t\t");
					InstructionData data = cfg.GetData(instruction);
					writer.Write("[{0}:{1}] ", data.StackBefore, data.StackAfter);
					Formatter.WriteInstruction(writer, instruction);
					writer.WriteLine();
				}
				InstructionBlock[] successors = block.Successors;
				if (successors.Length > 0)
				{
					writer.WriteLine("\tsuccessors:");
					foreach (InstructionBlock block2 in successors)
					{
						writer.WriteLine("\t\tblock {0}", block2.Index);
					}
				}
			}*/
			return writer.ToString();
		}

		private static string GetIL(MethodDefinition method)
		{
			try
			{
				StringBuilder builder = new StringBuilder();

				for (int i = 0; i < method.Body.Instructions.Count; i++)
				{
					Instruction instruction = method.Body.Instructions[i];
					string str = instruction.OpCode.ToString();

					if (instruction.Operand != null)
					{
						str = str + "   ...   " + instruction.Operand;
					}

					builder.AppendLine(str);
				}

				return builder.ToString();
			}
			catch (Exception exception)
			{
				//PublicDI.log.error("In getIL_usingRawIlParsing :{0} \n {1} \n", new object[] { exception.Message, exception.StackTrace });
			}

			return "";
		}

		private static string GetSourceCode(TypeDefinition typeDefinition)
		{
			try
			{
				var csharpLanguage = new CSharpLanguage();
				var textOutput = new PlainTextOutput();
				var decompilationOptions = new DecompilationOptions();

				decompilationOptions.DecompilerSettings.FullyQualifyAmbiguousTypeNames = true;
				decompilationOptions.FullDecompilation = true;
				decompilationOptions.DecompilerSettings.ShowXmlDocumentation = true;
				
				csharpLanguage.DecompileType(typeDefinition, textOutput, decompilationOptions);

				return textOutput.ToString();
			}
			catch (Exception exception)
			{
				//PublicDI.log.error("in getSourceCode: {0}", new object[] { exception.Message });
				return ("Error in creating source code from Type: " + exception.Message);
			}
		}

		private static string GetSourceCode(MethodDefinition methodDefinition)
		{
			try
			{
				var csharpLanguage = new CSharpLanguage();
				var textOutput = new PlainTextOutput();
				var decompilationOptions = new DecompilationOptions();

				decompilationOptions.DecompilerSettings.FullyQualifyAmbiguousTypeNames = true;
				decompilationOptions.DecompilerSettings.ShowXmlDocumentation = true;
				decompilationOptions.FullDecompilation = true;

				csharpLanguage.DecompileMethod(methodDefinition, textOutput, decompilationOptions);

				return textOutput.ToString();

			}
			catch (Exception exception)
			{
				return ("Error in creating source code from IL: " + exception.Message);
			}
		}

		private static string GetSourceCode(EventDefinition eventDefinition)
		{
			try
			{
				var csharpLanguage = new CSharpLanguage();
				var textOutput = new PlainTextOutput();
				var decompilationOptions = new DecompilationOptions();

				decompilationOptions.DecompilerSettings.FullyQualifyAmbiguousTypeNames = true;
				decompilationOptions.DecompilerSettings.ShowXmlDocumentation = true;
				decompilationOptions.FullDecompilation = true;

				csharpLanguage.DecompileEvent(eventDefinition, textOutput, decompilationOptions);

				return textOutput.ToString();

			}
			catch (Exception exception)
			{
				return ("Error in creating source code from IL: " + exception.Message);
			}
		}

		private static string GetSourceCode(PropertyDefinition propertyDefinition)
		{
			try
			{
				var csharpLanguage = new CSharpLanguage();
				var textOutput = new PlainTextOutput();
				var decompilationOptions = new DecompilationOptions();

				decompilationOptions.DecompilerSettings.FullyQualifyAmbiguousTypeNames = true;
				decompilationOptions.DecompilerSettings.ShowXmlDocumentation = true;
				decompilationOptions.FullDecompilation = true;

				csharpLanguage.DecompileProperty(propertyDefinition, textOutput, decompilationOptions);

				return textOutput.ToString();

			}
			catch (Exception exception)
			{
				return ("Error in creating source code from IL: " + exception.Message);
			}
		}

		#endregion

		#region ReverseEngineer & Tests

		public static void TryMe()
		{
			//DataBase.Current.Database.CreateIfNotExists();
			//DataBase.Current.Database.Initialize(true);

			//Decompiler.ClassNameFilter = new System.Collections.Generic.List<string>() { "OKHOSTING.Code" };
			//Decompiler.ReverseEngineer(typeof(OKHOSTING.Code.Class).Assembly);

			//Templates.Language.ReadFromFiles();

			//LanguageGroup lg = new LanguageGroup();
			//lg.Name = "EntityFramework";
			//LanguageGroupMember lgm = new LanguageGroupMember();
			//lgm.Group = lg;
			//lgm.Language = (from lang in DataBase.Current.Set<OKHOSTING.Code.Generation.Language>() where lang.Name == "CSharp" select lang).FirstOrDefault<Templates.Language>();
			//DataBase.Current.Set<LanguageGroup>().Add(lg);
			//DataBase.Current.Set<LanguageGroupMember>().Add(lgm);
			//DataBase.Current.SaveChanges();

			var groups = DataBase.Current.Set<LanguageGroup>().Include("Languages").Include("Languages.Language").ToList<LanguageGroup>();
			var types = DataBase.Current.Set<Type>().Include("Members").ToList<Type>();

			foreach (var group in groups)
			{
				foreach (var t in types)
				{
					group.RenderToFile(t);
				}
			}

			DataBase.CloseCurrentDataBase();
		}

		public static void ReverseEngineer(System.Reflection.Assembly assembly)
		{
			var _module = assembly.ManifestModule;

			foreach (System.Type _type in DataBase.GetAllPersistentTypes(assembly))
			{
				Type type = ReverseEngineer(_type);
			}
		}

		public static Type ReverseEngineer(System.Type _type)
		{
			var memberFilters = System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance;

			//for byref types, just ignore them and use the element type
			if (_type.IsByRef || _type.IsPointer)
			{
				_type = _type.GetElementType();
			}

			string typeName = _type.GetName();
			Type type = (from t in DataBase.Current.Set<Type>() where t.Name == typeName && t.NameSpace == _type.Namespace select t).SingleOrDefault<Type>();

			if (type != null)
			{
				return type;
			}

			//initialize the right Type's subclass

			//enum
			if (_type.IsEnum)
			{
				type = new Enumeration();
			}
			//interface
			else if (_type.IsInterface)
			{
				type = new Interface();
			}
			//delegate
			else if (_type.IsSubclassOf(typeof(System.Delegate)))
			{
				type = new Delegate();
			}
			//class
			else if (_type.IsClass)
			{
				type = new Class();
			}
			//struct
			else if (_type.IsValueType && !_type.IsEnum)
			{
				type = new Struct();
			}

			type.Name = typeName;
			type.NameSpace = _type.Namespace;
			type.IsArray = _type.IsArray;
			type.IsGenericParameter = _type.IsGenericParameter;
			type.Access = _type.GetAccessModifier();
			type.Module = (from m in DataBase.Current.Set<Module>() where m.Name == _type.Module.Name.Replace(".dll", "") select m).SingleOrDefault<Module>();

			if (type.Module == null)
			{
				type.Module = new Module();
				type.Module.Name = _type.Module.Name.Replace(".dll", "");
				type.Module.Version = _type.Module.Assembly.GetName().Version.ToString();
				DataBase.Current.Set<Module>().Add(type.Module);
			}

			//save now so this doesnt loop on ReverseEngineering other types
			DataBase.Current.Set<Type>().Add(type);
			DataBase.Current.SaveChanges();

			//generics
			if (_type.IsGeneric())
			{
				int position = 0;

				foreach (System.Type _genericArg in _type.GetGenericArguments())
				{
					TypeGenericArgument genericArg = new TypeGenericArgument();

					if (_genericArg.IsGenericParameter)
					{
						genericArg.Position = _genericArg.GenericParameterPosition;
					}
					else
					{
						genericArg.Position = position;
					}

					genericArg.ArgumentType = ReverseEngineer(_genericArg);
					genericArg.AppliedTo = type;
					DataBase.Current.Set<TypeGenericArgument>().Add(genericArg);

					position++;
				}
			}


			DataBase.Current.SaveChanges();

			//return in this point if this class is not included in the namespces list
			bool goDeeper = false;

			foreach (string namesp in ClassNameFilter)
			{
				if (type.FullName.StartsWith(namesp))
				{
					goDeeper = true;
					break;
				}
			}

			if (!goDeeper)
			{
				return type;
			}

			//enum fields
			if (_type.IsEnum)
			{
				//((Enumeration)type).UnderlyingType = ReverseEngineer(Enum.GetUnderlyingType(_type));

				foreach (string _item in Enum.GetNames(_type))
				{
					Field field = new Field();
					field.Type = type;
					field.Name = _item.ToString();
					field.IsLiteral = true;
					field.LiteralValue = Enum.Parse(_type, _item);
					
					DataBase.Current.Set<Field>().Add(field);
				}
			}
			else if (_type.IsClass)
			{
				((Class)type).IsAbstract = _type.IsAbstract;

				if (_type.BaseType != null)
				{
					((Class)type).Parent = (Class)ReverseEngineer(_type.BaseType);
				}

				foreach (System.Type _interface in _type.GetImplementedInterfaces())
				{
					if (((Class)type).ImplementedInterfaces == null)
					{
						((Class)type).ImplementedInterfaces = new System.Collections.Generic.List<Interface>();
					}

					((Class)type).ImplementedInterfaces.ToList<Interface>().Add((Interface)ReverseEngineer(_interface));
				}
			}

			//constructors
			foreach (System.Reflection.ConstructorInfo _constructor in _type.GetConstructors(memberFilters))
			{
				if (_constructor.IsCompilerGenerated())
				{
					continue;
				}

				Method constructor = new Method();

				//decompile method body
				if (!_constructor.IsAbstract)
				{
					Mono.Cecil.AssemblyDefinition ass = Decompiler.LoadAssembly(_constructor.DeclaringType.Module.Assembly.Location);
					MethodDefinition md = ass.MainModule.GetType(_constructor.DeclaringType.FullName).Methods.Where(md2 => md2.Name == _constructor.Name).Select(md2 => md2).First();
					constructor.MethodCode = Decompiler.GetSourceCode(md);
				}

				constructor.Name = type.Name;
				constructor.Type = type;
				constructor.ReturnType = type;
				constructor.IsStatic = _constructor.IsStatic;
				constructor.IsAbstract = _constructor.IsAbstract;
				constructor.IsVirtual = _constructor.IsVirtual;
				constructor.Access = _constructor.GetAccessModifier();
			
				foreach (System.Reflection.ParameterInfo _param in _constructor.GetParameters())
				{
					MethodArgument ma = new MethodArgument();
					ma.Method = constructor;
					ma.Name = _param.Name;
					ma.Type = ReverseEngineer(_param.ParameterType);

					DataBase.Current.Set<MethodArgument>().Add(ma);
				}

				if (_constructor.IsGenericMethod || _constructor.IsGenericMethodDefinition || _constructor.ContainsGenericParameters)
				{
					foreach (System.Type _genericArg in _constructor.GetGenericArguments())
					{
						MethodGenericArgument genericArg = new MethodGenericArgument();

						genericArg.ArgumentType = ReverseEngineer(_genericArg);
						genericArg.Position = _genericArg.GenericParameterPosition;
						genericArg.AppliedTo = constructor;

						DataBase.Current.Set<MethodGenericArgument>().Add(genericArg);
					}
				}

				DataBase.Current.Set<Method>().Add(constructor);

				ReverseEngineerAttributes(constructor, _constructor);
			}

			//fields -- except for enums
			if (!(type is Enumeration))
			{
				foreach (System.Reflection.FieldInfo _field in _type.GetFields(memberFilters))
				{
					if (_field.IsCompilerGenerated())
					{
						continue;
					}

					Field field = new Field();

					field.Name = _field.Name;
					field.IsLiteral = _field.IsLiteral;
					field.IsStatic = _field.IsStatic;
					field.IsInitOnly = _field.IsInitOnly;
					field.Access = _field.GetAccessModifier();
					field.ReturnType = ReverseEngineer(_field.FieldType);
					field.Type = type;
					
					if (field.IsLiteral)
					{
						field.LiteralValue = _field.GetRawConstantValue();
					}

					DataBase.Current.Set<Field>().Add(field);
					
					ReverseEngineerAttributes(field, _field);
				}
			}

			//properties
			foreach (System.Reflection.PropertyInfo _property in _type.GetProperties(memberFilters))
			{
				Property property;

				if (_property.IsCompilerGenerated())
				{
					continue;
				}

				if (_property.PropertyType.Equals(typeof(string)))
				{
					property = new StringProperty();
				}
				else
				{
					property = new Property();
				}

				if (_property.GetMethod != null)
				{
					property.GetMethod = ReverseEngineer(_property.GetMethod);
				}

				if (_property.SetMethod != null)
				{
					property.SetMethod = ReverseEngineer(_property.SetMethod);
				}

				//reverse engineer attributes manually to see if we can deduce if thios property is required or has a string lenght
				foreach (System.Attribute att in _property.GetCustomAttributes(false))
				{
					System.Type attType = att.GetType();

					//hardcoded for "Required" attribute
					if (attType.Name.Contains("Required"))
					{
						property.Required = true;
						continue;
					}

					//hardcoded for "StringLenght" attribute
					if ((attType.Name.Contains("Lenght") || attType.Name.Contains("Size")) && property is StringProperty)
					{
						foreach (System.Reflection.PropertyInfo p in attType.GetProperties())
						{
							if (p.Name.Contains("Length") || p.Name.Contains("Size"))
							{
								try
								{
									((StringProperty)property).MaxLenght = Convert.ToInt32(p.GetValue(att));
								}
								catch { }

								break;
							}
						}

						continue;
					}
				}

				property.Name = _property.Name;
				property.Type = type;
				property.ReturnType = ReverseEngineer(_property.PropertyType);
				property.Access = _property.GetAccessmodifier();

				DataBase.Current.Set<Property>().Add(property);
				
				ReverseEngineerAttributes(property, _property);
			}

			//methods
			foreach (System.Reflection.MethodInfo _method in _type.GetMethods(memberFilters))
			{
				if (_method.IsCompilerGenerated())
				{
					continue;
				}

				//exclude get and set methods since they are decompiled along with their properties
				if (_method.Name.StartsWith("get_") || _method.Name.StartsWith("set_"))
				{
					continue;
				}

				Method method = ReverseEngineer(_method);
			}

			//events
			foreach (System.Reflection.EventInfo _event in _type.GetEvents(memberFilters))
			{
				if (_event.IsCompilerGenerated())
				{
					continue;
				}

				Event even = new Event();
				even.Name = _event.Name;
				even.Type = type;
				even.IsStatic = false; //TODO: HARDCODED
				even.Access = AccessModifier.Public; //TODO: HARDCODED
				even.Delegate = (Delegate) ReverseEngineer(_event.EventHandlerType);
				
				DataBase.Current.Set<Event>().Add(even);
			
				ReverseEngineerAttributes(even, _event);
			}

			//inner types
			foreach (System.Type _nestedType in _type.GetNestedTypes(memberFilters))
			{
				if (_nestedType.IsCompilerGenerated())
				{
					continue;
				}

				Type nestedType = ReverseEngineer(_nestedType);
				nestedType.Access = _nestedType.GetAccessModifier();
				nestedType.Container = (Class)type;
			}

			ReverseEngineerAttributes(type, _type);

			DataBase.Current.SaveChanges();

			return type;
		}

		public static System.Collections.Generic.List<string> ClassNameFilter{ get; set; } 

		
		#endregion

		#region Private

		private static Method ReverseEngineer(System.Reflection.MethodInfo _method)
		{
			Method method = new Method();

			//decompile method body
			if (!_method.IsAbstract && !_method.IsCompilerGenerated())
			{
				Mono.Cecil.AssemblyDefinition ass = Decompiler.LoadAssembly(_method.DeclaringType.Module.Assembly.Location);
				MethodDefinition md = ass.MainModule.GetType(_method.DeclaringType.FullName).Methods.Where(md2 => md2.Name == _method.Name).Select(md2 => md2).First();
				method.MethodCode = Decompiler.GetSourceCode(md);
			}

			method.Name = _method.Name;
			method.Type = ReverseEngineer(_method.DeclaringType);
			method.ReturnType = ReverseEngineer(_method.ReturnType);
			method.IsStatic = _method.IsStatic;
			method.IsAbstract = _method.IsAbstract;
			method.IsVirtual = _method.IsVirtual;
			method.Access = _method.GetAccessModifier();

			DataBase.Current.Set<Method>().Add(method);

			ReverseEngineerAttributes(method, _method);

			foreach (System.Reflection.ParameterInfo _param in _method.GetParameters())
			{
				MethodArgument ma = new MethodArgument();
				ma.Method = method;
				ma.Name = _param.Name;
				ma.Type = ReverseEngineer(_param.ParameterType);

				DataBase.Current.Set<MethodArgument>().Add(ma);
			}

			if (_method.IsGenericMethod || _method.IsGenericMethodDefinition || _method.ContainsGenericParameters)
			{
				foreach (System.Type _genericArg in _method.GetGenericArguments())
				{
					MethodGenericArgument genericArg = new MethodGenericArgument();

					genericArg.ArgumentType = ReverseEngineer(_genericArg);
					genericArg.Position = _genericArg.GenericParameterPosition;
					genericArg.AppliedTo = method;

					DataBase.Current.Set<MethodGenericArgument>().Add(genericArg);
				}
			}

			return method;
		}

		/// <summary>
		/// reverse engineer member attributes, ignore arguments since nthey are too hard to generalize
		/// </summary>
		private static System.Collections.Generic.List<MemberAttribute> ReverseEngineerAttributes(Member member, System.Reflection.MemberInfo _member)
		{
			System.Collections.Generic.List<MemberAttribute> attributes = new System.Collections.Generic.List<MemberAttribute>();

			//reverse engineer member attributes, ignore arguments since nthey are too hard to generalize
			foreach (System.Attribute att in _member.GetCustomAttributes(false))
			{
				MemberAttribute ma = new MemberAttribute();
				ma.AppliedTo = member;
				ma.Attribute = new RunTime.Instance();
				ma.Attribute.Type = ReverseEngineer(att.GetType());

				attributes.Add(ma);
				DataBase.Current.Set<MemberAttribute>().Add(ma);
			}

			return attributes;
		}

		/// <summary>
		/// reverse engineer type attributes, ignore arguments since nthey are too hard to generalize
		/// </summary>
		private static System.Collections.Generic.List<TypeAttribute> ReverseEngineerAttributes(Type type, System.Type _type)
		{
			System.Collections.Generic.List<TypeAttribute> attributes = new System.Collections.Generic.List<TypeAttribute>();

			//reverse engineer member attributes, ignore arguments since nthey are too hard to generalize
			foreach (System.Attribute att in _type.GetCustomAttributes(false))
			{
				TypeAttribute ma = new TypeAttribute();
				ma.AppliedTo = type;
				ma.Attribute = new RunTime.Instance();
				ma.Attribute.Type = ReverseEngineer(att.GetType());

				attributes.Add(ma);
				DataBase.Current.Set<TypeAttribute>().Add(ma);
			}

			return attributes;
		}

		#endregion
	}

	internal static class AccessModifierExtensions
	{
		internal static AccessModifier GetAccessModifier(this System.Reflection.ConstructorInfo constructorInfo)
		{
			if (constructorInfo.IsPrivate)
				return AccessModifier.Private;
			if (constructorInfo.IsFamily)
				return AccessModifier.Protected;
			if (constructorInfo.IsAssembly)
				return AccessModifier.Internal;
			if (constructorInfo.IsPublic)
				return AccessModifier.Public;

			throw new ArgumentException("Did not find access modifier", "constructorInfo");
		}

		internal static AccessModifier GetAccessModifier(this System.Reflection.MethodInfo methodInfo)
		{
			if (methodInfo.IsPrivate)
				return AccessModifier.Private;
			if (methodInfo.IsFamily)
				return AccessModifier.Protected;
			if (methodInfo.IsAssembly)
				return AccessModifier.Internal;
			if (methodInfo.IsPublic)
				return AccessModifier.Public;

			throw new ArgumentException("Did not find access modifier", "methodInfo");
		}

		internal static AccessModifier GetAccessModifier(this System.Reflection.FieldInfo fieldInfo)
		{
			if (fieldInfo.IsPrivate)
				return AccessModifier.Private;
			if (fieldInfo.IsFamily)
				return AccessModifier.Protected;
			if (fieldInfo.IsAssembly)
				return AccessModifier.Internal;
			if (fieldInfo.IsPublic)
				return AccessModifier.Public;

			throw new ArgumentException("Did not find access modifier", "fieldInfo");
		}

		internal static AccessModifier GetAccessModifier(this System.Type type)
		{
			if (type.IsNestedPrivate)
				return AccessModifier.Private;
			if (type.IsNestedFamily)
				return AccessModifier.Protected;
			if (!type.IsVisible)
				return AccessModifier.Internal;
			if (type.IsPublic)
				return AccessModifier.Public;

			throw new ArgumentException("Did not find access modifier", "type");
		}

		internal static AccessModifier GetAccessmodifier(this System.Reflection.PropertyInfo propertyInfo)
		{
			if (propertyInfo.SetMethod != null)
				return propertyInfo.SetMethod.GetAccessModifier();

			if (propertyInfo.GetMethod != null)
				return propertyInfo.GetMethod.GetAccessModifier();

			return AccessModifier.Private;
		}

		internal static string GetName(this System.Type type)
		{
			string name = type.Name;

			if (type.IsGeneric())
			{
				name = name.TrimEnd('`', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0');
				
				name += "<";

				foreach (System.Type _genericArg in type.GetGenericArguments())
				{
					name += _genericArg.FullName + ", ";
				}

				name = name.TrimEnd(',', ' ');
				name += '>';
			}

			return name;
		}

		internal static string GetFullName(this System.Type type)
		{
			return type.Namespace + "." + type.GetName();
		}

		internal static System.Type[] GetImplementedInterfaces(this System.Type type)
		{
			List<System.Type> allInterfaces = new List<System.Type>(type.GetInterfaces());

			if (type.BaseType != null)
			{
				foreach (System.Type i in type.BaseType.GetInterfaces())
				{
					allInterfaces.Remove(i);
				}
			}

			return allInterfaces.ToArray();
		}
	}
}