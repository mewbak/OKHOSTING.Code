using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Softosis.Model.Security
{
	public class Role
	{
		[Key]
		public int Id { get; set; }

		[System.ComponentModel.DataAnnotations.Required]
		[System.ComponentModel.DataAnnotations.StringLength(100)]
		public string Name { get; set; }
	}
}