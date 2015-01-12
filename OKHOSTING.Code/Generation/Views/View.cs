using OKHOSTING.Softosis.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Softosis.Views
{
	public class View
	{
		public string Name { get; set; }

		public string FullName
		{
			get
			{
				return string.Format("{0}.{1}", Container, Name);
			}
		}

		public string ContainerId{ get; set; }
		public View Container { get; set; }

		public System.Collections.Generic.List<View> SubViews { get; set; }
	}
}