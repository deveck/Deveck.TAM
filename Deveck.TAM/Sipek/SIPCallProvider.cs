using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

using Deveck.TAM.Actions;
using Deveck.TAM.Core;
using NLog;
using Sipek.Common;
using Sipek.Common.CallControl;
using Sipek.Sip;

namespace Deveck.TAM.Sipek
{
	public class SIPCallProvider : ICallProvider
	{
		public event IncomingCallDelegate OnIncomingCall;
		
		private SipekResources _resources = null;
		private Logger _log = LogManager.GetCurrentClassLogger();
		
		private List<SIPCall> _calls = new List<SIPCall>();
		
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
					
					pjsipCallProxy.OnWavPlayerCompleted += 
						delegate(int callId) 
						{ 
							_log.Info("Wav playback for call '{0}' ended", callId);
							SIPCall call = FindCallById(callId);
							
							if(call != null)
								call.AudioPlaybackCompleted();
						};
				});
		}

		
		private SIPCall FindCallById(int callId)
		{
			foreach(SIPCall call in _calls)
			{
				if(call.SipekCallId.Equals(callId))
					return call;
			}
			
			return null;
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

			//HACK: how to discover accountid?
			IAccount account = _resources.Configurator.Accounts[0];
			IActionProvider actionProvider = null;
			if(account != null && typeof(IActionProvider).IsAssignableFrom(account.GetType()))
				actionProvider = (IActionProvider)account;
			
			
			SIPIncomingCall call = new SIPIncomingCall(this, _resources, callId, number, actionProvider);
			call.CallState = CallState.Ringing;
			
			lock(_calls)
			{
				_calls.Add(call);
			}
			if(OnIncomingCall != null)
				OnIncomingCall(call);
		}
		
		private void onCallStateChanged(int callId)
		{
			_log.Info("Call with id '{0}' changed its state to {1}", callId, _resources.CallManager.getCall(callId).StateId);

			lock(_calls)
			{
				SIPCall call = FindCallById(callId);
				if(call == null)
				{
					_log.Info("Call with id '{0}' is not in list. Ignoring state change", callId);
					return;
				}
				
				switch(_resources.CallManager.getCall(callId).StateId)
				{
					case EStateId.ACTIVE:
						call.CallState = CallState.Connected;
						break;
					case EStateId.INCOMING:
						call.CallState = CallState.Ringing;
						break;
						
						
					case EStateId.TERMINATED:
					case EStateId.IDLE:
						call.CallState = CallState.Disconnected;
						call.Hangup();
						break;
						
					case EStateId.RELEASED:
						call.CallState = CallState.HangUp;
						break;
						
					case EStateId.NULL:
						call.CallState = CallState.Disconnected;
						lock(_calls)
						{
							_log.Info("Removing call '{0}'", callId);
							_calls.Remove(call);
						}
						break;
				}
			}
			
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
