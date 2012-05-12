/*
 * Created by SharpDevelop.
 * User: test
 * Date: 01.05.2012
 * Time: 20:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Windows.Forms;

using Deveck.TAM.Actions;
using Deveck.TAM.Core;
using NLog;

namespace Deveck.TAM.Sipek
{
	/// <summary>
	/// Description of SipCall.
	/// </summary>
	public class SIPCall : ICall
	{
		protected AutoResetEvent _audioPlaybackCompleted = new AutoResetEvent(false);
		protected SipekResources _resources;
		
		protected SIPCallProvider _callProvider;
		protected DateTime _date;
		protected CallState _callState;
		
		protected int _sipekCallId;
		protected IActionProvider _actionProvider;
		
		private Logger _log = LogManager.GetCurrentClassLogger();
		
		public event Action<ICall> OnAudioPlaybackCompleted;
		
		public SIPCall(SIPCallProvider callProvider, SipekResources resources, int sipekCallId, IActionProvider actionProvider)
		{
			_resources = resources;
			_sipekCallId = sipekCallId;
			_callProvider = callProvider;
			_actionProvider = actionProvider;
			_date = DateTime.Now;
		}
		
		public int SipekCallId
		{
			get{ return _sipekCallId; }
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
				_callState = value;
				_log.Info("Changed state of call '{0}' to '{1}'", SipekCallId, CallState);
			}
		}
		
		public IActionProvider ActionProvider
		{
			get{ return _actionProvider;}
		}
		
		public CallDirection CallDirection {
			get { return CallDirection.Outgoing; }
			
			
		}
		
		public void Hangup()
		{
			if(CallState == CallState.Connected)
			{
			_callProvider.Invoke(
				(MethodInvoker)delegate
				{
					_resources.CallManager.onUserRelease(_sipekCallId);
				});
			}
			_audioPlaybackCompleted.Set();
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
					_resources.CallManager.playWavFile(_sipekCallId, file);
				});
			
			_audioPlaybackCompleted.WaitOne();
		}
		
		public void AudioPlaybackCompleted()
		{
			_audioPlaybackCompleted.Set();
			if(OnAudioPlaybackCompleted != null)
				OnAudioPlaybackCompleted(this);
		}
	}
}
