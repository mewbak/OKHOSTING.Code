using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM.Replication
{
	public class SynchronizationRule: DataObject
	{
		public DataType DataTypeSSS;

		public List<DataValue> DataValues;
	}
}
