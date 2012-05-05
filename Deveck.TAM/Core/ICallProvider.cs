using System;

namespace Deveck.TAM.Core
{
	
	public delegate void IncomingCallDelegate(IIncomingCall incomingCall);
	
	/// <summary>
	/// Abstracts different call providers (SIP, Skype, analog,...)
	/// </summary>
	public interface ICallProvider : IDisposable
	{
		event IncomingCallDelegate OnIncomingCall;
		
		/// <summary>
		/// Initializes or reinitializes the call provider
		/// </summary>
		void Initialize();		
		
	}
	
	public class CallProviderException : Exception
	{
		public CallProviderException(String message, params object[] args)
			:base(String.Format(message, args))
		{			
		}
	}
}
