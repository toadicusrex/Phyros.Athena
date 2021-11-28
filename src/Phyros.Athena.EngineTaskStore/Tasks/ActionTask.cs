using System.Collections.Generic;
using Phyros.Athena.Model.Workflow.ProcessItems;

namespace Phyros.Athena.EngineTaskQueue.Tasks
{
	public class ActionTask : IProcessItemEngineTask, IProcessEngineTask, IActionTask
	{
		public bool IsStartAction { get; set; }
		public string ProcessId { get; set; }
		public string ProcessItemId { get; set; }
		public string ActionId { get; set; }
		public IDictionary<string, ProcessItemPropertyChange> PropertyChanges { get; set; }

		public string ExpectedLastMutationId { get; set; }
		public string PrincipalId { get; set; }
	}
}
