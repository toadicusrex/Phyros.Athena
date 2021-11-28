using Serilog.Events;

namespace Phyros.Athena.Logging.SerilogLogger
{
	public static class LoggingEventTypeExtensions
	{
		public static LogEventLevel ToLogEventLevel(this LoggingEventType eventType)
		{
			switch (eventType)
			{
				case LoggingEventType.Verbose:
					return LogEventLevel.Verbose;
				case LoggingEventType.Debug:
					return LogEventLevel.Debug;
				case LoggingEventType.Information:
					return LogEventLevel.Information;
				case LoggingEventType.Warning:
					return LogEventLevel.Warning;
				case LoggingEventType.Fatal:
					return LogEventLevel.Fatal;
				default:
					return LogEventLevel.Fatal;
			}
		}
	}
}