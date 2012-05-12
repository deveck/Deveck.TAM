using System;
using System.Threading;
using Deveck.TAM.Core;

namespace Deveck.TAM.Actions
{
	public class DelayAction : IAction
	{
		private int _delay;
		
		public DelayAction(int delay)
		{
			_delay = delay;
		}
		
		public void Execute(ICall call)
		{
			Thread.Sleep(_delay);
		}
	}
}
