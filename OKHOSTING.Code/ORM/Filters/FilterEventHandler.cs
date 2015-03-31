using System;

namespace OKHOSTING.Code.ORM.Filters
{

	/// <summary>
	/// Delegate for custom filter event
	/// </summary>
	/// <param name="sender">
	/// Object that throws the event
	/// </param>
	/// <param name="e">
	/// Event information argument
	/// </param>
	public delegate void FilterEventHandler(IFilter sender, FilterEventArgs e);

}