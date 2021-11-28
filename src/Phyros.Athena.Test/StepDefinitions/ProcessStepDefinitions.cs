using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Phyros.Athena.EventQueue;
using Phyros.Athena.Managers;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.ProcessItemStore;
using Phyros.Athena.ProcessStore;
using Phyros.Athena.Test.Contexts;
using Phyros.Athena.Test.TestAdapters.Logging;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace Phyros.Athena.Test.StepDefinitions
{
	[Binding]
	public class ProcessStepDefinitions
	{
		private readonly ContainerContext _containerContext;
		private readonly WorkflowContext _workflowContext;
		private readonly ManualResetEventSlim _eventListener = new ManualResetEventSlim();

		public ProcessStepDefinitions(ContainerContext containerContext, WorkflowContext workflowContext, ITestOutputHelper testOutputHelper)
		{
			Thread.SetData(Thread.GetNamedDataSlot(TestOutputLoggingAdapter.LoggingAdapterThreadDataName), testOutputHelper);
			_containerContext = containerContext;
			_workflowContext = workflowContext;
		}
		[Given(@"process '(.*)'")]
		public async Task GivenProcess(string processId)
		{
			var processRepository = _containerContext.GetWorkflowProcessStore();
			_workflowContext.Process = await processRepository.GetProcessAsync(processId);
			_workflowContext.Should().NotBeNull();
		}
            
		[Given(@"start action '(.*)' is triggered")]
		[When(@"start action '(.*)' is triggered")]
		[Then(@"start action '(.*)' is triggered")]
		public async Task WhenStartActionIsTriggered(string actionName)
		{
			var bpmManager = _containerContext.GetAthenaBpmManager();
			_workflowContext.ProcessItemId = await bpmManager.QueueStartAction(_workflowContext.Process.Name, actionName );
			_workflowContext.ActionName = actionName;
		}
            
		[Given(@"followup action '(.*)' is triggered")]
		[When(@"followup action '(.*)' is triggered")]
		[Then(@"followup action '(.*)' is triggered")]
		public async Task WhenFollowupActionIsTriggered(string followupActionName)
		{
			var bpmManager = _containerContext.GetAthenaBpmManager();
			var item = await LoadProcessItemFromProcessItemStore();
			await bpmManager.QueueAction(_workflowContext.ProcessItemId, item.ItemState.ProcessItemMutationId, _workflowContext.Process.Name, followupActionName, null);
			_workflowContext.ActionName = followupActionName;
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