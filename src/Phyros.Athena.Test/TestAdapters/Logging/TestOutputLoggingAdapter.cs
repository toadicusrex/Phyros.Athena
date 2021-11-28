using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Phyros.Athena.Logging;
using Phyros.Athena.Logging.SerilogLogger;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using TechTalk.SpecFlow;
using Xunit.Abstractions;

namespace Phyros.Athena.Test.TestAdapters.Logging
{
	public class TestOutputLoggingAdapter : ILoggingAdapter
	{
		private readonly EventLogContainer _eventLogContainer;
		public const string LoggingAdapterThreadDataName = "outputwriter";
		private readonly Logger _logger;

		public TestOutputLoggingAdapter(EventLogContainer eventLogContainer)
		{
			_eventLogContainer = eventLogContainer;
			_logger = new LoggerConfiguration()
				.WriteTo.File(
					$"logs/{DateTime.Now.ToString("yyyy-MM-dd HH-mm", CultureInfo.InvariantCulture)}.txt"
					, rollingInterval: RollingInterval.Day
					, retainedFileCountLimit: 1
					, buffered: true
				)
				.CreateLogger();
		}

		public void WriteEntry(LogEntry entry)
		{
			try
			{
				if (entry.Properties.ContainsKey("processItemId"))
				{
					_eventLogContainer.Add(entry);
				}
			}
			catch 
			{

			}
			try
			{
				_logger.Write(entry.Severity.ToLogEventLevel(), entry.Exception, entry.MessageTemplate, entry.Properties);
			}
			catch
			{
			}
		}
	}
}
