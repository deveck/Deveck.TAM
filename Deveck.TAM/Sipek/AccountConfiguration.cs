/*
 * Created by SharpDevelop.
 * User: Andreas Reiter <development@wiffzack.com>
 * Date: 24.04.2012
 * Time: 21:41
 * 
 * This file is part of GastroboniSQL (www.wiffzack.com)
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Sipek.Common;

namespace Deveck.TAM.Sipek
{
	/// <summary>
	/// Description of AccountConfiguration.
	/// </summary>
	public class AccountConfiguration
	{
		public static AccountConfiguration ReadAccountConfiguration()
		{
			if(!File.Exists("accounts.xml"))
				throw new FileNotFoundException("Cannot find accounts.xml");

			XmlDocument doc = new XmlDocument();
			doc.Load("accounts.xml");			
			return new AccountConfiguration(doc.DocumentElement);
		}
		
		
		private List<IAccount> _accounts = new List<IAccount>();
		private AccountConfiguration(XmlElement doc)
		{
			int i = 0;
			foreach(XmlElement element in doc.SelectNodes("sipekAccount"))
			{
				_accounts.Add(new XmlAccount(element, i));
				i++;
			}
		}
		
		public List<IAccount> Accounts
		{
			get{ return _accounts; }
		}
	}
	
	public class XmlAccount : IAccount
	{		
		private XmlElement _accountRoot;
		private int _index;
		private int _regstate;
		
		public XmlAccount(XmlElement accountRoot, int index)
		{
			_index = index;
			_accountRoot = accountRoot;	
		}
		
		public int Index {
			get { return _index;}
			set { _index = value; }
		}
		
		public string AccountName {
			get { return XmlHelper.ReadString(_accountRoot, "displayName");}
			set { }
		}
		
		public string HostName {
			get { return XmlHelper.ReadString(_accountRoot, "registrar");}
			set {	}
		}
		
		public string Id {
			get { return UserName; }
			set { }
		}
		
		public string UserName {
			get {
				return XmlHelper.ReadString(_accountRoot, "username");
			}
			set {}
		}
		
		public string Password {
			get { return XmlHelper.ReadString(_accountRoot, "password");}
			set {}
		}
		
		public string DisplayName {
			get { return AccountName; }
			set {}
		}
		
		public string DomainName {
			get { return XmlHelper.ReadString(_accountRoot, "realm"); }
			set { }
		}
		
		public int RegState {
			get {	return _regstate; }
			set { _regstate = value; }
		}
		
		public string ProxyAddress {
			get { return ""; }
			set { }
		}
		
		public ETransportMode TransportMode {
			get { return  XmlHelper.ReadEnum<ETransportMode>(_accountRoot, "transport", ETransportMode.TM_UDP); }
			set {}
		}
		
		public override string ToString()
		{
			return string.Format("[XmlAccount: DisplayName={0} AccountName={1} Id={2} Domain={3} Host={4} Index={5} Proxy={6} Transport={7}]",
			                     DisplayName, AccountName, Id, DomainName, HostName, Index, ProxyAddress, TransportMode);
		}

	}
}
