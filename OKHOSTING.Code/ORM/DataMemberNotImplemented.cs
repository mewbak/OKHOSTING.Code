using System;
using System.Reflection;

namespace OKHOSTING.Code.ORM
{

	/// <summary>
	/// Exception that throws when you try to load information about 
	/// a member not defined as part of a DataObject
	/// </summary>
	public class DataMemberNotImplemented: Exception
	{
		
		/// <summary>
		/// MemberInfo not implemented
		/// </summary>
		public readonly MemberInfo MemberInfo;

		/// <summary>
		/// Constructs the exception
		/// </summary>
		/// <param name="MemberInfo">
		/// MemberInfo not implemented
		/// </param>
		/// <param name="message">
		/// Error message
		/// </param>
		public DataMemberNotImplemented(MemberInfo memberInfo, string message): base(message)
		{
			this.MemberInfo = memberInfo;
		}

	}

}