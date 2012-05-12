using System;
using System.Collections.Generic;
using Deveck.TAM.Core;
using NLog;

namespace Deveck.TAM.Triggers
{

	public class TriggerCollector : ITrigger
	{
		private List<ITrigger> _triggers = new List<ITrigger>();
		private String _name;
		
		public TriggerCollector(String name)
		{
			_name = name;
		}
		
		
		public void AddTrigger(ITrigger trigger)
		{
			_triggers.Add(trigger);
		}
		
		public string Name
		{
			get{ return _name; }
		}
		
		public bool IsTriggered(ICall call, DateTime triggerDate)
		{
			foreach(ITrigger trigger in _triggers)
			{
				if(trigger.IsTriggered(call, triggerDate))
					return true;
			}
			
			return false;
		}
	}
}
