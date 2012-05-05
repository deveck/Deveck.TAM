/* 
 * Copyright (C) 2008 Sasa Coh <sasacoh@gmail.com>
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
 * WaveLib library sources http://www.codeproject.com/KB/graphics/AudioLib.aspx
 * 
 * Visit SipekSDK page at http://voipengine.googlepages.com/
 * 
 * Visit SIPek's home page at http://sipekphone.googlepages.com/ 
 * 
 */
 
using System;
using System.Collections.Generic;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;

using Deveck.TAM.Sipek.Properties;
using Sipek.Common;
using Sipek.Common.CallControl;
using Sipek.Sip;

namespace Deveck.TAM.Sipek
{
  /// <summary>
  /// ConcreteFactory 
  /// Implementation of AbstractFactory. 
  /// </summary>
  public class SipekResources : AbstractFactory
  {
    //IMediaProxyInterface _mediaProxy = new CMediaPlayerProxy();
    ICallLogInterface _callLogger = new Calllog();
    pjsipStackProxy _stackProxy = pjsipStackProxy.Instance;
    SipekConfigurator _config = new SipekConfigurator();

    #region Constructor
    public SipekResources()
    {
      // initialize sip struct at startup
      //SipConfigStruct.Instance.stunServer = this.Configurator.StunServerAddress;
      //SipConfigStruct.Instance.publishEnabled = this.Configurator.PublishEnabled;
      //SipConfigStruct.Instance.expires = this.Configurator.Expires;
      //SipConfigStruct.Instance.VADEnabled = this.Configurator.VADEnabled;
      //SipConfigStruct.Instance.ECTail = this.Configurator.ECTail;
      //SipConfigStruct.Instance.nameServer = this.Configurator.NameServer;

      // initialize modules
      _callManager.StackProxy = _stackProxy;
      _callManager.Config = _config;
      _callManager.Factory = this;
      //_callManager.MediaProxy = _mediaProxy;
      _stackProxy.Config = _config;
      _registrar.Config = _config;
      _messenger.Config = _config;

    }
    #endregion Constructor

    #region AbstractFactory methods
    public ITimer createTimer()
    {
      return new Timer();
    }

    public IStateMachine createStateMachine()
    {
      // TODO: check max number of calls
      return new CStateMachine();
    }

    #endregion

    #region Other Resources
    public pjsipStackProxy StackProxy
    {
      get { return _stackProxy; }
      set { _stackProxy = value; }
    }

    public SipekConfigurator Configurator
    {
      get { return _config; }
      set {}
    }

    // getters
    public IMediaProxyInterface MediaProxy
    {
      get { return null; }
      set { }
    }

    public ICallLogInterface CallLogger
    {
      get { return _callLogger; }
      set { }
    }

    private IRegistrar _registrar = pjsipRegistrar.Instance;
    public IRegistrar Registrar
    {
      get { return _registrar; }
    }

    private IPresenceAndMessaging _messenger = pjsipPresenceAndMessaging.Instance;
    public IPresenceAndMessaging Messenger
    {
      get { return _messenger; }
    }

    private CCallManager _callManager = CCallManager.Instance;
    public CCallManager CallManager
    {
      get { return CCallManager.Instance; }
    }
    #endregion
  }

  #region Concrete implementations

  public class Timer : ITimer
  {
    private System.Timers.Timer _timer;


    public Timer()
    {
      _timer = new System.Timers.Timer();
      if (this.Interval > 0) _timer.Interval = this.Interval;
      _timer.Interval = 100;
      _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Tick);
      _timer.Enabled = true;
    }

    void _timer_Tick(object sender, EventArgs e)
    {
    	_timer.Stop();
    	_elapsed(sender, e);
    }

    public bool Start()
    {
      _timer.Start();
      return true;
    }

    public bool Stop()
    {
      _timer.Stop();
      return true;
    }

    private int _interval;
    public int Interval
    {
      get { return _interval; }
      set { _interval = value; _timer.Interval = value; }
    }

