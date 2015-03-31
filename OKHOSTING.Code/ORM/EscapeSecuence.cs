using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM
{

	/// <summary>
	/// Represents a string escape secuence
	/// </summary>
	public enum EscapeSecuence
	{
		BackSpace = '\b',
		NewLine = '\n',
		CarriageReturn = '\r',
		HorizontalTab = '	',
		VerticalTab = '\v',
		SingleQuotationMark = '\'',
		DoubleQuotationMark = '\"',
		BackSlash = '\\'
	}
}
