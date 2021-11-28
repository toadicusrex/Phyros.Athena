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
	public class UnlockTaskHandler : IEngineTaskHandler
	{
		private readonly IProcessItemEventStore _processItemEventStore;
		private readonly IWorkflowEngineEventQueue _eventQueue;
		private readonly ILoggingAdapter _loggingAdapter;
		private readonly Func<string, IEngineTaskValidator> _taskValidatorFactory;

		public UnlockTaskHandler(
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
			var unlockTask = task as UnlockTask;
			var processItem = await _processItemEventStore.GetProcessItemAsync(unlockTask.ProcessItemId);
			
			_taskValidatorFactory(EngineTaskValidatorKinds.CurrentUserCanLockProcessItem).Validate(task, processItem);
			_taskValidatorFactory(EngineTaskValidatorKinds.RequestIsForTheMostRecentState).Validate(task, processItem);
			
			if (String.IsNullOrWhiteSpace(processItem.Lock?.LockedToPrincipalId) || processItem.Lock?.LockExpirationUtc < DateTime.UtcNow)
			{
				// the current user doesn't have a valid lock.  There's no reason to apply any mutation.
				return;
			}

			processItem.ApplyMutation(new UnlockProcessItemMutation(){});
			_processItemEventStore.StoreUnsavedMutations((IProcessItemEventContainer)processItem,
				(processItemMutationId, mutationDateTimeUtc) =>
				{
					processItem.SetProcessItemState(processItemMutationId, mutationDateTimeUtc);
				});

			_eventQueue.PublishEvent(new EventNotification()
			{
				ProcessItemId = unlockTask.ProcessItemId,
				NotificationKind = EventNotificationKinds.ProcessItemUnlock
			});
		}
	}
}
