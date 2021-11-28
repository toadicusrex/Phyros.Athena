using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Phyros.Athena.Managers;

namespace Phyros.Athena.Controllers
{
	public class WorkListController : Controller
	{
		private readonly IAthenaBpmManager _workflowManager;

		public WorkListController(IAthenaBpmManager workflowManager)
		{
			_workflowManager = workflowManager;
		}

		public IActionResult GetWorkList(string workList)
		{
			var processItems = _workflowManager.GetWorkList(ClaimsPrincipal.Current, workList);
			return new ObjectResult(processItems);
		}

		public IActionResult GetWorkListKinds()
		{
			var workListKinds = _workflowManager.GetWorkListNamesAsync(ClaimsPrincipal.Current);
			return new ObjectResult(workListKinds);
		}
	}
}
