using System.Collections.Generic;
using Serilog;
using Serilog.Events;
using System.Linq;
using Newtonsoft.Json;

namespace Phyros.Athena.Logging.SerilogLogger
{
	public class SerilogLoggingAdapter : ILoggingAdapter
	{
		public void WriteEntry(LogEntry entry)
		{
			//Log
			//	.ForContext("AdditionalProperties", entry.Properties)
			//	.Write(entry.Severity.ToLogEventLevel(), entry.Exception, entry.MessageTemplate, entry.Properties);
		}

		public void Close()
		{
			Log.CloseAndFlush();
		}
	}

	public static class PropertyDictionaryExtensions
	{
		public static IEnumerable<LogEventProperty> ToEnumerableOfLogEventProperties(
			this Dictionary<string, object> properties)
		{
			return properties.Select(x => new LogEventProperty(x.Key, new ScalarValue(JsonConvert.SerializeObject(x.Value))));
		}
	}
}
