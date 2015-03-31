using System;
using System.Web;
using System.Diagnostics;
using System.Threading;
using OKHOSTING.Code.ORM;
using OKHOSTING.Code.ORM.Filters;
using OKHOSTING.Code.ORM.Validators;
using OKHOSTING.Tools;
using System.Collections.Generic;
using OKHOSTING.Code.ORM.Sql.Generators;

namespace OKHOSTING.Code.ORM
{
	/// <summary>
	/// A DataBase that uses tables, columns and rows for storing DataObjects, like MySQL, Access or a Xml DataSet
	/// </summary>
	public abstract class RelationalDataBase: DataBase
	{
		/// <summary>
		/// Create the Database table for the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for Table Creation
		/// </param>
		/// <param name="inherits">
		/// Indicates if do you want to create too, the parent DataTypes
		/// of the dtype argument
		/// </param>
		public abstract void Create(DataType dtype, bool inherits);

		/// <summary>
		/// Drop the Database table for the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for Table Creation
		/// </param>
		/// <param name="inherits">
		/// Indicates if do you want to drop too, the parent DataTypes
		/// of the dtype argument
		/// </param>
		public abstract void Drop(DataType dtype, bool inherits);

		/// <summary>
		/// Create the indexes for the table of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for index creation
		/// </param>
		/// <param name="inherits">
		/// Indicates if do you want to create too, the parent DataTypes 
		/// indexes of the dtype argument
		/// </param>
		public abstract void CreateIndexes(DataType dtype, bool inherits);

		/// <summary>
		/// Create the foreign keys for the table of the specified DataType
		/// </summary>
		/// <param name="dtype">
		/// DataType for foreign keys creation
		/// </param>
		/// <param name="inherits">
		/// Indicates if do you want to create too, the parent DataTypes
		/// foreign keys of the dtype argument
		/// </param>
		public abstract void CreateForeignKeys(DataType dtype, bool inherits);

		/// <summary>
		/// Returns a boolean value that determines if the Table
		/// associated to the specified DataType exists
		/// </summary>
		/// <param name="dtype">
		/// DataType for table validation
		/// </param>
		/// <returns>
		/// true if the database table for the specified DataType exists,
		/// otherwise false
		/// </returns>
		public abstract bool TableExists(DataType dtype);

		/// <summary>
		/// Returns a value that indicates if the Constrainst 
		/// exists in the Underlying Database 
		/// </summary>
		/// <param name="Name">
		/// Name of the constraint to validate
		/// </param>
		/// <returns>
		/// Value that indicates if the Constrainst 
		/// exists in the Underlying Database 
		/// </returns>
		public abstract bool ConstraintExists(string Name);

		/// <summary>
		/// Verify if exists the specified index on the Database
		/// </summary>
		/// <param name="Name">
		/// Name of the index
		/// </param>
		/// <returns>
		/// Boolean value that indicates if exists 
		/// the specified index on the Database
		/// </returns>
		public abstract bool IndexExists(string Name);

		/// <summary>
		/// Performs initial tasks so the current DataBase can perform operations on these DataTypes
		/// </summary>
		/// <param name="dtypes">List of DataTypes that will be supported by the current DataBase</param>
		/// <remarks>This method should only be executed on system setup</remarks>
		public override void Setup(List<DataType> dtypes)
		{
			foreach (DataType dtype in dtypes)
			{
				//if table is already created, drop it
				if (TableExists(dtype))
				{
					Drop(dtype, false);
				}

				//re-create table
				Create(dtype, false);
			}

			//Inserting initial data
			foreach (DataType dtype in dtypes)
			{
				DataObjectList initial = dtype.GetSetupDataObjects();
				if (initial != null) Insert(initial);
			}

			//create indexes
			foreach (DataType dtype in dtypes)
			{
				CreateIndexes(dtype, false);
			}

			//create foreign keys
			foreach (DataType dtype in dtypes)
			{
				CreateForeignKeys(dtype, false);
			}
		}

		/// <summary>
		/// Verifies that all DataTypes are crreclty setup in the Database
		/// by performing a simple select operation on each one. 
		/// If errors are found, a list of exceptions is returned
		/// </summary>
		/// <param name="dtypes">List of DataTypes that will be verified by the current DataBase</param>
		/// <returns>A list of exceptions found in the current setup, if any</returns>
		/// <remarks>This method should only be executed after a system setup or system update</remarks>
		public override List<DataTypeNotSupportedException> VerifySetup(List<DataType> dtypes)
		{
			List<DataTypeNotSupportedException> errors = new List<DataTypeNotSupportedException>();

			foreach (DataType dtype in dtypes)
			{
				//verify table exists
				if (!TableExists(dtype))
				{
					errors.Add(new DataTypeNotSupportedException(dtype, "Table does not exist for DataType '" + dtype.UniqueId + "'"));
				}
			}

			//add errors found by base class
			errors.AddRange(base.VerifySetup(dtypes));

			return errors;
		}
	}
}