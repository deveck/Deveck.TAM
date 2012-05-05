/*
 * Created by SharpDevelop.
 * User: Andreas Reiter <development@wiffzack.com>
 * Date: 22.04.2012
 * Time: 19:11
 * 
 * This file is part of GastroboniSQL (www.wiffzack.com)
 */
using System;
using System.ServiceProcess;
using System.Threading;

using Deveck.TAM.Core;
using Deveck.TAM.Sipek;
using Deveck.Utils;
using Sipek.Common;
using Sipek.Common.CallControl;

namespace Deveck.TAM
{
	class Program : ServiceBase
	{
		private static bool _consoleMode = false;
		private static TAMCore _tam = null;
		
		
		public static void Main(string[] args)
		{
			CommandLineHandler cmdLine = new CommandLineHandler();
			cmdLine.RegisterCallback("console",new Action<CommandLineHandler.CommandOption>(cmdLine_Console));
			cmdLine.Parse(args);
			if(_consoleMode)
			{
				Initialize();
				Thread.Sleep(Timeout.Infinite);
			}
		}

		private static void cmdLine_Console(CommandLineHandler.CommandOption cmdOption)
		{
			_consoleMode = true;
		}
		
		private static void Initialize()
		{
			_tam = new TAMCore();
			_tam.AddCallProvider(new SIPCallProvider());	
			_tam.Initialize();
		}
		
		protected override void OnStart(string[] args)
		{
			Initialize();
			base.OnStart(args);
		}
		
		protected override void OnStop()
		{
			if(_tam != null)
			{
				_tam.Dispose();
				_tam = null;
			}
			base.OnStop();
		}
	}
}