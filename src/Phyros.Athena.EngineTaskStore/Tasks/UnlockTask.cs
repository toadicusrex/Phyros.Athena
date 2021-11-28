namespace Phyros.Athena.EngineTaskQueue.Tasks
{
	public class UnlockTask : IProcessItemEngineTask
	{
		public string ProcessItemId { get; set; }
		public string ProcessItemStateId { get; set; }
		public string PrincipalId { get; set; }
		public string ExpectedLastMutationId { get; set; }
	}
}
