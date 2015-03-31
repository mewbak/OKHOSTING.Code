using System;
using System.Data;
using System.Xml.Serialization;
using OKHOSTING.Tools;
using OKHOSTING.Tools.Extensions;
using OKHOSTING.Code.ORM.Sql.Generators;
using OKHOSTING.Code.ORM.Sql;
using System.Collections.Generic;
using System.Reflection;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// Configuration class for the OKHOSTING.Code.ORM library
	/// </summary>
	[Serializable]
	public class Configuration : OKHOSTING.Tools.ConfigurationBase, IXmlSerializable
	{
		/// <summary>
		/// The DataBase that will be used as DataBase.Current every time a new session
		/// is started
		/// </summary>
		public DataBase CurrentDataBase;

		/// <summary>
		/// List of installed plugins
		/// </summary>
		public List<PlugIn> PlugIns = new List<PlugIn>();

		/// <summary>
		/// Defines the way in wich the DataType tables will be named in the DataBase.
		/// This format is used 
		/// </summary>
		public TableNameFormat TableNameFormat = TableNameFormat.DataTypeName;

		/// <summary>
		/// Current configuration
		/// </summary>
		public static Configuration Current;

		/// <summary>
		/// Loads the current configuration
		/// </summary>
		static Configuration()
		{
			Current = (Configuration)OKHOSTING.Tools.ConfigurationBase.Load(typeof(Configuration));
		}

		#region IXmlSerializable Members

		/// <summary>
		/// Following online help recommendations, this method allways return null
		/// </summary>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Deserialize the Configuration
		/// </summary>
		void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			//Local Vars
			Type type;

			//deserialize TableNameFormat
			reader.ReadToFollowing("TableNameFormat");
			TableNameFormat = (TableNameFormat)Enum.Parse(typeof(TableNameFormat), reader.ReadElementContentAsString());

			//deserialize CurrentDataBase
			reader.MoveToAttribute("Type");
			type = Type.GetType(reader.Value, true);
			CurrentDataBase = (DataBase) type.CreateInstance();
			CurrentDataBase.ReadXml(reader);

			//deserialize plugins
			reader.ReadToFollowing("PlugIns");
			while (reader.ReadToFollowing("PlugIn"))
			{
				PlugIn p = new PlugIn();
				
				reader.MoveToAttribute("Type");
				p.Type = Type.GetType(reader.Value, true);

				reader.MoveToAttribute("Enabled");
				p.Enabled = bool.Parse(reader.Value);

				PlugIns.Add(p);
			}
		}

		/// <summary>
		/// Serialize the Configuration
		/// </summary>
		void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			//if CurrentDataBase is null, create a MySQl database
			//with a connection string, as a default database
			if (CurrentDataBase == null)
			{
				CurrentDataBase = new MySqlDataBase("server=localhost;database=mydatabase;user id=myuser;pwd=mypassword;Persist Security Info=False;");
			}

			//serialize TableNameFormat
			writer.WriteElementString("TableNameFormat", TableNameFormat.ToString());

			//serialize CurrentDataBase
			writer.WriteStartElement("CurrentDataBase");
			writer.WriteAttributeString("Type", CurrentDataBase.GetType().AssemblyQualifiedName);
			CurrentDataBase.WriteXml(writer);
			writer.WriteEndElement();

			//serialize PlugIns
			writer.WriteStartElement("PlugIns");
			foreach (PlugIn p in PlugIns)
			{
				writer.WriteStartElement("PlugIn");
				writer.WriteAttributeString("Type", p.Type.AssemblyQualifiedName);
				writer.WriteAttributeString("Enabled", p.Enabled.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		#endregion

		/// <summary>
		/// A plug in is a regular class that defines two static methods:
		/// public static void PlugIn_OnSessionStart();
		/// public static void PlugIn_OnSessionEnd();
		/// This methods are called when a session is atarted or ended, and allows
		/// the plugin to subscribe to DataBase events or perform start-up tasks
		/// that extends the application behaviour
		/// </summary>
		public class PlugIn
		{
			/// <summary>
			/// The class that defines PlugIn_OnSessionStart and PlugIn_OnSessionEnd as static methods
			/// </summary>
			public Type Type;

			/// <summary>
			/// If set to true, this plug in is called when a session starts or ends.
			/// If set to false, this plugin is ignored
			/// </summary>
			public bool Enabled;

			/// <summary>
			/// Name of the static method that will be invoked for every enabled plug in when a new session starts
			/// </summary>
			public const string OnSessionStartMethodName = "PlugIn_OnSessionStart";

			/// <summary>
			/// Name of the static method that will be invoked for every enabled plug in when a session ends
			/// </summary>
			public const string OnSessionEndMethodName = "PlugIn_OnSessionEnd";

			/// <summary>
			/// Invokes the OnSessionStart method for the current plugin
			/// </summary>
			/// <remarks>
			/// This method should only be invoked ONCE at session start
			/// </remarks>
			public void InvokeOnSessionStartMethod()
			{
				InvokePlugInMethod(OnSessionStartMethodName);
			}

			/// <summary>
			/// Invokes the OnSessionEnd method for the current plugin
			/// </summary>
			/// <remarks>
			/// This method should only be invoked ONCE at session end
			/// </remarks>
			public void InvokeOnSessionEndMethod()
			{
				InvokePlugInMethod(OnSessionEndMethodName);
			}

			/// <summary>
			/// Invokes a static method in the PlugIn Type, for session start or session end events
			/// </summary>
			/// <param name="methodName">Name of the method to be invoked</param>
			private void InvokePlugInMethod(string methodName)
			{
				MethodInfo method;

				method = this.Type.GetMethod(methodName);
				if (method == null) return;
				if (!method.IsStatic) return;

				method.Invoke(null, null);
			}
		}
	}
}