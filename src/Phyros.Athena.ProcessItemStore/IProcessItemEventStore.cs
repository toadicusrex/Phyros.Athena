using System;
using System.Threading.Tasks;
using Phyros.Athena.Model.Workflow.ProcessItems;

namespace Phyros.Athena.ProcessItemStore
{
	public interface IProcessItemEventStore
	{
		Task<IProcessItem> GetProcessItemAsync(string processItemId);
		void StoreUnsavedMutations(IProcessItemEventContainer processItem, MutationStoredCallback callback);
	}

	public delegate void MutationStoredCallback(string processItemMutationId, DateTime mutationDateTimeUtc);
}
