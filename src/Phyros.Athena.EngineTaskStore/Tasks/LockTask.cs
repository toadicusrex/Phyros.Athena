namespace Phyros.Athena.EngineTaskQueue.Tasks
{
	public class LockTask : IProcessItemEngineTask
	{
		public string ProcessItemId { get; set; }
		public string PrincipalId { get; set; }
		public string ExpectedLastMutationId { get; set; }
	}
}
