/* 
 * Copyright (C) 2007 Sasa Coh <sasacoh@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Sipek.Common;
using Sipek.Common.CallControl;

#if DEBUG

using NUnit.Framework;


namespace UnitTest
{
	public class MockSipCallProxy : ICallProxyInterface
	{
	  private int _sessionId = -1;
	  public override int makeCall(string dialedNo, int accountId) { return 1; }
	
	  public override bool endCall() { return true; }
	
	  public override bool alerted() { return true; }
	
	  public override bool acceptCall() { return true; }
	
	  public override bool holdCall() { return true; }
	
	  public override bool retrieveCall() { return true; }
	
	  public override bool xferCall(string number) { return true; }
	
	  public override bool xferCallSession(int partnersession) { return true; }
	
	  public override bool threePtyCall(int partnersession) { return true; }
	
	  public override bool serviceRequest(int code, string dest) { return true; }
	
	  public override bool dialDtmf(string digits, EDtmfMode mode) { return true; }
	
	  public override int SessionId
	  {
	    get
	    {
	      return 1;
	    }
	    set
	    {
	      _sessionId = value;
	    }
	  }
	
	  public int makeCallByUri(string uri)
	  {
	    throw new Exception("The method or operation is not implemented.");
	  }
	
	
	  public static void onIncomingCall(int sessionId, string number, string info)
	  {
	    BaseIncomingCall(sessionId, number, info);
	  }
	
	  public static void OnCallStateChanged(int callId, ESessionState callState, string info)
	  {
	    BaseCallStateChanged(callId, callState, info);
	  }
	
	
	  public override string getCurrentCodec()
	  {
	    throw new Exception("The method or operation is not implemented.");
	  }
	
	  public override bool conferenceCall()
	  {
	    throw new Exception("The method or operation is not implemented.");
	  }
		
		public override int playWavFile(string file)
		{
			throw new NotImplementedException();
		}
	}
}
#endif
