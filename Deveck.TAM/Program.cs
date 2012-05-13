/*
 * Created by SharpDevelop.
 * User: Andreas Reiter <development@wiffzack.com>
 * Date: 22.04.2012
 * Time: 19:11
 * 
 * This file is part of GastroboniSQL (www.wiffzack.com)
 */
using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

using Deveck.TAM.Core;
using Deveck.TAM.Sipek;
using Deveck.Utils;
using NLog;
using Sipek.Common;
using Sipek.Common.CallControl;

namespace Deveck.TAM
{
	class Program : ServiceBase
	{
		private static bool _consoleMode = false;
		private static TAMCore _tam = null;
		
		private static Logger _log = null;
		
		public static void Main(string[] args)
		{
			Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			_log = LogManager.GetCurrentClassLogger();
			
			CommandLineHandler cmdLine = new CommandLineHandler();
			cmdLine.RegisterCallback("console",new Action<CommandLineHandler.CommandOption>(cmdLine_Console));
			cmdLine.Parse(args);
			if(_consoleMode)
			{
				Initialize();
				Thread.Sleep(Timeout.Infinite);
			}
			else
			{
				System.ServiceProcess.ServiceBase[] services_to_run =
					new  ServiceBase[] { new Program() };
				
				System.ServiceProcess.ServiceBase.Run(services_to_run);
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
			try{
				_log.Info("Starting service...");
				Initialize();
				base.OnStart(args);
			}
			catch(Exception e)
			{
				_log.ErrorException("Error initializing", e);
				throw e;
			}
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
		
		public Program()
		{
			ServiceName = "Deveck.TAM";
		}
	}
}