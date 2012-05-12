/*
 * Created by SharpDevelop.
 * User: test
 * Date: 01.05.2012
 * Time: 20:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;
using Deveck.TAM.Actions;
using Deveck.TAM.Core;
using NLog;

namespace Deveck.TAM.Sipek
{
	/// <summary>
	/// Description of SipIncomingCall.
	/// </summary>
	public class SIPIncomingCall : SIPCall, IIncomingCall
	{
		private Logger _log = LogManager.GetCurrentClassLogger();
		
		private String _remoteIdentifier;
		
		public SIPIncomingCall(SIPCallProvider callProvider, SipekResources resources, int sipekCallId, String remoteIdentifier, IActionProvider actionProvider)
			:base(callProvider, resources, sipekCallId, actionProvider)
		{
			_remoteIdentifier = remoteIdentifier;	
			
		}
		
		public void AcceptCall()
		{
			_log.Info("Accepting call '{0}' remote identifier '{1}'", _sipekCallId, _remoteIdentifier);
			lock(this)
			{
				_callProvider.Invoke(
					(MethodInvoker)delegate
					{
						if(CallState == CallState.Ringing)
						  _resources.CallManager.onUserAnswer(_sipekCallId);
					});
			}
		}
	}
}
