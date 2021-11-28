using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Phyros.Athena.Logging;
using Phyros.Athena.Model.Workflow.ProcessItems;
using Phyros.Athena.Model.Workflow.ProcessItems.Mutations;
using Phyros.Athena.ProcessItemStore;

namespace Phyros.Athena.Test.TestAdapters
{
	public class TestWorkflowProcessItemEventStore : IProcessItemEventStore
	{
		private readonly ILoggingAdapter _loggingAdapter;

		private readonly List<ProcessItemEvent> _eventStreams = new List<ProcessItemEvent>();
		private readonly ReaderWriterLockSlim _eventStreamsLock = new ReaderWriterLockSlim();

		public TestWorkflowProcessItemEventStore(ILoggingAdapter loggingAdapter)
		{
			_loggingAdapter = loggingAdapter;
		}

		public Task<IProcessItem> GetProcessItemAsync(string processItemId)
		{
			_eventStreamsLock.EnterReadLock();
			var events = _eventStreams
																											.Where(x => x.ProcessItemId == processItemId)
																											.OrderBy(x => x.MutationTimestampUtc)
																											.ToList();
			_eventStreamsLock.ExitReadLock();
			if (!events.Any())
			{
				return Task.FromResult<IProcessItem>(null);
			}

			var processItem = new ProcessItem();
			foreach (var e in events)
			{
				processItem.ReplayMutation(new CompletedProcessItemMutation(e.MutationId, e.MutationTimestampUtc, e.Mutation));
			}

			return Task.FromResult<IProcessItem>(processItem);
		}

		public void StoreUnsavedMutations(IProcessItemEventContainer processItem, MutationStoredCallback callback)
		{
			string lastProcessItemMutationId = null;
			var lastMutationDateTimeUtc = DateTime.MinValue;
			_loggingAdapter.WriteEntry(new LogEntry(LoggingEventType.Information, "Storing mutations: {mutations}",
				new Dictionary<string, object>()
				{
					{"mutations", JsonConvert.SerializeObject(processItem.UnsavedMutations)}
				}));
			while (processItem.UnsavedMutations.Any())
			{
				_eventStreamsLock.EnterWriteLock();
				_eventStreams.Add(new ProcessItemEvent()
				{
					MutationId = lastProcessItemMutationId = Guid.NewGuid().ToString(),
					MutationTimestampUtc = lastMutationDateTimeUtc = DateTime.UtcNow,
					ProcessItemId = processItem.ProcessItemId,
					Mutation = processItem.UnsavedMutations.Dequeue(),
				});
				_eventStreamsLock.ExitWriteLock();
			}

			callback?.Invoke(lastProcessItemMutationId, lastMutationDateTimeUtc);
			
		}
	}

	public class CompletedProcessItemMutation : ICompletedProcessItemMutation
	{
		public CompletedProcessItemMutation(string mutationId, DateTime mutationTimestampUtc, IProcessItemMutation mutation)
		{
			StoredMutation = mutation;
			ProcessItemMutationId = mutationId;
			ProcessItemMutationTimestampUtc = mutationTimestampUtc;
		}

		public IProcessItemMutation StoredMutation { get; set; }
		public string ProcessItemMutationId { get; set; }
		public DateTime ProcessItemMutationTimestampUtc { get; set; }
	}

	public class ProcessItemEvent
	{
		public string ProcessItemId {get;set;}
		public IProcessItemMutation Mutation {get;set;}
		public DateTime MutationTimestampUtc {get;set;}
		public string MutationId { get; set; }
	}
}
