using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Phyros.Athena.EventQueue;
using Phyros.Athena.Logging;

namespace Phyros.Athena.Test.TestAdapters
{
	public class TestWorkflowEngineEventQueue : IWorkflowEngineEventQueue
	{
		private readonly ILoggingAdapter _loggingAdapter;

		private readonly List<EventNotification> _notifications;
		private readonly ReaderWriterLockSlim _notificationsLock = new ReaderWriterLockSlim();
		private readonly List<EventSubscription> _subscriptions;
		private readonly ReaderWriterLockSlim _subscriptionsLock = new ReaderWriterLockSlim();

		public TestWorkflowEngineEventQueue(ILoggingAdapter loggingAdapter)
		{
			_loggingAdapter = loggingAdapter;
			_loggingAdapter.WriteEntry(new LogEntry(LoggingEventType.Information, "Test Event Queue created"));
			_notifications = new List<EventNotification>();
			_subscriptions = new List<EventSubscription>();
		}
		public void PublishEvent(EventNotification notification)
		{
			_notifications.Add(notification);
			_loggingAdapter.WriteEntry(new LogEntry(LoggingEventType.Information, "New event published. {notification}",
				new Dictionary<string, object>()
				{
					{ "notification", JsonConvert.SerializeObject(notification)},
					{ "processItemId", notification.ProcessItemId }
				}));
		}

		public EventSubscription SubscribeToEvents(Predicate<EventNotification> query)
		{
			var subscription = new EventSubscription()
			{
				Matches = query
			};
			_notificationsLock.EnterWriteLock();
			_subscriptions.Add(subscription);
			_notificationsLock.ExitWriteLock();
			return subscription;
		}

		public void ReleaseOne(string processItemId)
		{
			var notification = _notifications.ToList().Where(x => x.ProcessItemId == processItemId).OrderBy(x => x.Timestamp).FirstOrDefault();
			if (notification == null)
			{
				for (var i = 0; notification == null && i < 3; i++)
				{
					Thread.Sleep(1000);
					notification = _notifications.ToList().Where(x => x.ProcessItemId == processItemId).OrderBy(x => x.Timestamp).FirstOrDefault();
				}
			}

			if (notification == null)
			{
				return;
				throw new Exception("No message was found to release.");
			}
			foreach (var subscriber in _subscriptions.ToList().Where(subscriber => subscriber.Matches(notification)))
			{
				subscriber.MessageReceived(notification);
				_notificationsLock.EnterUpgradeableReadLock();
				if (_notifications.Contains(notification))
				{
					_notificationsLock.EnterWriteLock();
					if (_notifications.Contains(notification))
					{
						_notifications.Remove(notification);
					}
					_notificationsLock.ExitWriteLock();
				}
				_notificationsLock.ExitUpgradeableReadLock();
			}
		}
	}
}
