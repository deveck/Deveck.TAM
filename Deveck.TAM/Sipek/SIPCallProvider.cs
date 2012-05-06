using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

using Deveck.TAM.Core;
using NLog;
using Sipek.Common;
using Sipek.Common.CallControl;

namespace Deveck.TAM.Sipek
{
	public class SIPCallProvider : ICallProvider
	{
		public event IncomingCallDelegate OnIncomingCall;
		
		private SipekResources _resources = null;
		private Logger _log = LogManager.GetCurrentClassLogger();
		
		private List<SIPIncomingCall> _incomingCalls = new List<SIPIncomingCall>();
		
		private Thread _sipThread = null;
		private Dispatcher _dispatcher = null;
		private AutoResetEvent _initialized = new AutoResetEvent(false);
		
		private void InitThread()
		{
			_sipThread = new Thread(new ThreadStart(
				delegate
				{
					_dispatcher = Dispatcher.CurrentDispatcher;
					_initialized.Set();
					Dispatcher.Run();					
				}));
			_sipThread.Start();
		}
		
		public void Invoke(Delegate target)
		{
			_dispatcher.Invoke(DispatcherPriority.Normal, target);
		}
		
		public void Initialize()
		{
			if(_resources != null)
				Dispose();
			
			InitThread();
			
			_initialized.WaitOne();
			_dispatcher.Invoke(
				DispatcherPriority.Normal,
				(MethodInvoker)delegate
				{
					_log.Info("Initializing SIPCallProvider");
					_resources = new SipekResources();
					_resources.CallManager.CallStateRefresh += onCallStateChanged;
					_resources.CallManager.IncomingCallNotification += new DIncomingCallNotification(resources_CallManager_IncomingCallNotification);
					_resources.Registrar.AccountStateChanged += new DAccountStateChanged(resources_Registrar_AccountStateChanged);
					
					int status = _resources.CallManager.Initialize();
					_resources.CallManager.CallLogger = _resources.CallLogger;
					
					if (status != 0)
						throw new CallProviderException("Init SIP stack problem! \r\nPlease, check configuration and start again! \r\nStatus code " + status);
					
					// initialize Stack
					_resources.Registrar.registerAccounts();
					
					
					int noOfCodecs = _resources.StackProxy.getNoOfCodecs();
					for (int i = 0; i < noOfCodecs; i++)
					{
						string codecname = _resources.StackProxy.getCodec(i);
						if (_resources.Configurator.CodecList.Contains(codecname))
						{
							// leave default
							_resources.StackProxy.setCodecPriority(codecname, 128);
						}
						else
						{
							// disable
							_resources.StackProxy.setCodecPriority(codecname, 0);
						}
						
					}
				});
		}


		private void resources_Registrar_AccountStateChanged(int accountId, int accState)
		{
			if(_resources.Configurator.Accounts.Count < accountId)
				_log.Error("Account state of nonexisting account {0} changed to {1}", accountId, accState);
			else
				_log.Info("Account state of Account {0} changed to {1}",  _resources.Configurator.Accounts[accountId], accState);
		}

		private void resources_CallManager_IncomingCallNotification(int callId, string number, string info)
		{
			_log.Info("Incoming call with id '{0}' from '{1}', info: {2}", callId, number, info);
			
			SIPIncomingCall call = new SIPIncomingCall(this, _resources, callId, number);
			_incomingCalls.Add(call);
			if(OnIncomingCall != null)
				OnIncomingCall(call);
		}
		
		private void onCallStateChanged(int callId)
		{
			_log.Info("Call with id '{0}' changed its state to {1}", callId, _resources.CallManager.getCall(callId).StateId);

		}
		
		public void Dispose()
		{
			_log.Info("Shuting down SIPCallProvider");
			if(_resources != null)
			{
				_dispatcher.Invoke(
					DispatcherPriority.Normal,
					(MethodInvoker)delegate
					{
						_resources.StackProxy.shutdown();
						_resources.CallManager.Shutdown();
						_resources = null;
					});
			}
		}
		
		
	}
}
