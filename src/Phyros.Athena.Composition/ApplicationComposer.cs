using System;
using System.Collections.Generic;
using Phyros.Athena.Engines;
using Phyros.Athena.Engines.Default;
using Phyros.Athena.Engines.Default.TaskHandlers;
using Phyros.Athena.Engines.Default.TaskHandlers.Validators;
using Phyros.Athena.EngineTaskQueue;
using Phyros.Athena.Logging;
using Phyros.Athena.Logging.SerilogLogger;
using Phyros.Athena.Managers;
using Phyros.Athena.Managers.Default;
using SimpleInjector;

namespace Phyros.Athena.Composition
{
	public static class ApplicationComposer
	{
		public static readonly Container Container = new Container();

		public static void Compose()
		{
			var setupLogger = ConfigureSerilogLogger();

			setupLogger.WriteEntry(new LogEntry(LoggingEventType.Information, "Wireup started.", new Dictionary<string, object>(){}));

			Container.Register<IAthenaBpmEngineManager, AthenaBpmEngineManager>();
			Container.Register<IAthenaBpmManager, AthenaBpmManager>();
			Container.Register<IAthenaEngine, AthenaEngine>();

			Container.Register<ActionTaskHandler>();
			Container.Register<LockTaskHandler>();
			Container.Register<UnlockTaskHandler>();
			Container.Register<Func<string, IEngineTaskHandler>>(() => (taskKind) =>
			{
				return taskKind switch
				{
					"ActionTask" => Container.GetInstance<ActionTaskHandler>(),
					"LockTask" => Container.GetInstance<LockTaskHandler>(),
					"UnlockTask" => Container.GetInstance<UnlockTaskHandler>(),
					_ => throw new Exception($"No engine task handler configured for task kind '{ taskKind }'.")
				};
			});

			Container.Register<CurrentUserCanLockProcessItemValidator>();
			Container.Register<RequestIsForTheMostRecentProcessItemStateValidator>();
			Container.Register<Func<string, IEngineTaskValidator>>(() => (validatorKind) =>
			{
				return validatorKind switch
				{
					EngineTaskValidatorKinds.CurrentUserCanLockProcessItem => Container.GetInstance<CurrentUserCanLockProcessItemValidator>(),
					EngineTaskValidatorKinds.RequestIsForTheMostRecentState => Container.GetInstance<RequestIsForTheMostRecentProcessItemStateValidator>(),
					_ => throw new Exception($"No engine task validator configured for validator kind '{ validatorKind }'.")
				};
			});

		}

		private static ILoggingAdapter ConfigureSerilogLogger()
		{
			return WireupCommand.WireupSerilog(Container);
		}

		public static IAthenaBpmEngineManager GetEngineManager()
		{
			return Container.GetInstance<IAthenaBpmEngineManager>();
		}
	}
}
