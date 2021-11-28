using System;
using Phyros.Athena.Engines;

namespace Phyros.Athena.Managers.Default
{
	public class AthenaBpmEngineManager : IAthenaBpmEngineManager
	{
		private readonly IAthenaEngine _engine;

		public AthenaBpmEngineManager(IAthenaEngine engine)
		{
			_engine = engine;
		}
		public void Start()
		{
			_engine.Start();
		}

		public void Terminate()
		{
			throw new NotImplementedException();
		}
	}
}
