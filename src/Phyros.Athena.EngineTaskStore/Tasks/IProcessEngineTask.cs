namespace Phyros.Athena.EngineTaskQueue.Tasks
{
	public interface IProcessEngineTask : IEngineTask
	{
		string ProcessId { get; set; }
	}
}