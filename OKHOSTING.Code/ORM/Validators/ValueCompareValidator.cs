using System;
using OKHOSTING.Code.ORM.Filters;

namespace OKHOSTING.Code.ORM.Validators
{
	/// <summary>
	/// Defines a validation based on the comparison between
	/// an absolute value and a DataValue
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=true)]
	public class ValueCompareValidator: CompareValidator
	{

		#region Fields

		/// <summary>
		/// Value used on the comparison
		/// </summary>
		public readonly IComparable ValueToCompare;

		#endregion

		#region Constructors

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, short valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, int valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, long valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, ushort valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, uint valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, ulong valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, byte valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, sbyte valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, float valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, double valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, decimal valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, DateTime valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, string valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, bool valueToCompare) : this(op, (IComparable)valueToCompare) { }

		/// <summary>
		/// Construct the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, char valueToCompare) : this(op, (IComparable)valueToCompare) { }


		/// <summary>
		/// Constructs the validator
		/// </summary>
		/// <param name="op">
		/// Operator used on the comparison
		/// </param>
		/// <param name="valueToCompare">
		/// Value used on the comparison
		/// </param>
		public ValueCompareValidator(CompareOperator op, IComparable valueToCompare): base(op)
		{
			//Validating if valueToCompare argument is null
			if (valueToCompare == null) throw new ArgumentNullException("valueToCompare");

			//Initializing validator
			this.ValueToCompare = valueToCompare;
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
			//Validating
			return base.Validate(this.ValueToCompare);
		}

		#endregion

	}
}