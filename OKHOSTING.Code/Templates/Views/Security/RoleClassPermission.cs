﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Softosis.Model.Security
{
	public class RoleClassPermission
	{
		[Key]
		public int Id { get; set; }

		[System.ComponentModel.DataAnnotations.Required]
		public Role Role { get; set; }
		
		[System.ComponentModel.DataAnnotations.Required]
		public Class Class { get; set; }

		public bool Write { get; set; }
		public bool Delete { get; set; }

		public string Criteria { get; set; }
	}
}