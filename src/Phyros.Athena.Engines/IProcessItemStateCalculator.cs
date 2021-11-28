using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Phyros.Athena.Engines.Model;

namespace Phyros.Athena.Engines
{
	public interface IProcessItemStateCalculator
	{
		ProcessItemState GetSingleItem(string processItemId, ClaimsPrincipal principal);
	}
}
