/*
 * Created by SharpDevelop.
 * User: dev
 * Date: 12.05.2012
 * Time: 00:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Deveck.TAM.Triggers
{
	/// <summary>
	/// Description of Triggers.
	/// </summary>
	public class TriggerManager
	{
		private static TriggerManager _instance = null;
		
		public static TriggerManager Instance
		{
			get
			{
				if(_instance == null)
				{
					_instance = new TriggerManager();
				}
				
				return _instance;
			}
		}
		
		private IDictionary<String, ITrigger> _triggers;
		
		private TriggerManager()
		{
			_triggers = XmlTriggerFactory.LoadTriggers();
		}

		public ITrigger FindTrigger(String name)
		{
			return _triggers[name];
		}
		
	}
}
