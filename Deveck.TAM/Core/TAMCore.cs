/*
 * Created by SharpDevelop.
 * User: test
 * Date: 01.05.2012
 * Time: 13:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Threading;

using Deveck.TAM.Actions;

namespace Deveck.TAM.Core
{
	/// <summary>
	/// Description of TAM.
	/// </summary>
	public class TAMCore : IDisposable
	{
		private List<ICallProvider> _callProviders = new List<ICallProvider>();
		
		public TAMCore()
		{
			ActionManager.Load();
		}
		
		public void AddCallProvider(ICallProvider provider)
		{
			if(!_callProviders.Contains(provider))
			{
				_callProviders.Add(provider);
				provider.OnIncomingCall += new IncomingCallDelegate(provider_OnIncomingCall);
			}
		}		
		
		public void Initialize()
		{
			foreach(ICallProvider callProvider in _callProviders)
			{
				callProvider.Initialize();
			}
		}
		
		public void Dispose()
		{
			foreach(ICallProvider callProvider in _callProviders)
				callProvider.Dispose();
		}
		
		private void provider_OnIncomingCall(IIncomingCall incomingCall)
		{			
			ThreadPool.UnsafeQueueUserWorkItem(CallActionsThread, incomingCall);			
		}
		
		private void CallActionsThread(object state)
		{
			ICall call = (ICall)state;
			
			if(call.ActionProvider.ActionPacks == null || call.ActionProvider.ActionPacks.Length == 0)
			{
				call.Hangup();
				return;
			}
			
			List<ActionPack> actions = new List<ActionPack>(call.ActionProvider.ActionPacks);
			while(actions.Count > 0 && call.CallState != CallState.Error && call.CallState != CallState.Disconnected && call.CallState != CallState.HangUp)
			{
				ActionPack current = actions[0];
				if(current.TriggeredExecution(call))
					actions.RemoveAt(0);
				
				Thread.Sleep(500);
			}
			
			if(call.CallState != CallState.Error && call.CallState != CallState.Disconnected && call.CallState != CallState.HangUp)
				call.Hangup();
		}
	}
}
