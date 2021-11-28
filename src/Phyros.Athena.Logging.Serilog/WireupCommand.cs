using System;
using System.Globalization;
using Serilog;
using SimpleInjector;

namespace Phyros.Athena.Logging.SerilogLogger
{
	public static class WireupCommand
	{
		public static ILoggingAdapter WireupSerilog(Container container)
		{
			container.RegisterSingleton<ILoggingAdapter, SerilogLoggingAdapter>();

			Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

			
			
			return new SerilogLoggingAdapter();
		}
	}
}
