using Phyros.Athena.EngineTaskQueue.Tasks;

namespace Phyros.Athena.EngineTaskQueue
{
	public interface IWorkflowEngineTaskQueue
	{
		void Enqueue(IEngineTask task);
		bool TryDequeueNext(out IEngineTask task);
	}
}
