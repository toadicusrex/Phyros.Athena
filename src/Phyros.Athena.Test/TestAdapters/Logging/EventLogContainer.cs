using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phyros.Athena.Logging;

namespace Phyros.Athena.Test.TestAdapters.Logging
{
	public class EventLogContainer
	{
		private readonly List<EventLogWrapper> _eventLogWrappers = new List<EventLogWrapper>();
		private readonly object _eventLogLock = new object();

		public void Add(LogEntry logEntry)
		{
			lock (_eventLogLock)
			{
				_eventLogWrappers.Add(new EventLogWrapper()
				{
					Entry = logEntry,
					Timestamp = DateTime.Now,
					ProcessItemId = (string) logEntry.Properties["processItemId"]
				});
			}
		}

		public IEnumerable<EventLogWrapper> GetEventsForProcessItemId(string processItemId)
		{
			lock (_eventLogLock)
			{
				return _eventLogWrappers.Where(x => x.ProcessItemId.Equals(processItemId)).ToArray();
			}
		}
	}

	public class EventLogWrapper
	{
		public LogEntry Entry { get; set; }
		public DateTime Timestamp { get; set; }
		public string ProcessItemId { get; set; }
	}
}
