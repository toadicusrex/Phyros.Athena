namespace Phyros.Athena.EngineTaskQueue.Tasks
{
	public interface IProcessItemEngineTask : IEngineTask
	{
		string ExpectedLastMutationId { get; set; }
		string ProcessItemId { get; set; }
	}
}