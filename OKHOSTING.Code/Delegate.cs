using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace OKHOSTING.Code
{
	[Table("Delegate")]
	public class Delegate : Class
	{
		public Method InvokeMethod
		{
			get 
			{
				return 
					(
						from method in base.Methods
						where method.Name == "Invoke"
						select method
					).FirstOrDefault<Method>();
			}
		}

		public Delegate()
		{
		}
	}
}