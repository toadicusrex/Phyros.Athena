using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Phyros.Athena.Managers;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.ProcessItemStore;
using Phyros.Athena.Test.Contexts;
using TechTalk.SpecFlow;

namespace Phyros.Athena.Test.StepDefinitions
{
	[Binding]
	public class ProcessItemStepDefinitions
	{
		private readonly ContainerContext _containerContext;
		private readonly WorkflowContext _workflowContext;

		public ProcessItemStepDefinitions(ContainerContext containerContext, WorkflowContext workflowContext)
		{
			_containerContext = containerContext;
			_workflowContext = workflowContext;
		}

		[Given(@"the process item should be recoverable from the ProcessItemStore and should be at step '(.*)'")]
		[When(@"the process item should be recoverable from the ProcessItemStore and should be at step '(.*)'")]
		[Then(@"the process item should be recoverable from the ProcessItemStore and should be at step '(.*)'")]
		public async Task ThenTheProcessItemShouldBeRecoverableFromTheProcessItemStoreAndShouldBeAtStep(string destinationStepId)
		{
			var item = await LoadProcessItemFromProcessItemStore();
			if (string.IsNullOrWhiteSpace(destinationStepId))
			{
				item.CurrentStepId.Should().BeNull();
			}
			else
			{
				item.CurrentStepId.Should().BeEquivalentTo(destinationStepId);
			}
		}

		[Given(@"the processItemId should not be non-null and non-empty")]
		[When(@"the processItemId should not be non-null and non-empty")]
		[Then(@"the processItemId should not be non-null and non-empty")]
		public void ThenProcessItemShouldBeNon_Null()
		{
			_workflowContext.ProcessItemId.Should().NotBeNull();
			_workflowContext.ProcessItemId.Should().NotBeEmpty();
		}

		[Given(@"a lock has been requested with principalId '(.*)'")]
		[When(@"a lock has been requested with principalId '(.*)'")]
		[Then(@"a lock has been requested with principalId '(.*)'")]
		public async Task WhenALockHasBeenRequested(string userId)
		{
			var bpmManager = _containerContext.GetAthenaBpmManager();
			var item = await LoadProcessItemFromProcessItemStore();
			await bpmManager.QueueProcessItemLock(_workflowContext.ProcessItemId, item?.ItemState?.ProcessItemMutationId, userId);
		}

		private async Task<IProcessItem> LoadProcessItemFromProcessItemStore()
		{
			var processItemStore = _containerContext.GetProcessItemEventStore();
			var found = await processItemStore.GetProcessItemAsync(_workflowContext.ProcessItemId);
			var count = 0;
			while (found == null && count < 5)
			{
				Thread.Sleep(100);
				found = await processItemStore.GetProcessItemAsync(_workflowContext.ProcessItemId);
				count++;
			}
			return found;
		}
	}
}
