using System;
using Deveck.TAM.Core;

namespace Deveck.TAM.Triggers
{
	public class RingDelay : ITrigger
	{
		private int _delay;
		private String _name;
		
		public RingDelay(String triggerText, String name)
		{
			_name = name;
			
			if(!int.TryParse(triggerText, out _delay))
				throw new ArgumentException(String.Format("Cannot parse ring delay '{0}'", triggerText));
		}
		
		public string Name
		{
			get{ return _name; }
		}
		
		public bool IsTriggered(ICall call, DateTime triggerDate)
		{
			if( new TimeSpan(triggerDate.Ticks - call.Date.Ticks).TotalMilliseconds >= _delay)
				return true;
			
			return false;
		}
		
		public override string ToString()
		{
			return string.Format("[RingDelay Delay={0}]", _delay);
		}

	}
}
