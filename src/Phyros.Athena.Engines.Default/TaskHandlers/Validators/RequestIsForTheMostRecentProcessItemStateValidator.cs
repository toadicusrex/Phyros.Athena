using System;
using System.Collections.Generic;
using System.Text;
using Phyros.Athena.EngineTaskQueue.Tasks;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.Model.Workflow.ProcessItems.MutationHandlers;

namespace Phyros.Athena.Engines.Default.TaskHandlers.Validators
{
	public class RequestIsForTheMostRecentProcessItemStateValidator : IEngineTaskValidator
	{
		public void Validate(IEngineTask task, IProcessItem processItem)
		{
			if (!(task is IActionTask actionTask))
			{
				// don't validate a non-action task
				return;
			}
			if (!actionTask.ExpectedLastMutationId.Equals(processItem.ItemState?.ProcessItemMutationId))
			{
				throw new ProcessItemStateChangedException(processItem.ProcessItemId, actionTask.ExpectedLastMutationId, processItem.ItemState.ProcessItemMutationId);
			}
		}
	}
}
