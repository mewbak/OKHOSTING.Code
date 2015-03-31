using System;
using System.IO;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Validates that a string DataValue contains a valid path to a directory.
	/// Path can absolute or relative to the "/Custom" directory.
	/// </summary>
	/// <remarks>Applies only to string DataValues</remarks>
	/// <example>
	/// c:\myfolder\ --> absolute path
	/// /myfolder/ --> relative path (starting at /Custom directory)
	/// </example>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class DirectoryValidator : DataValueValidator
	{
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
			string path;
			
			//Getting the value of the DataValue
			string currentValue = (string)this.GetCurrentValue();

			//if null, exit
			if (string.IsNullOrWhiteSpace(currentValue)) return null;

			//absolute path
			if (currentValue.Contains(":"))
			{
				path = currentValue;
			}
			//relative path
			else
			{
				path = AppDomain.CurrentDomain.BaseDirectory + @"Custom\" + currentValue.TrimStart('/', '\\');
			}

			//validate file path
			if (!Directory.Exists(path))
			{
				error = new ValidationError(this, "Directory '" + path + "' does not exists");
			}

			//Returning the error or null
			return error;
		}

		/// <summary>
		/// Determines whether DirectoryValidator attibute is defined in a DataValue
		/// </summary>
		public static bool IsImplemented(DataValue dvalue)
		{
			//Validating if the dvalue is null
			if (dvalue == null) throw new ArgumentNullException("dvalue");

			return DataValueValidator.IsDefined(dvalue.InnerMember, typeof(DirectoryValidator));
		}
	}
}