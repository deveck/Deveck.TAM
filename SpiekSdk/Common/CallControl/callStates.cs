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
 * 
 * @see http://voipengine.googlepages.com/
 * 
 */

using Sipek.Common;


namespace Sipek.Common.CallControl
{

  #region IdleState
  /// <summary>
  /// State Idle indicates the call is inactive
  /// </summary>
  internal class CIdleState : IAbstractState
  {
    public CIdleState(IStateMachine sm) 
      : base(sm)
    {
      Id = EStateId.IDLE;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }

    public override bool endCall()
    {
      _smref.destroy();
      CallProxy.endCall();
      return base.endCall();
    }

    /// <summary>
    /// Make call to a given number and accountId. Assign sessionId to call state machine got from VoIP part.
    /// </summary>
    /// <param name="dialedNo"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public override int makeCall(string dialedNo, int accountId)
    {
      _smref.CallingNumber = dialedNo;
      // make call and save sessionId
      _smref.changeState(EStateId.CONNECTING);
      _smref.Session = CallProxy.makeCall(dialedNo, accountId);
      return _smref.Session;
    }

    /// <summary>
    /// Handle incoming call requests. Check for supplementary services flags.
    /// </summary>
    /// <param name="callingNo"></param>
    /// <param name="display"></param>
    public override void incomingCall(string callingNo, string display)
    {
      // check supplementary services flags
      if ((_smref.Config.CFUFlag == true) && (_smref.Config.CFUNumber.Length > 0))
      {
        // do NOT notify about state changes 
        _smref.DisableStateNotifications = true;
        CallProxy.serviceRequest((int)EServiceCodes.SC_CFU, _smref.Config.CFUNumber);
        this.endCall();
        return;
      }
      else if (_smref.Config.DNDFlag == true)
      {
        // do NOT notify about state changes 
        _smref.DisableStateNotifications = true;
        CallProxy.serviceRequest((int)EServiceCodes.SC_DND, "");
        this.endCall();
        return;
      }

      _smref.CallingNumber = callingNo;
      _smref.CallingName = display;
      _smref.changeState(EStateId.INCOMING);
    }

  }
  #endregion 

  #region ConnectingState
  /// <summary>
  /// Connecting states indicates outgoing call has been initiated and waiting for a response.
  /// </summary>
  internal class CConnectingState : IAbstractState
  {
    public CConnectingState(CStateMachine sm) 
      : base(sm)
    {
      Id = EStateId.CONNECTING;
    }

    public override void onEntry()
    {
      _smref.Type = ECallType.EDialed;
    }

    public override void onExit()
    {
    }

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
    }

    public override void onAlerting()
    {
      _smref.changeState(EStateId.ALERTING);
    }


    public override void onConnect()
    {
      _smref.changeState(EStateId.ACTIVE);
    }

