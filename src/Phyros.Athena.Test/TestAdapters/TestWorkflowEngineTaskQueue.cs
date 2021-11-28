using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Phyros.Athena.EngineTaskQueue;
using Phyros.Athena.EngineTaskQueue.Tasks;
using Phyros.Athena.Logging;

namespace Phyros.Athena.Test.TestAdapters
{
	public class TestWorkflowEngineTaskQueue : IWorkflowEngineTaskQueue
	{
		private readonly ILoggingAdapter _loggingAdapter;

		public TestWorkflowEngineTaskQueue(ILoggingAdapter loggingAdapter)
		{
			_loggingAdapter = loggingAdapter;
		}
		private static readonly ConcurrentQueue<IEngineTask> Queue = new ConcurrentQueue<IEngineTask>();

		public void Enqueue(IEngineTask task)
		{
			_loggingAdapter.WriteEntry(new LogEntry(LoggingEventType.Information, "Enqueuing task: {task}",
				new Dictionary<string, object>()
				{
					{"task", JsonConvert.SerializeObject(task)}
				}));
			Queue.Enqueue(task);

		}

		public bool TryDequeueNext(out IEngineTask task)
		{
			var success = Queue.TryDequeue(out task);
			if (success)
			{
				_loggingAdapter.WriteEntry(new LogEntry(LoggingEventType.Information, "Dequeuing task: {task}",
					new Dictionary<string, object>()
					{
						{"task", JsonConvert.SerializeObject(task)}
					}));
			}
			//else
			//{
			//	_loggingAdapter.WriteEntry(
			//		new LogEntry(LoggingEventType.Information, "Attempt to dequeue task found no tasks."));
			//}
			return success;
		}
	}
}
