using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using Phyros.Athena.Engines.Default;
using Phyros.Athena.EventQueue;
using Phyros.Athena.Managers;
using Phyros.Athena.Test.Contexts;
using Phyros.Athena.Test.TestAdapters;
using Polly;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace Phyros.Athena.Test.StepDefinitions
{
	[Binding]
	public class EventNotificationStepDefinitions
	{
		private readonly ContainerContext _containerContext;
		private readonly WorkflowContext _workflowContext;
		private readonly EventContext _eventContext;
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly ManualResetEventSlim _eventListener = new ManualResetEventSlim();
		private readonly Policy _timeoutPolicy;

		public EventNotificationStepDefinitions(ContainerContext containerContext, WorkflowContext workflowContext, EventContext eventContext, ITestOutputHelper testOutputHelper)
		{
			_containerContext = containerContext;
			_workflowContext = workflowContext;
			_eventContext = eventContext;
			_testOutputHelper = testOutputHelper;
			var timeoutPolicy = Policy.Timeout(Debugger.IsAttached ? 300 : 5, Polly.Timeout.TimeoutStrategy.Pessimistic,
				(context, timespan, task) =>
				{
					_testOutputHelper.WriteLine("... waiting for event, timed out.");
				});

			var retryPolicy = Policy.Handle<Exception>()
				.WaitAndRetry(5, i => TimeSpan.FromSeconds(1), (context, timeSpan, task) =>
				{
					_testOutputHelper.WriteLine("... waiting for event, retrying.");
				});

			_timeoutPolicy = Policy.Wrap(timeoutPolicy, retryPolicy);
		}

		[Given(@"an notification subscriber has been configured")]
		[When(@"an notification subscriber has been configured")]
		[Then(@"an notification subscriber has been configured")]
		public void AnEventSubscriberHasBeenConfigured()
		{
			var processItemId = _workflowContext.ProcessItemId.ToString();
			_eventContext.Subscription = _containerContext.GetWorkflowEngineEventQueue()
				.SubscribeToEvents(x => String.Equals(x.ProcessItemId, processItemId))
				.SetupCallback((sender, args) =>
				{
					args.Notification.Should().NotBeNull();
					_eventContext.LastNotification = args.Notification;
					_eventListener.Set();
				});
		}


		[Given(@"a process item notification has been received")]
		[When(@"a process item notification has been received")]
		[Then(@"a process item notification has been received")]
		public void AMatchingEventShouldHaveBeenAddedToTheEventStack()
		{
			_timeoutPolicy.Execute(WaitForMessage);
		}


		[Given(@"an event should have been added to the event stack")]
		[When(@"an event should have been added to the event stack")]
		[Then(@"an event should have been added to the event stack")]
		public void AnEventShouldHaveBeenAddedToTheEventStack()
		{
			_timeoutPolicy.Execute(WaitForMessage);
			_workflowContext.ProcessItemId.Should().NotBeNull();
		}


		[Then(@"an exception of type '(.*)' should be thrown")]
		public void ThenAnExceptionOfTypeShouldBeThrown(string exceptionType)
		{
			var message = _timeoutPolicy.Execute(WaitForMessage);
			//while (message == null && message.NotificationKind != EventNotificationKinds.Exception)
			//{
			//	message = _timeoutPolicy.Execute(WaitForMessage);
			//}

			if (message.Exception != null && !Equals(message.Exception?.GetType().Name, exceptionType))
			{
				_testOutputHelper.WriteLine(message.Exception.ToString());
			}
			message.Exception.GetType().Name.Should().BeEquivalentTo(exceptionType);

			//_scenarioContext.Pending();
		}

		private EventNotification WaitForMessage()
		{
			var eventQueue = (TestWorkflowEngineEventQueue)_containerContext.GetWorkflowEngineEventQueue();
			eventQueue.ReleaseOne(_workflowContext.ProcessItemId);
			_eventListener.Wait();
			_eventListener.Reset();
			return _eventContext.LastNotification;
		}
	}
}
