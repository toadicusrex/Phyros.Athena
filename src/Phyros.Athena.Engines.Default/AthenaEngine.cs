using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Phyros.Athena.EngineTaskQueue;
using Phyros.Athena.EngineTaskQueue.Tasks;
using Phyros.Athena.EventQueue;
using Phyros.Athena.Logging;
using Phyros.Athena.Model.Workflow.Exceptions;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.Model.Workflow.ProcessItems.Mutations;
using Phyros.Athena.ProcessItemStore;
using Phyros.Athena.ProcessStore;

namespace Phyros.Athena.Engines.Default
{
	public class AthenaEngine : IAthenaEngine
	{
		private readonly IProcessItemEventStore _processItemEventStore;
		private readonly IWorkflowProcessStore _processStore;
		private readonly IWorkflowEngineTaskQueue _taskQueue;
		private readonly IWorkflowEngineEventQueue _eventQueue;
		private readonly ILoggingAdapter _loggingAdapter;
		private readonly Func<string, IEngineTaskHandler> _taskHandlerFactory;

		public AthenaEngine(
			IProcessItemEventStore processItemEventStore,
			IWorkflowProcessStore processStore,
			IWorkflowEngineTaskQueue taskQueue,
			IWorkflowEngineEventQueue eventQueue,
			ILoggingAdapter loggingAdapter,
			Func<string, IEngineTaskHandler> taskHandlerFactory
			)
		{
			_processItemEventStore = processItemEventStore;
			_processStore = processStore;
			_taskQueue = taskQueue;
			_eventQueue = eventQueue;
			_loggingAdapter = loggingAdapter;
			_taskHandlerFactory = taskHandlerFactory;
		}

		public void Start()
		{
			var queueWatcherThread = new Thread(StartQueueWatcher);
			queueWatcherThread.Start();
		}

		private void StartQueueWatcher()
		{
			while (true)
			{
				if (_taskQueue.TryDequeueNext(out var nextTask))
				{
					ThreadPool.QueueUserWorkItem(async state => await InvokeAction(nextTask));
				}
			}
		}

		private async Task InvokeAction(IEngineTask task)
		{
			try
			{
				var taskHandler = _taskHandlerFactory(task.GetType().Name);
				await taskHandler.ProcessActionTask(task);
			}
			catch (Exception e)
			{
				// global exception handler for tasks
				_loggingAdapter.WriteEntry(new LogEntry(LoggingEventType.Error, "We threw an exception! {exception}", new  Dictionary<string, object>()
				{
					{ "exception", e },
				}));	

				_eventQueue.PublishEvent(new EventNotification()
				{
					ProcessId = (task as IProcessEngineTask)?.ProcessId,
					ProcessItemId = (task as IProcessItemEngineTask)?.ProcessItemId,
					ActionId = (task as IActionTask)?.ActionId,
					PrincipalId = task.PrincipalId,
					NotificationKind = EventNotificationKinds.Exception,
					Exception = e
				});
			}
		}

		public void Terminate()
		{
			throw new NotImplementedException();
		}
	}
}
