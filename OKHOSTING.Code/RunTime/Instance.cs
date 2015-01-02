using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code.RunTime
{
	/// <summary>
	/// Represents an actual instance of a type, a created object with assigned values
	/// </summary>
	[System.ComponentModel.DefaultProperty("Name")]
	public class Instance 
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id
		{
			get;
			set;
		}
		
		/// <summary>
		/// The type that defines this object
		/// </summary>
		public Type Type
		{
			get; set;
		}

		public System.Guid? MemberId { get; set; }
		public System.Guid? TypeId { get; set; }

		public string Name
		{
			get; set;
		}

		public Instance()
		{
		}
	}
}