    private TimerExpiredCallback _elapsed;
    public TimerExpiredCallback Elapsed
    {
      set { 
        _elapsed = value;
      }
    }
  }
 
  /// <summary>
  /// 
  /// </summary>
  public class SipekConfigurator : IConfiguratorInterface
  {
  	
  	private AccountConfiguration _accountConfig = AccountConfiguration.ReadAccountConfiguration();
  	
    public bool IsNull { get { return false; } }

    public bool CFUFlag {
      get { return Settings.Default.cfgCFUFlag; }
      set { Settings.Default.cfgCFUFlag = value; }
    }
    public string CFUNumber 
    {
      get { return Settings.Default.cfgCFUNumber; }
      set { Settings.Default.cfgCFUNumber = value; }
    }
    public bool CFNRFlag 
    {
      get { return Settings.Default.cfgCFNRFlag; }
      set { Settings.Default.cfgCFNRFlag = value; }
    }  

    public string CFNRNumber
    {
      get { return Settings.Default.cfgCFNRNumber; }
      set { Settings.Default.cfgCFNRNumber = value; }
    }
    public bool DNDFlag {
      get { return Settings.Default.cfgDNDFlag; }
      set { Settings.Default.cfgDNDFlag = value; }
    }
    public bool AAFlag {
      get { return Settings.Default.cfgAAFlag; }
      set { Settings.Default.cfgAAFlag = value; }
    }

    public bool CFBFlag
    {
      get { return Settings.Default.cfgCFBFlag; }
      set { Settings.Default.cfgCFBFlag = value; }
    }

    public string CFBNumber
    {
      get { return Settings.Default.cfgCFBNumber; }
      set { Settings.Default.cfgCFBNumber = value; }
    }

    public int SIPPort
    {
      get { return Settings.Default.cfgSipPort; }
      set { Settings.Default.cfgSipPort = value; }
    }

    public bool PublishEnabled
    {
      get {
        SipConfigStruct.Instance.publishEnabled = Settings.Default.cfgSipPublishEnabled;
        return Settings.Default.cfgSipPublishEnabled;
      }
      set {
        SipConfigStruct.Instance.publishEnabled = value;
        Settings.Default.cfgSipPublishEnabled = value;
      }    
    }

    public string StunServerAddress
    {
      get
      {
        SipConfigStruct.Instance.stunServer = Settings.Default.cfgStunServerAddress;
        return Settings.Default.cfgStunServerAddress;
      }
      set
      {
        Settings.Default.cfgStunServerAddress = value;
        SipConfigStruct.Instance.stunServer = value;
      }
    }

    public EDtmfMode DtmfMode
    {
      get
      {
        return (EDtmfMode)Settings.Default.cfgDtmfMode;
      }
      set
      {
        Settings.Default.cfgDtmfMode = (int)value;
      }
    }

    public int Expires
    {
      get
      {
        SipConfigStruct.Instance.expires = Settings.Default.cfgRegistrationTimeout;
        return Settings.Default.cfgRegistrationTimeout;
      }
      set
      {
        Settings.Default.cfgRegistrationTimeout = value;
        SipConfigStruct.Instance.expires = value;
      }
    }

    public int ECTail
    {
      get
      {
        SipConfigStruct.Instance.ECTail = Settings.Default.cfgECTail;
        return Settings.Default.cfgECTail;
      }
      set
      {
        Settings.Default.cfgECTail = value;
        SipConfigStruct.Instance.ECTail = value;
      }
    }

    public bool VADEnabled
    {
      get
      {
        SipConfigStruct.Instance.VADEnabled = Settings.Default.cfgVAD;
        return Settings.Default.cfgVAD;
      }
      set
      {
        Settings.Default.cfgVAD = value;
        SipConfigStruct.Instance.VADEnabled = value;
      }
    }


    public string NameServer
    {
      get
      {
        SipConfigStruct.Instance.nameServer = Settings.Default.cfgNameServer;
        return Settings.Default.cfgNameServer;
      }
      set
      {
        Settings.Default.cfgNameServer = value;
        SipConfigStruct.Instance.nameServer = value;
      }
    }

    public void Save()
    {
      // save properties
      Settings.Default.Save();
    }

    public List<string> CodecList
    {
      get 
      {
        List<string> codecList = new List<string>();
        foreach (string item in Settings.Default.cfgCodecList)
        {
          codecList.Add(item);
        }
        return codecList; 
      }
      set 
      {
        Settings.Default.cfgCodecList.Clear();
        List<string> cl = value;
        foreach (string item in cl)
        {
          Settings.Default.cfgCodecList.Add(item);
        }
      }
    }
    
  	
	public int DefaultAccountIndex {
		get { return 0; }
	}
  	
	public List<IAccount> Accounts {
		get { return _accountConfig.Accounts; }
	}
    
    
    public IAccount FindAccountById(String accountId)
    {
    	foreach(IAccount account in Accounts)
    	{
    		if(account.Id.Equals(accountId))
    			return account;
    	}
    	
    	return null;
    }
  } 

  #endregion Concrete Implementations

}