    public override bool endCall()
    {
      _smref.changeState(EStateId.TERMINATED);
      CallProxy.endCall();
      return base.endCall();
    }

  }
  #endregion

  #region AlertingState
  /// <summary>
  /// Alerting state indicates other side accepts the call. Play ring back tone.
  /// </summary>
  internal class CAlertingState : IAbstractState
  {
    public CAlertingState(CStateMachine sm)
      : base(sm)
    {
      Id = EStateId.ALERTING;
    }

    public override void onEntry()
    {
      MediaProxy.playTone(ETones.EToneRingback);
    }

    public override void onExit()
    {
      MediaProxy.stopTone();
    }

    public override void onConnect()
    {
      _smref.Time = System.DateTime.Now;
      _smref.changeState(EStateId.ACTIVE);
    }

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
    }

    public override bool endCall()
    {
      _smref.changeState(EStateId.TERMINATED);
      CallProxy.endCall();
      return base.endCall();
    }
  }
  #endregion

  #region ActiveState
  /// <summary>
  /// Active state indicates converstation. 
  /// </summary>
  internal class CActiveState : IAbstractState
  {
    public CActiveState(CStateMachine sm) 
      : base(sm)
    {
      Id = EStateId.ACTIVE;
    }

    public override void onEntry()
    {
      _smref.Counting = true;
      MediaProxy.stopTone();
    }

    public override void onExit()
    {
    }

    public override bool endCall()
    {
      _smref.Duration = System.DateTime.Now.Subtract(_smref.Time);

      _smref.changeState(EStateId.TERMINATED);
      CallProxy.endCall();
      return base.endCall();
    }

    public override bool holdCall()
    {
      _smref.HoldRequested = true;
      return CallProxy.holdCall();
    }

    public override bool xferCall(string number)
    {
      return CallProxy.xferCall(number);
    }
    public override bool xferCallSession(int partnersession)
    {
      return CallProxy.xferCallSession(partnersession);
    }

    public override void onHoldConfirm()
    {
      // check if Hold requested
      if (_smref.HoldRequested)
      {
        _smref.changeState(EStateId.HOLDING);
        // activate pending action if any
        _smref.activatePendingAction();
      }
      _smref.HoldRequested = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
    }

    public override bool conferenceCall()
    {
      return CallProxy.conferenceCall();
    }
  }
  #endregion

  #region ReleasedState
  /// <summary>
  /// Released State indicates call has been released from other party (stack) 
  /// and now waiting for destruction. The state machine will be release on 
  /// endCall user request or released timer timeout.
  /// </summary>
  internal class CReleasedState : IAbstractState
  {
    public CReleasedState(CStateMachine sm)
      : base(sm)
    {
      Id = EStateId.RELEASED;
    }

    /// <summary>
    /// Enter release state. If release timer not implemented release call imediately
    /// </summary>
    public override void onEntry()
    {
      MediaProxy.playTone(ETones.EToneCongestion);
      bool success = _smref.startTimer(ETimerType.ERELEASED);
      if (!success) _smref.destroy();
    }

    public override void onExit()
    {
      MediaProxy.stopTone();
      _smref.stopAllTimers();
    }

    public override bool endCall()
    {
      // try once more (might not be needed)!
      CallProxy.endCall();
      // destroy it!
      _smref.destroy();
      return true;
    }

    public override bool releasedTimerExpired(int sessionId)
    {
      _smref.destroy();
      return true;
    }

    /// <summary>
    /// If by any chance this lost event comes here: destroy State machine
    /// </summary>
    public override void onReleased()
    {
      _smref.destroy();
    }
  }
  #endregion

  #region TerminatedState
  /// <summary>
  /// Released State indicates call has been released by user and waiting for destruction.
  /// The state machine will be destroyed on onReleased event from stack or on released timer 
  /// timeout
  /// </summary>
  internal class CTerminatedState : IAbstractState
  {
    public CTerminatedState(CStateMachine sm)
      : base(sm)
    {
      Id = EStateId.TERMINATED;
    }

    /// <summary>
    /// Enter release state. If release timer not implemented release call imediately
    /// </summary>
    public override void onEntry()
    {
      bool success = _smref.startTimer(ETimerType.ERELEASED);
      if (!success) _smref.destroy();
    }

    public override void onExit()
    {
      _smref.stopAllTimers();
    }

    public override bool endCall()
    {
      CallProxy.endCall();
      return true;
    }

    public override bool releasedTimerExpired(int sessionId)
    {
      _smref.destroy();
      return true;
    }

    public override void onAlerting()
    {
      CallProxy.endCall();
    }

    public override void onConnect()
    {
      CallProxy.endCall();
    }

    /// <summary>
    /// Destroy state machine 
    /// </summary>
    public override void onReleased()
    {
      _smref.destroy();
    }
  }
  #endregion

  #region IncomingState
  /// <summary>
  /// Incoming state indicates incoming call. Check CFx and DND features. Start ringer. 
  /// </summary>
  internal class CIncomingState : IAbstractState
  {
    public CIncomingState(CStateMachine sm)
      : base(sm)
    {
      Id = EStateId.INCOMING;
    }

    public override void onEntry()
    {
      // set incoming call flags
      _smref.Incoming = true;

      int sessionId = SessionId;

      // Start no response timer
      _smref.startTimer(ETimerType.ENORESPONSE);

      CallProxy.alerted();
      _smref.Type = ECallType.EMissed;
      MediaProxy.playTone(ETones.EToneRing);

      // if CFNR active start timer
      if (_smref.Config.CFNRFlag)
      {
        _smref.startTimer(ETimerType.ENOREPLY);
      }
      // auto answer call (if single call)
      if ((_smref.Config.AAFlag == true) && (_smref.NumberOfCalls == 1))
      {
        this.acceptCall();
      }
    }

    public override void onExit()
    {
      MediaProxy.stopTone();
      _smref.stopAllTimers();
    }

    public override bool acceptCall()
    {
      _smref.Type = ECallType.EReceived;
      _smref.Time = System.DateTime.Now;

      CallProxy.acceptCall();
      _smref.changeState(EStateId.ACTIVE);
      return true;
    }

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
    }

    public override bool xferCall(string number)
    {
      // In fact this is not Tranfser. It's Deflect => redirect...
      return CallProxy.serviceRequest((int)EServiceCodes.SC_CD, number);
    }

    public override bool endCall()
    {
      _smref.changeState(EStateId.TERMINATED);
      CallProxy.endCall();
      return base.endCall();
    }

    /// <summary>
    /// No reply timer expired. Send service code to call proxy.
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public override bool noReplyTimerExpired(int sessionId)
    {
      CallProxy.serviceRequest((int)EServiceCodes.SC_CFNR, _smref.Config.CFUNumber);
      return true;
    }

    /// <summary>
    /// Response timer expired. Releasing the call...
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public override bool noResponseTimerExpired(int sessionId)
    {
      _smref.changeState(EStateId.TERMINATED);
      CallProxy.endCall();
      return true;
    }
  }
  #endregion

  #region HoldingState
  /// <summary>
  /// Holding state indicates call is hodling.
  /// </summary>
  internal class CHoldingState : IAbstractState
  {
    public CHoldingState(CStateMachine sm)
      : base(sm)
    {
      Id = EStateId.HOLDING;
    }

    public override void onEntry()
    {
    }

    public override void onExit()
    {
    }

    public override bool retrieveCall()
    {
      _smref.RetrieveRequested = true;
      CallProxy.retrieveCall();
      _smref.changeState(EStateId.ACTIVE);
      return true;
    }

    // TODO implement in stack interface
    //public override onRetrieveConfirm()
    //{
    //  if (_smref.RetrieveRequested)
    //  {
    //    _smref.changeState(EStateId.ACTIVE);
    //  }
    //  _smref.RetrieveRequested = false;
    //}

    public override void onReleased()
    {
      _smref.changeState(EStateId.RELEASED);
    }

    public override bool endCall()
    {
      CallProxy.endCall();
      _smref.changeState(EStateId.TERMINATED);
      return base.endCall();
    }
  }
  #endregion

} // namespace Sipek
