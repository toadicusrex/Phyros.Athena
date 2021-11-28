using System;

namespace Phyros.Athena.Model.Workflow.ProcessItems.MutationHandlers
{
	public class ProcessItemStateChangedException : Exception
	{
		public ProcessItemStateChangedException(string processItemId, string requestedProcessItemMutationId, string currentProcessItemMutationId ) : 
			base($"An attempt to modify process item with ProcessItemId '{ processItemId }' failed because the request expected the item to have a last mutation id of '{requestedProcessItemMutationId}', but the item's last mutation id is '{currentProcessItemMutationId}'")
		{
		}
	}
}