using System;

namespace OKHOSTING.Code.ORM.Filters
{
	
	/// <summary>
	/// Argument for custom filter event
	/// </summary>
	public class FilterEventArgs: EventArgs
	{

		#region Fields

		/// <summary>
		/// Indicates if the filter was successfully approved
		/// </summary>
		public bool Match = false;

		/// <summary>
		/// DataObject in wich the filter must be applied
		/// </summary>
		public readonly DataObject DataObjectForFilter;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the argument
		/// </summary>
		/// <param name="dataObjectForFilter">
		/// DataObject in wich the filter must be applied
		/// </param>
		public FilterEventArgs(DataObject dataObjectForFilter)
		{
			this.DataObjectForFilter = dataObjectForFilter;
			this.Match = false;
		}

		#endregion 

	}
}