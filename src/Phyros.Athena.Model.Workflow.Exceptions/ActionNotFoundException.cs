using System;
using System.Collections.Generic;
using System.Text;

namespace Phyros.Athena.Model.Workflow.Exceptions
{
	public class ActionNotFoundException : Exception
	{
		public ActionNotFoundException(string processItemId, string processId, string principalId, string actionId, string currentStepId) : base(
			$"An attempt to perform action '{actionId}' on process item '{processItemId}' at step '{currentStepId}' of process '{processId}' for principalId '{principalId}', but the action could not be located.")

		{
		}
	}
}
