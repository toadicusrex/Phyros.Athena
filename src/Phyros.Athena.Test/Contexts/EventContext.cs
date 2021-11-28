using Phyros.Athena.EventQueue;
using TechTalk.SpecFlow;

namespace Phyros.Athena.Test.Contexts
{
	public class EventContext
	{
		public EventSubscription Subscription { get; set; }
		public EventNotification LastNotification { get; set; }
	}
}