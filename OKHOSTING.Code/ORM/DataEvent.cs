using System;
using System.Reflection;

namespace OKHOSTING.Code.ORM
{

	/// <summary>
	/// Represents a class event on a DataObject
	/// </summary>
	[Serializable]
	[AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
	public class DataEvent: DataMember
	{

		/// <summary>
		/// Returns the EventInfo that this instance is representing
		/// </summary>
		public new EventInfo InnerMember
		{
			get
			{
				return (EventInfo)base.InnerMember;
			}
		}

		/// <summary>
		/// Indicates if the current DataField is static
		/// </summary>
		public override bool IsStatic
		{
			get
			{
				return InnerMember.GetRaiseMethod().IsStatic;
			}
		}

		/// <summary>
		/// Add an Event Handler to the event
		/// </summary>
		/// <param name="dobj">
		/// DataObject in wich you desire add the handler
		/// </param>
		/// <param name="handler">
		/// Procedure that handles the event
		/// </param>
		public void AddHandler(DataObject dobj, Delegate handler)
		{
			InnerMember.AddEventHandler(dobj, handler);
		}

		/// <summary>
		/// Remove an Event Handler from the event
		/// </summary>
		/// <param name="dobj">
		/// DataObject in wich you desire remove the handler
		/// </param>
		/// <param name="handler">
		/// Procedure that currently handles the event
		/// </param>
		public void RemoveHandler(DataObject dobj, Delegate handler)
		{
			InnerMember.RemoveEventHandler(dobj, handler);
		}
		
		/// <summary>
		/// Returns whether DataEvent attibute is declared in a class event	
		/// </summary>
		public static bool IsImplementedBy(EventInfo eventInfo)
		{
			return DataMember.IsImplementedBy(eventInfo, typeof(DataEvent));
		}
	}
}