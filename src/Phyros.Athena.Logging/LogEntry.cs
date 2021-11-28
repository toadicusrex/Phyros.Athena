using System;
using System.Collections.Generic;

namespace Phyros.Athena.Logging
{
	public class LogEntry 
	{
		public LoggingEventType Severity { get; }
		public string MessageTemplate { get; }
		public Exception Exception { get; }
		public Dictionary<string, object> Properties { get; }

		public LogEntry(LoggingEventType severity, string messageTemplate, Dictionary<string, object> properties = null, Exception exception = null)
		{
			if (messageTemplate == null) throw new ArgumentNullException(nameof(messageTemplate));
			if (messageTemplate == string.Empty) throw new ArgumentException("Message Template cannot be empty.", nameof(messageTemplate));

			Severity = severity;
			MessageTemplate = messageTemplate;
			Exception = exception;
			Properties = properties ?? new Dictionary<string, object>();
		}

		
	}

	public enum LoggingEventType
	{
		Verbose,
		Debug,
		Information,
		Warning,
		Error,
		Fatal
	}
}