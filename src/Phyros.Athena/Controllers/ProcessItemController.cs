using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Phyros.Athena.ApiModels;
using Phyros.Athena.Managers;

namespace Phyros.Athena.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProcessItemController : ControllerBase
	{
		private readonly IAthenaBpmManager _workflowManager;

		public ProcessItemController(IAthenaBpmManager workflowManager)
		{
			_workflowManager = workflowManager;
		}
		[HttpPost]
		public ActionResult Perform(PerformActionModel action)
		{
			return null;
		}

		[HttpGet]
		public ActionResult Get(string workflowProcessId)
		{
			var processItem = _workflowManager.GetProcessItemAsync(ClaimsPrincipal.Current, workflowProcessId);
			return new ObjectResult(processItem);
		}

		[HttpGet]
		public ActionResult Reassess(string workflowProcessId)
		{
			var processItem = _workflowManager.ReassessProcessItemAsync(ClaimsPrincipal.Current, workflowProcessId);
			return new ObjectResult(processItem);
		}
	}
}
