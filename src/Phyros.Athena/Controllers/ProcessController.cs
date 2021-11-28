using Microsoft.AspNetCore.Mvc;
using Phyros.Athena.Managers;

namespace Phyros.Athena.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProcessController : ControllerBase
	{
		private IAthenaBpmManager _workflowManager;

		//public ActionResult UpdateProcess(string uniqueProcessName, IWorkflowProcess processModel)
		//{
		//	var updateResult = _workflowManager.UpdateProcess(ClaimsPrincipal.Current, uniqueProcessName, processModel);
		//	return new ObjectResult(updateResult);
		//}
		//public ActionResult
	}
}
