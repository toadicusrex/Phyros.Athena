using System;
using System.Threading.Tasks;
using Phyros.Athena.Engines.Default.TaskHandlers.Validators;
using Phyros.Athena.EngineTaskQueue;
using Phyros.Athena.EngineTaskQueue.Tasks;
using Phyros.Athena.EventQueue;
using Phyros.Athena.Logging;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.Model.Workflow.ProcessItems.MutationHandlers;
using Phyros.Athena.Model.Workflow.ProcessItems.Mutations;
using Phyros.Athena.ProcessItemStore;

namespace Phyros.Athena.Engines.Default.TaskHandlers
{
	public class LockTaskHandler : IEngineTaskHandler
	{
		private readonly IProcessItemEventStore _processItemEventStore;
		private readonly IWorkflowEngineEventQueue _eventQueue;
		private readonly ILoggingAdapter _loggingAdapter;
		private readonly Func<string, IEngineTaskValidator> _taskValidatorFactory;

		public LockTaskHandler(
			IProcessItemEventStore processItemEventStore,
			IWorkflowEngineEventQueue eventQueue,
			ILoggingAdapter loggingAdapter,
			Func<string, IEngineTaskValidator> taskValidatorFactory
		)
		{
			_processItemEventStore = processItemEventStore;
			_eventQueue = eventQueue;
			_loggingAdapter = loggingAdapter;
			_taskValidatorFactory = taskValidatorFactory;
			_loggingAdapter = loggingAdapter;
		}

		public async Task ProcessActionTask(IEngineTask task)
		{
			var lockTask = task as LockTask;
			var processItem = await _processItemEventStore.GetProcessItemAsync(lockTask.ProcessItemId);
			
			_taskValidatorFactory(EngineTaskValidatorKinds.CurrentUserCanLockProcessItem).Validate(task, processItem);
			_taskValidatorFactory(EngineTaskValidatorKinds.RequestIsForTheMostRecentState).Validate(task, processItem);
			
			processItem.ApplyMutation(new LockProcessItemMutation()
			{
				LockDuration = TimeSpan.FromMinutes(5),
				PrincipalId = task.PrincipalId
			});
			_processItemEventStore.StoreUnsavedMutations((IProcessItemEventContainer)processItem,
				(processItemMutationId, mutationDateTimeUtc) =>
				{
					processItem.SetProcessItemState(processItemMutationId, mutationDateTimeUtc);
				});

			_eventQueue.PublishEvent(new EventNotification()
			{
				ProcessItemId = lockTask.ProcessItemId,
				NotificationKind = EventNotificationKinds.ProcessItemLock
			});
		}
	}
}
