using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Softosis.Model.Security
{
	public class RoleModulePermission
	{
		[Key]
		public int Id { get; set; }

		[System.ComponentModel.DataAnnotations.Required]
		public Role Role { get; set; }

		[System.ComponentModel.DataAnnotations.Required]
		public Module Module { get; set; }

		public bool Write { get; set; }
	}
}