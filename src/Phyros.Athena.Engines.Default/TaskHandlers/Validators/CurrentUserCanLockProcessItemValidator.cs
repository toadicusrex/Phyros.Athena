using System;
using System.Collections.Generic;
using System.Text;
using Phyros.Athena.EngineTaskQueue.Tasks;
using Phyros.Athena.Model.Workflow.Exceptions;
using Phyros.Athena.Model.Workflow.ProcessItems;

namespace Phyros.Athena.Engines.Default.TaskHandlers.Validators
{
	public class CurrentUserCanLockProcessItemValidator : IEngineTaskValidator
	{
		public void Validate(IEngineTask task, IProcessItem processItem)
		{
			if (processItem.Lock == null || processItem.Lock.LockExpirationUtc < DateTime.UtcNow) return;
			if (!Equals(processItem.Lock.LockedToPrincipalId, task.PrincipalId))
			{
				throw new ProcessItemLockedException(processItem.ProcessItemId, task.PrincipalId, processItem.Lock.LockExpirationUtc);
			}
		}
	}
}
