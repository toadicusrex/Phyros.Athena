using System;
using System.Collections.Generic;
using System.Text;

namespace Phyros.Athena.Model.Workflow.Exceptions
{
	public class ProcessItemLockedException : Exception
	{
		public ProcessItemLockedException(string processItemId, string userId, DateTime lockExpiration) : base(
			$"An attempt to perform an action on process item '{processItemId}' for userId '{userId}' has failed due to an existing item lock.  The lock will expire at '{lockExpiration.ToLongDateString()}' (UTC).")

		{
		}
	}
}
