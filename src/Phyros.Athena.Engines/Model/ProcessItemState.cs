using System;
using System.Collections.Generic;
using System.Text;
using Phyros.Athena.Model.Workflow.ProcessItems;

namespace Phyros.Athena.Engines.Model
{
	public class ProcessItemState
	{
		IProcessItem ProcessItem { get;set; }
		Athena.Model.Workflow.ProcessItems.ProcessItemState StoredState {get;set;}
		IEnumerable<ProcessItemStateActions> AvailableActions {get;set;}
	}
}
