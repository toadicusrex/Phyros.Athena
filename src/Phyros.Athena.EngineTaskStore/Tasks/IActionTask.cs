namespace Phyros.Athena.EngineTaskQueue.Tasks
{
	public interface IActionTask : IEngineTask, IProcessItemEngineTask
	{
		string ActionId { get; set; }
	}
}