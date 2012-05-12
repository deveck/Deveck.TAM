/*
 * Created by SharpDevelop.
 * User: dev
 * Date: 11.05.2012
 * Time: 22:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Xml;

using Deveck.TAM.Triggers;

namespace Deveck.TAM.Actions
{
	/// <summary>
	/// Description of XmlTriggerFactory.
	/// </summary>
	public class XmlActionFactory
	{
		public static IDictionary<String, ActionPack> LoadActions()
		{
			XmlDocument accountDoc = new XmlDocument();
			accountDoc.Load("accounts.xml");
			
			IDictionary<String, ActionPack> actionPacks = new Dictionary<String, ActionPack>();
			foreach(XmlElement actionsElement in accountDoc.DocumentElement.SelectNodes("actions"))
			{
				String name = XmlHelper.ReadString(actionsElement, "name");
				List<ITrigger> triggers = new List<ITrigger>();
				foreach(XmlElement triggerElement in actionsElement.SelectNodes("trigger"))
				{
					triggers.Add(TriggerManager.Instance.FindTrigger(triggerElement.InnerText));
				}
				
				List<IAction> actions = new List<IAction>();
				
				foreach(XmlElement realTrigger in actionsElement.ChildNodes)
				{
					if(realTrigger.Name.Equals("playwav"))
						actions.Add(new PlaywavAction(realTrigger.InnerText));
					else if(realTrigger.Name.Equals("delay"))
						actions.Add(new DelayAction(int.Parse(realTrigger.InnerText)));
					else if(realTrigger.Name.Equals("hangup"))
						actions.Add(new HangupAction());
					else if(realTrigger.Name.Equals("accept"))
						actions.Add(new AcceptCallAction());
				}
				
				actionPacks.Add(name, new ActionPack(name, triggers.ToArray(), actions.ToArray()));
			}
			
			return actionPacks;
			
		}
	}
}
