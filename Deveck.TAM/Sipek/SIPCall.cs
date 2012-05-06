/*
 * Created by SharpDevelop.
 * User: test
 * Date: 01.05.2012
 * Time: 20:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using Deveck.TAM.Core;
using NLog;

namespace Deveck.TAM.Sipek
{
	/// <summary>
	/// Description of SipCall.
	/// </summary>
	public class SIPCall : ICall
	{
		protected SipekResources _resources;
		
		protected SIPCallProvider _callProvider;
		protected DateTime _date;
		protected CallState _callState;
	
		protected int _sipekCallId;
		
		private Logger _log = LogManager.GetCurrentClassLogger();
		
		public SIPCall(SIPCallProvider callProvider, SipekResources resources, int sipekCallId)
		{
			_resources = resources;
			_sipekCallId = sipekCallId;
			_callProvider = callProvider;
		}
		
		public ICallProvider CallProvider {
			get { return _callProvider; }
		}
		
		public DateTime Date {
			get { return _date;	}
		}
		
		public CallState CallState {
			get { return _callState; }
			set
			{
				lock(this)
				{
					_callState = value;
				}
			}
		}
		
		public CallDirection CallDirection {
			get { return CallDirection.Outgoing; }
			
			
		}
		
		public void Hangup()
		{
			_callProvider.Invoke(
				(MethodInvoker)delegate
				{
				  _resources.CallManager.onUserRelease(_sipekCallId);
				});
		}
		
		public void PlayAudioFile(string file)
		{
			_log.Info("Playing wav file '{0}'", file);
//			if(!(CallState == CallState.Connected))
//				throw new InvalidOperationException("Call is not connected");
			
			_callProvider.Invoke(
				(MethodInvoker)delegate
				{
					_log.Info("In invoke", file);
				  _resources.CallManager.playWavFile(0, file);
				});
		}
	}
}
