using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OKHOSTING.Code.ORM.Replication
{
	public class SynchronizationLog
	{
		public ulong Id;

		public DateTime Date;

		public RemoteDataBase RemoteDataBase;

		public DataObject DataObject;

		public List<DataValue> DataValues;
	}
}
