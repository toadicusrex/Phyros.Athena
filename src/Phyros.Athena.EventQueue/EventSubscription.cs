using System;

namespace Phyros.Athena.EventQueue
{
	public class EventSubscription
	{
		public event NotificationReceivedDelegate OnMessageReceived;

		public Predicate<EventNotification> Matches;

		public void MessageReceived(EventNotification notification)
		{
			OnMessageReceived?.Invoke(this, new NotificationReceivedDelegateArgs()
			{
				Notification = notification
			});
		}

		public EventSubscription SetupCallback(NotificationReceivedDelegate eventAction)
		{
			OnMessageReceived += eventAction;
			return this;
		}
	}

	public delegate void NotificationReceivedDelegate(object sender, NotificationReceivedDelegateArgs args);

	public class NotificationReceivedDelegateArgs
	{
		public EventNotification Notification { get; set; }
	}
}