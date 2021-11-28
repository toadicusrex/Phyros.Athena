using System.Threading.Tasks;
using Phyros.Athena.Model.Workflow.Processes;

namespace Phyros.Athena.ProcessStore
{
	public interface IWorkflowProcessStore
	{
		Task<WorkflowProcess> GetProcessAsync(string processId);
	}
}
