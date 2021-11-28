using System;

namespace Phyros.Athena.EventQueue
{
	public interface IWorkflowEngineEventQueue
	{
		void PublishEvent(EventNotification notification);
		EventSubscription SubscribeToEvents(Predicate<EventNotification> query);
	}
}
