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

namespace Deveck.TAM.Actions
{
	/// <summary>
	/// Description of Triggers.
	/// </summary>
	public class ActionManager
	{
		private static ActionManager _instance = null;
		
		public static ActionManager Instance
		{
			get
			{
				Load();
				return _instance;
			}
		}
		
		public static void Load()
		{
			if(_instance == null)
			{
				_instance = new ActionManager();
			}
		}
		
		private IDictionary<String, ActionPack> _actions;
		
		private ActionManager()
		{
			_actions = XmlActionFactory.LoadActions();
		}
		
		public ActionPack FindActionPack(String name)
		{
			return _actions[name];
		}
	}
}
