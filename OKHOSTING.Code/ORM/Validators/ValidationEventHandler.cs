using System;

namespace OKHOSTING.Code.ORM.Validators
{

	/// <summary>
	/// Defines the structure for custom validation events
	/// </summary>
	/// <param name="sender">
	/// Reference to object that send the event
	/// </param>
	/// <param name="e">
	/// Information about the event
	/// </param>
	public delegate void ValidationEventHandler(IValidator sender, ValidationEventArgs e);

}