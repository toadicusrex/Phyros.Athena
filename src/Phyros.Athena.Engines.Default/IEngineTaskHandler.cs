using System.Threading.Tasks;
using Phyros.Athena.EngineTaskQueue;
using Phyros.Athena.EngineTaskQueue.Tasks;

namespace Phyros.Athena.Engines.Default
{
	public interface IEngineTaskHandler
	{
		Task ProcessActionTask(IEngineTask task);
	}
}
