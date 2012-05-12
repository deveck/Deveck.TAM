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

namespace Deveck.TAM.Triggers
{
	/// <summary>
	/// Description of XmlTriggerFactory.
	/// </summary>
	public class XmlTriggerFactory
	{
		public static IDictionary<String, ITrigger> LoadTriggers()
		{
			XmlDocument accountDoc = new XmlDocument();
			accountDoc.Load("accounts.xml");
			
			IDictionary<String, ITrigger> triggers = new Dictionary<String, ITrigger>();
			foreach(XmlElement triggerElement in accountDoc.DocumentElement.SelectNodes("trigger"))
			{
				String name = XmlHelper.ReadString(triggerElement, "name");
				
				TriggerCollector collector = new TriggerCollector(name);
				
				foreach(XmlElement realTrigger in triggerElement.ChildNodes)
				{
					if(realTrigger.Name.Equals("day"))
						collector.AddTrigger(new DayTrigger(realTrigger.InnerText, name));
					else if(realTrigger.Name.Equals("ringDelay"))
						collector.AddTrigger(new RingDelay(realTrigger.InnerText, name));
				}
				
				triggers.Add(name, collector);				
			}
			
			return triggers;
			
		}
	}
}
