/*
 * Created by SharpDevelop.
 * User: test
 * Date: 01.05.2012
 * Time: 19:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Deveck.TAM.Actions;

namespace Deveck.TAM.Core
{
	
	public enum CallDirection
	{
		Incoming,
		Outgoing
	}
	public enum CallState
	{
		Ringing,
		Error,
		Connected,
		HangUp,
		Disconnected		
	}
	/// <summary>
	/// Description of IIncomingCall.
	/// </summary>
	public interface ICall
	{
		event Action<ICall> OnAudioPlaybackCompleted;
		
		/// <summary>
		/// Returns the corresponding call provider
		/// </summary>
		ICallProvider CallProvider {get;}
		
		/// <summary>
		/// Date and time of the incoming call
		/// </summary>
		DateTime Date {get;}
		
		/// <summary>
		/// State of the call
		/// </summary>
		CallState CallState {get; }
		
		CallDirection CallDirection {get;}
		
		IActionProvider ActionProvider {get;}
		
		void Hangup();
		
		void PlayAudioFile(String file);
	}
	
	public interface IIncomingCall : ICall
	{
		void AcceptCall();
	}
}
