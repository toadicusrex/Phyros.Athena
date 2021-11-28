using Phyros.Athena.EngineTaskQueue.Tasks;
using Phyros.Athena.Model.Workflow.ProcessItems;

namespace Phyros.Athena.Engines.Default.TaskHandlers.Validators
{
	public interface IEngineTaskValidator
	{
		void Validate(IEngineTask task, IProcessItem processItem);
	}
}