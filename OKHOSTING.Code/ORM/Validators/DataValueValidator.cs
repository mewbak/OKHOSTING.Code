using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Validates a specific DataValue. CHild classes will be able to perform a validation on a specific DataValue
	/// </summary>
	public abstract class DataValueValidator: Attribute, IValidator
	{
		#region Fields & Properties

		/// <summary>
		/// DataValue that implements the DataValueValidator
		/// </summary>
		private DataValue dataValue;

		/// <summary>
		/// DataObject instance for do the validation
		/// </summary>
		private DataObject dataObject;

		/// <summary>
		/// DataValue that implements the DataValueValidator
		/// </summary>
		public DataValue DataValue
		{ 
			get 
			{ 
				return dataValue; 
			} 
		}

		/// <summary>
		/// DataObject instance for do the validation
		/// </summary>
		public DataObject DataObject
		{ 
			get 
			{ 
				return dataObject; 
			} 
		}

		#endregion 

		#region Abstract Methods

		/// <summary>
		/// Performs the validation
		/// </summary>
		/// <returns>
		/// ValidationError object with the error founded if the validation fails,
		/// otherwise returns null
		/// </returns>
		public abstract ValidationError Validate();

		#endregion 

		#region Validator Implementation

		/// <summary>
		/// Returns a boolean value that indicates if the specified 
		/// DataValueValidator is implemented in the specified DataValue
		/// </summary>
		/// <param name="dataValueValidatorType">
		/// Type of DataValueValidator to verify
		/// </param>
		/// <param name="dataValue">
		/// DataValue in which will be searched the specified DataValueValidator
		/// </param>
		/// <returns>
		/// Boolean value that indicates if the specified DataValueValidator 
		/// is implemented in the specified DataValue
		/// </returns>
		public static bool IsImplemented(Type dataValueValidatorType, DataValue dataValue)
		{
			//Validating if the dataValue argument is null
			if (dataValue == null) throw new ArgumentNullException("dataValue");

			//Loading the attributes of the DataValue
			object[] array = LoadAttributesFromValue(dataValue, dataValueValidatorType);

			//Validating if the DataValueValidator was 
			//reached and Returning the value
			return (array.Length > 0);
		}

		/// <summary>
		/// Returns all the DataValueValidator's implemented 
		/// for the specified DataValue. The validators returned for
		/// this overload can't be used for real validation, only 
		/// for informative purpose because don't specify the DataObject
		/// instance corresponding to the validation
		/// </summary>
		/// <param name="dataValue">
		/// DataValue whose DataValueValidator's object was returned
		/// </param>
		/// <returns>
		/// Array of DataValueValidator's implmented 
		/// for the specified DataValue
		/// </returns>
		internal static DataValueValidator[] GetValidators(DataValue dataValue)
		{return GetValidators(dataValue, null); }

		/// <summary>
		/// Returns all the DataValueValidator's implemented 
		/// for the specified DataValue
		/// </summary>
		/// <param name="dataValue">
		/// DataValue whose DataValueValidator's object was returned
		/// </param>
		/// <param name="dataObject">
		/// dataObject instance that will be used to validate this validator (null if dont
		/// is required the validation)
		/// </param>
		/// <returns>
		/// Array of DataValueValidator's implmented 
		/// for the specified DataValue
		/// </returns>
		public static DataValueValidator[] GetValidators(DataValue dataValue, DataObject dataObject)
		{
			//Validating if the dataValue argument is null
			if (dataValue == null) throw new ArgumentNullException("dataValue");

			//Loading the DataValueValidator's of DataValue
			object[] array = LoadAttributesFromValue(dataValue, typeof(DataValueValidator));

			//Creating the array of validators to return
			DataValueValidator[] validators = new DataValueValidator[array.Length];
			
			//Crossing the array of attributes loaded and copying to array for return 
			for(int i= 0; i < array.Length; i++)
			{
				validators[i] = (DataValueValidator)array[i];
				validators[i].dataValue = dataValue;
				validators[i].dataObject = dataObject;
			}

			//Returning the validators array
			return validators;
		}

		/// <summary>
		/// Returns all the DataValueValidator's implemented 
		/// for all the DataValue's contained on dataValues collection.
		/// The validators returned for this overload can't be used for 
		/// real validation, only for informative purpose because don't 
		/// specify the DataObjectinstance corresponding to the validation
		/// </summary>
		/// <param name="dataValues">
		/// Collection of DataValue objects for loading the DataValueValidator's
		/// </param>
		/// <returns>
		/// Array with all the DataValueValidator's implemented 
		/// for all the DataValue's contained on dataValues collection
		/// </returns>
		public static DataValueValidator[] GetValidators(List<DataValue> dataValues)
		{ return GetValidators(dataValues, null); }

		/// <summary>
		/// Returns all the DataValueValidator's implemented 
		/// for all the DataValue's contained on dataValues collection
		/// </summary>
		/// <param name="dataValues">
		/// Collection of DataValue objects for loading the DataValueValidator's
		/// </param>
		/// <returns>
		/// Array with all the DataValueValidator's implemented 
		/// for all the DataValue's contained on dataValues collection
		/// </returns>
		public static DataValueValidator[] GetValidators(List<DataValue> dataValues, DataObject dataObject)
		{
			//Validating if dataValues argument is null
			if (dataValues == null) throw new ArgumentNullException("dataValues");

			//Loading all the validators for all the DataValues
			ArrayList array= new ArrayList();
			foreach(DataValue dv in dataValues)
			{
				array.AddRange(GetValidators(dv, dataObject));
			}
			
			//Creating array of DataValueValidator's
			DataValueValidator[] validators = new DataValueValidator[array.Count];
			array.CopyTo(validators);

			//Returning the validators
			return validators;
		}

		#endregion
		
		#region Internal Utilities

		/// <summary>
		/// Load the attributes of the specified Type on the indicated DataValue 
		/// </summary>
		/// <param name="dataValue">
		/// DataValue whose attributes will be loaded
		/// </param>
		/// <param name="attributeType">
		/// Type of the searched attribute
		/// </param>
		/// <returns>
		/// Array of attributes of the specified Type on the indicated DataValue 
		/// </returns>
		private static object[] LoadAttributesFromValue(DataValue dataValue, Type attributeType)
		{
			//Local Vars
			object[] array;

			//Validating if the member is a field
			if (dataValue.InnerMember.MemberType == MemberTypes.Field)
			{
				array = ((FieldInfo)dataValue.InnerMember).GetCustomAttributes(attributeType, false);
			}
			else
			{
				array = ((PropertyInfo)dataValue.InnerMember).GetCustomAttributes(attributeType, false);
			}

			//Returning the array of attributes
			return array;
		}

		/// <summary>
		/// Returns the current value of the the DataValue associated
		/// </summary>
		/// <returns>
		/// Current value of the the DataValue associated
		/// </returns>
		protected object GetCurrentValue()
		{
			//Local Vars 
			object currentValue = null;

			//Validating if the DataObject to validate was specified
			if (this.dataObject == null)
				throw new InvalidOperationException("Current validater DataObject property can't be null");

			//Getting the value
			currentValue = this.dataObject.GetValue(this.dataValue);

			//Returning current value
			return currentValue;
		}

		#endregion

	}
}