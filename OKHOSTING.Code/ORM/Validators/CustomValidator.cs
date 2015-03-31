using System;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Allows to perform custom validations over DataObjects data
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=true)]
	public class CustomValidator: DataValueValidator
	{

		#region Fields

		/// <summary>
		/// Error message to be showed when the validator fails
		/// </summary>
		public readonly string ErrorMessage;

		#endregion

		#region Events

		/// <summary>
		/// Event throwed on the validation
		/// </summary>
		public event ValidationEventHandler Validating;

		#endregion

		#region Constructors

		/// <summary>
		/// Construct the validator 
		/// </summary>
		/// <param name="errorMessage">
		/// Error message to be showed when the validator fails
		/// </param>
		/// <param name="validatingEventHandler">
		/// Delegate that points to the event that perform the validation
		/// </param>
		public CustomValidator(string errorMessage, ValidationEventHandler validatingEventHandler)
		{
			//Validating if the errorMessage argument is null
			if (ErrorMessage == null) throw new ArgumentNullException("errorMessage");

			//Initializing the error message
			this.ErrorMessage = errorMessage;
		}

		#endregion

		#region Validation Implementation

		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		public override ValidationError Validate()
		{
			//Local Vars
			ValidationError error = null;

			//Validating if exists subscriptors to Validating event
			if (this.Validating != null)
			{
				//Creating the argument for the event
				ValidationEventArgs e = new ValidationEventArgs(this.ErrorMessage, this.GetCurrentValue());

				//Requesting validation to the client
				this.Validating(this, e);

				//If the validation fails, creating the respective error
				if (!e.IsValid) error = new ValidationError(this, e.ErrorMessage);
			}

			//Returning the applicable error or null
			return error;
		}

		#endregion
	
	}
}