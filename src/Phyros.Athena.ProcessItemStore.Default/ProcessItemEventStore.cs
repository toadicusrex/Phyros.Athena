using System;
using System.Threading;
using System.Threading.Tasks;
using Phyros.Athena.Model.Workflow.ProcessItems;

namespace Phyros.Athena.ProcessItemStore.Default
{
	public class ProcessItemEventStore : IProcessItemEventStore
	{
		public Task<IProcessItem> GetProcessItemAsync(string processItemId)
		{
			throw new NotImplementedException();
		}

		public void StoreUnsavedMutations(IProcessItemEventContainer processItem, MutationStoredCallback callback)
		{
			throw new NotImplementedException();
		}
	}
}
