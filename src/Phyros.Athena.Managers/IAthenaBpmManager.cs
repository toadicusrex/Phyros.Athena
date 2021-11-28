using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Phyros.Athena.Engines.Model;
using Phyros.Athena.Model.Workflow.ProcessItems;
using ProcessItemState = Phyros.Athena.Engines.Model.ProcessItemState;

namespace Phyros.Athena.Managers
{
	public interface IAthenaBpmManager
	{
		Task<ProcessItemState> GetProcessItemAsync(ClaimsPrincipal principal, string workflowProcessId);
		Task<IEnumerable<ProcessItemState>> GetWorkList(ClaimsPrincipal principal, string workList);
		Task<IEnumerable<string>> GetWorkListNamesAsync(ClaimsPrincipal principal);

		Task ReassessProcessItemAsync(ClaimsPrincipal principal, string workflowProcessId);
		Task<string> QueueStartAction(string processId, string actionId);
		Task QueueAction(string processItemId, string expectedProcessItemStateId, string processId, string actionId, Dictionary<string, object> properties);
		Task QueueProcessItemLock(string processItemId, string processItemStateId, string principalId);
	}
}
