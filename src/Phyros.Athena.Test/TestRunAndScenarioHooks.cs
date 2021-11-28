using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Phyros.Athena.Composition;
using Phyros.Athena.EngineTaskQueue;
using Phyros.Athena.EventQueue;
using Phyros.Athena.Logging;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.ProcessItemStore;
using Phyros.Athena.ProcessStore;
using Phyros.Athena.Test.Contexts;
using Phyros.Athena.Test.TestAdapters;
using Phyros.Athena.Test.TestAdapters.Logging;
using Serilog;
using SimpleInjector;
using TechTalk.SpecFlow;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Phyros.Athena.Test
{
	[Binding]
	public class TestRunAndScenarioHooks
	{
		private readonly ContainerContext _containerContext;
		private readonly WorkflowContext _workflowContext;
		private readonly ITestOutputHelper _testOutputHelper;
		private static Container _container;

		[BeforeTestRun]
		public static void Initialize()
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			ApplicationComposer.Container.Options.AllowOverridingRegistrations = true;
			ApplicationComposer.Container.Register<IWorkflowProcessStore, TestWorkflowProcessStore>();
			ApplicationComposer.Container.RegisterSingleton<IProcessItemEventStore, TestWorkflowProcessItemEventStore>();
			ApplicationComposer.Container.RegisterSingleton<IWorkflowEngineTaskQueue, TestWorkflowEngineTaskQueue>();
			ApplicationComposer.Container.RegisterSingleton<IWorkflowEngineEventQueue, TestWorkflowEngineEventQueue>();

			ApplicationComposer.Compose();
			ApplicationComposer.Container.RegisterSingleton<ILoggingAdapter, TestOutputLoggingAdapter>();
			ApplicationComposer.Container.RegisterSingleton<EventLogContainer>();
			var engineManager = ApplicationComposer.GetEngineManager();
			engineManager.Start();

			_container = ApplicationComposer.Container;
		}

		[AfterTestRun]
		public static void Cleanup()
		{
			Log.CloseAndFlush();
		}

		public TestRunAndScenarioHooks(ContainerContext containerContext, WorkflowContext workflowContext, ITestOutputHelper testOutputHelper)
		{
			_containerContext = containerContext;
			_workflowContext = workflowContext;
			_testOutputHelper = testOutputHelper;
		}

		[BeforeScenario]
		public void SetUpContainerContext()
		{
			_containerContext.Container = _container;
		}

		[AfterScenario]
		public void WriteLoggedEventsToOutputHelper()
		{
			var eventLogContainer = _containerContext.GetEventLogContainer();
			var events = eventLogContainer.GetEventsForProcessItemId(_workflowContext.ProcessItemId);
			_testOutputHelper.WriteLine(String.Empty);
			_testOutputHelper.WriteLine(String.Empty);
			_testOutputHelper.WriteLine("LoggedEvents:");
			foreach (var loggedEvent in events)
			{
				_testOutputHelper.WriteLine(JsonConvert.SerializeObject(loggedEvent));
			}
			
		}

		[AfterStep]
		public async Task WriteCurrentProcessItemState(ContainerContext containerContext, WorkflowContext workflowContext, ITestOutputHelper testOutputHelper)
		{
			var item = await LoadProcessItemFromProcessItemStore();
			if (item != null)
			{
				_testOutputHelper.WriteLine(Environment.NewLine);
				_testOutputHelper.WriteLine(JsonConvert.SerializeObject(item, Formatting.Indented));
				_testOutputHelper.WriteLine(Environment.NewLine);
			}
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
