using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Phyros.Athena.Engines.Model;
using Phyros.Athena.EngineTaskQueue;
using Phyros.Athena.EngineTaskQueue.Tasks;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.ProcessStore;
using ProcessItemState = Phyros.Athena.Engines.Model.ProcessItemState;

namespace Phyros.Athena.Managers.Default
{
	public class AthenaBpmManager : IAthenaBpmManager
	{
		private readonly IWorkflowEngineTaskQueue _taskQueue;
		private readonly IWorkflowProcessStore _processStore;

		public AthenaBpmManager(IWorkflowEngineTaskQueue taskQueue, IWorkflowProcessStore processStore)
		{
			_taskQueue = taskQueue;
			_processStore = processStore;
		}
		public Task<ProcessItemState> GetProcessItemAsync(ClaimsPrincipal principal, string workflowProcessId)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<ProcessItemState>> GetWorkList(ClaimsPrincipal principal, string workList)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<string>> GetWorkListNamesAsync(ClaimsPrincipal principal)
		{
			throw new NotImplementedException();
		}

		public Task ReassessProcessItemAsync(ClaimsPrincipal principal, string workflowProcessId)
		{
			throw new NotImplementedException();
		}

		public Task<string> QueueStartAction(string processId, string actionId)
		{
			var process = _processStore.GetProcessAsync(processId);
			var newId = Guid.NewGuid().ToString();
			_taskQueue.Enqueue(new ActionTask()
			{
				ProcessId = processId,
				ProcessItemId = newId,
				ActionId = actionId
			});
			return Task.FromResult<string>(newId);
		}

		public Task QueueAction(string processItemId, string expectedProcessItemStateId, string processId, string actionId, Dictionary<string, object> properties)
		{
			var process = _processStore.GetProcessAsync(processId);
			_taskQueue.Enqueue(new ActionTask()
			{
				ProcessItemId = processItemId,
				ExpectedLastMutationId = expectedProcessItemStateId,
				ProcessId = processId,
				ActionId = actionId
			});
			return Task.CompletedTask;
		}

		public Task QueueProcessItemLock(string processItemId, string processItemStateId, string principalId)
		{
			_taskQueue.Enqueue(new LockTask()
			{
				ProcessItemId = processItemId,
				ExpectedLastMutationId = processItemStateId,
				PrincipalId = principalId
			});
			return Task.CompletedTask;
		}
	}
}
