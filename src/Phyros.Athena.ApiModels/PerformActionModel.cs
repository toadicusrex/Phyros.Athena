using System.Collections.Generic;

namespace Phyros.Athena.ApiModels
{
	public class PerformActionModel
	{
		public string ProcessItemId { get; set; }
		public IDictionary<string, object> Data { get; set; }
	}
}
