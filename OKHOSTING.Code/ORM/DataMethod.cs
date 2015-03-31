using System;
using System.Reflection;
using System.Xml.Serialization;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Attribute assigned to the methods of a DataObject
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class DataMethod: DataMember
	{
		/// <summary>
		/// Returns the Name of the current MemberInfo
		/// </summary>
		string _name = null;

		/// <summary>
		/// Returns the MethodInfo for the Method
		/// </summary>
		public new MethodInfo InnerMember
		{
			get 
			{
				return (MethodInfo) base.InnerMember;
			}
		}

		/// <summary>
		/// Returns the Name of the current MemberInfo
		/// </summary>
		public override string Name
		{
			get
			{
				if (_name == null)
				{
					//get full method signature
					string signature = InnerMember.ToString();

					//remove return type
					signature = signature.Substring(signature.IndexOf(' '));

					//remove blank spaces
					signature = signature.Replace(" ", "");

					//assign to _name
					_name = signature;
				}
				
				return _name;
			}
		}

		/// <summary>
		/// Indicates if the current DataField is static
		/// </summary>
		public override bool IsStatic
		{
			get
			{
				return InnerMember.IsStatic;
			}
		}

		/// <summary>
		/// Allows you to Invoke the Method
		/// </summary>
		/// <param name="dobj">
		/// DataObject in wich you desire Invoke the Method
		/// </param>
		/// <param name="args">
		/// Array of arguments for the method call
		/// </param>
		/// <returns>
		/// Return value of the invoked method
		/// </returns>
		public object Invoke(DataObject dobj, object[] args)
		{
			return InnerMember.Invoke(dobj, args);
		}

		/// <summary>
		/// Returns whether DataMethod's attibute is declared in a method	
		/// </summary>
		public static bool IsImplementedBy(MethodInfo method)
		{
			return DataMember.IsImplementedBy(method, typeof(DataMethod));
		}

		#region IXmlSerializable Members
		/*

		/// <summary>
		/// Deserialize the current DataMember
		/// </summary>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			DataType dtype;

			reader.MoveToAttribute("DataType");
			dtype = Type.GetType(reader.Value);
			reader.MoveToAttribute("Name");
			innerMember = dtype[reader.Value].InnerMember;

			//read parameters
			reader.ReadToFollowing("Parameters");
			foreach (ParameterInfo p in this.InnerMember.GetParameters())
			{
				writer.WriteStartElement("Parameter");
				writer.WriteAttributeString("Name", p.Name);
				writer.WriteAttributeString("Type", p.ParameterType.AssemblyQualifiedName);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Serialize the current DataMember
		/// </summary>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("DataType", DeclaringDataType.UniqueId);
			writer.WriteAttributeString("Name", Name);
			
			//write parameters
			string parameters = "";
			writer.WriteStartElement("Parameters");
			foreach (ParameterInfo p in this.InnerMember.GetParameters())
			{
				writer.WriteStartElement("Parameter");
				writer.WriteAttributeString("Name", p.Name);
				writer.WriteAttributeString("Type", p.ParameterType.AssemblyQualifiedName);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
		*/
		#endregion
	}
}