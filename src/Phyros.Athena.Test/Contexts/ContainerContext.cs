using System;
using System.Collections.Generic;
using System.Text;
using Phyros.Athena.EventQueue;
using Phyros.Athena.Managers;
using Phyros.Athena.ProcessItemStore;
using Phyros.Athena.ProcessStore;
using Phyros.Athena.Test.TestAdapters.Logging;
using SimpleInjector;

namespace Phyros.Athena.Test.Contexts
{
	public class ContainerContext
	{
		public Container Container { private get; set; }

		public IProcessItemEventStore GetProcessItemEventStore()
		{
			return Container.GetInstance<IProcessItemEventStore>();
		}

		public IWorkflowEngineEventQueue GetWorkflowEngineEventQueue()
		{
			return Container.GetInstance<IWorkflowEngineEventQueue>();
		}

		public EventLogContainer GetEventLogContainer()
		{
			return Container.GetInstance<EventLogContainer>();
		}

		public IAthenaBpmManager GetAthenaBpmManager()
		{
			return Container.GetInstance<IAthenaBpmManager>();
		}

		public IWorkflowProcessStore GetWorkflowProcessStore()
		{
			return Container.GetInstance<IWorkflowProcessStore>();
		}
	}
}
