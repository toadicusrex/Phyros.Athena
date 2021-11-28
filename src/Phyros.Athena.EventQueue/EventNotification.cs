using System;

namespace Phyros.Athena.EventQueue
{
	public class EventNotification
	{
		public EventNotification()
		{
			Timestamp = DateTime.UtcNow;
		}

		public DateTime Timestamp { get; set; }

		public string ProcessId { get; set; }
		public string ProcessItemId { get; set; }
		public string ActionId { get; set; }
		public string NotificationKind { get; set; }
		public Exception Exception { get; set; }
		public string PrincipalId { get; set; }
	}
}