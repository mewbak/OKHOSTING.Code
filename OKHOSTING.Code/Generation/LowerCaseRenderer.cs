using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code.Generation
{
	public class LowerCaseRenderer: Antlr4.StringTemplate.IAttributeRenderer
	{
		public string ToString(object obj, string formatString, System.Globalization.CultureInfo culture)
		{
			return obj.ToString().ToLower();
		}
	}
}