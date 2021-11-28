using System.Collections.Generic;
using System.Threading.Tasks;
using Phyros.Athena.Model.Workflow.Processes;
using Phyros.Athena.ProcessStore;
using Phyros.Athena.Test.TestAdapters.TestProcess;

namespace Phyros.Athena.Test.TestAdapters
{
	public class TestWorkflowProcessStore : IWorkflowProcessStore
	{
		public Task<WorkflowProcess> GetProcessAsync(string processId)
		{
			return Task.FromResult(processId switch
			{
				TestProcessIds.SimplestProcess => TestProcesses.SimplestProcess,
				TestProcessIds.ProcessWithSingleStep => TestProcesses.ProcessWithSingleStep,
				_ => throw new KeyNotFoundException(@"Unable to locate test process with id '{processId}'.")
			});
		}
	}
}
