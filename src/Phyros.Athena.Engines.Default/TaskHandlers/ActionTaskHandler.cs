using System;
using System.Linq;
using System.Threading.Tasks;
using Phyros.Athena.Engines.Default.TaskHandlers.Validators;
using Phyros.Athena.EngineTaskQueue;
using Phyros.Athena.EngineTaskQueue.Tasks;
using Phyros.Athena.EventQueue;
using Phyros.Athena.Logging;
using Phyros.Athena.Model.Workflow.Exceptions;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.Model.Workflow.ProcessItems.MutationHandlers;
using Phyros.Athena.Model.Workflow.ProcessItems.Mutations;
using Phyros.Athena.ProcessItemStore;
using Phyros.Athena.ProcessStore;

namespace Phyros.Athena.Engines.Default.TaskHandlers
{
	public class ActionTaskHandler : IEngineTaskHandler
	{
		private readonly IProcessItemEventStore _processItemEventStore;
		private readonly IWorkflowProcessStore _processStore;
		private readonly IWorkflowEngineEventQueue _eventQueue;
		private readonly ILoggingAdapter _loggingAdapter;
		private readonly Func<string, IEngineTaskValidator> _taskValidatorFactory;

		public ActionTaskHandler(
			IProcessItemEventStore processItemEventStore,
			IWorkflowProcessStore processStore,
			IWorkflowEngineEventQueue eventQueue,
			ILoggingAdapter loggingAdapter,
			Func<string, IEngineTaskValidator> taskValidatorFactory
		)
		{
			_processItemEventStore = processItemEventStore;
			_processStore = processStore;
			_eventQueue = eventQueue;
			_loggingAdapter = loggingAdapter;
			_taskValidatorFactory = taskValidatorFactory;
			_loggingAdapter = loggingAdapter;
		}
		public async Task ProcessActionTask(IEngineTask task)
		{
			var actionTask = task as ActionTask;
			var process = await _processStore.GetProcessAsync(actionTask.ProcessId);
			var processItem = await _processItemEventStore.GetProcessItemAsync(actionTask.ProcessItemId);

			if (processItem == null)
			{
				var action = process.StartActions[actionTask.ActionId];
				if (action == null)
				{
					throw new Exception("Start action not found, and process item is null.");
				}

				processItem = new ProcessItem();
				processItem.ApplyMutation(new CreationActionProcessItemMutation()
				{
					ProcessItemId = actionTask.ProcessItemId,
					ActionId = actionTask.ActionId,
					TargetStepId = action.ArrivalStep,
					PropertyChanges = actionTask.PropertyChanges
				});
				_processItemEventStore.StoreUnsavedMutations((IProcessItemEventContainer) processItem,
					(processItemMutationId, mutationDateTimeUtc) =>
					{
						processItem.SetProcessItemState(processItemMutationId, mutationDateTimeUtc);
					});
			}
			else
			{
				_taskValidatorFactory(EngineTaskValidatorKinds.CurrentUserCanLockProcessItem).Validate(task, processItem);
				_taskValidatorFactory(EngineTaskValidatorKinds.RequestIsForTheMostRecentState).Validate(task, processItem);

				if (process.Steps.TryGetValue(processItem.CurrentStepId, out var step))
				{
					if (step.Actions.TryGetValue(actionTask.ActionId, out var action))
					{
						processItem.ApplyMutation(new ActionProcessItemMutation()
						{
							ActionId = actionTask.ActionId,
							TargetStepId = action.ArrivalStep,
							PropertyChanges = actionTask.PropertyChanges
						});
						_processItemEventStore.StoreUnsavedMutations((IProcessItemEventContainer)processItem,
							(processItemMutationId, mutationDateTimeUtc) =>
							{
								processItem.SetProcessItemState(processItemMutationId, mutationDateTimeUtc);
							});
					}
					else
					{
						throw new ActionNotFoundException(actionTask.ProcessItemId, actionTask.ProcessId, actionTask.PrincipalId, processItem.CurrentStepId, actionTask.ActionId);
					}
				}
				else
				{
					throw new Exception("Step not found.");
				}
			}
			_eventQueue.PublishEvent(new EventNotification()
			{
				ProcessId = actionTask.ProcessId,
				ProcessItemId = actionTask.ProcessItemId,
				ActionId = actionTask.ActionId,
				NotificationKind = EventNotificationKinds.Action
			});
		}

		
	}
}
