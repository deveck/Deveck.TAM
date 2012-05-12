using System;
using Deveck.TAM.Core;
using Deveck.TAM.Triggers;
using NLog;

namespace Deveck.TAM.Actions
{
	/// <summary>
	/// Description of ActionPack.
	/// </summary>
	public class ActionPack
	{
		private Logger _log = LogManager.GetCurrentClassLogger();
		
		private ITrigger[] _triggers;
		private IAction[] _actions;
		private String _name;
		
		public ITrigger[] Triggers
		{
			get{ return _triggers;}
		}
		
		public String Name
		{
			get{ return _name;}
		}
		
		public IAction[] Action
		{
			get{ return _actions; }
		}
		
		
		public ActionPack(String name, ITrigger[] triggers, IAction[] actions)
		{
			_name = name;
			_triggers = triggers;
			_actions = actions;
		}
		
		public bool TriggeredExecution(ICall call)
		{
			foreach(ITrigger trigger in _triggers)
			{
				if(!trigger.IsTriggered(call, DateTime.Now))
					return false;
			}
			
			_log.Info("Actionpack '{0}' is triggered for call '{1}'", _name, call);
			
			foreach(IAction action in _actions)
			{
				try
				{
					_log.Info("started action '{0}'", action.GetType());
					action.Execute(call);
					_log.Info("Completed action '{0}'", action.GetType());
				}
				catch(Exception e)
				{
					_log.ErrorException(string.Format("Error executing action '{0}'",action.GetType()), e);
				}
			}
			
			return true;
		}
	}
}
