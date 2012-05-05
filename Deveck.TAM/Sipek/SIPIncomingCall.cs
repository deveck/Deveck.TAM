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
using Deveck.TAM.Core;

namespace Deveck.TAM.Sipek
{
	/// <summary>
	/// Description of SipIncomingCall.
	/// </summary>
	public class SIPIncomingCall : SIPCall, IIncomingCall
	{
		private String _remoteIdentifier;
		
		public SIPIncomingCall(SIPCallProvider callProvider, SipekResources resources, int sipekCallId, String remoteIdentifier)
			:base(callProvider, resources, sipekCallId)
		{
			_remoteIdentifier = remoteIdentifier;	
		}
		
		public void AcceptCall()
		{
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
