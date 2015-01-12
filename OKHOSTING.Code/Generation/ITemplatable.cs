using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.Code.Generation
{
	public interface ITemplatable
	{
		string NameResult
		{
			get;
		}

		string ContentResult
		{
			get;
		}

		string FilePathResult
		{
			get;
		}

		Language ActiveLanguage
		{
			get; set;
		}
	}
}