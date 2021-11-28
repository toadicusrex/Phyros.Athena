using Phyros.Athena.EventQueue;
using Phyros.Athena.Model.Workflow.Processes;
using Phyros.Athena.Model.Workflow.ProcessItems;

namespace Phyros.Athena.Test.Contexts
{
	public class WorkflowContext
	{
		public WorkflowProcess Process { get; set; }
		public string ProcessItemId { get; set; }
		public string ActionName { get; set; }
	}
}