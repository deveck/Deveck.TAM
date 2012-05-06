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
			ThreadPool.UnsafeQueueUserWorkItem(
				(WaitCallback)delegate(object state){
					Thread.Sleep(1000);
					incomingCall.AcceptCall();
					Thread.Sleep(1000);
					incomingCall.PlayAudioFile("C:\\austinpowers.wav");
				}, null);
		}
	}
}
