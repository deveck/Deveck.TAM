/*
 * Created by SharpDevelop.
 * User: dev
 * Date: 12.05.2012
 * Time: 02:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Deveck.TAM.Core;

namespace Deveck.TAM.Actions
{
	/// <summary>
	/// Description of AcceptCallAction.
	/// </summary>
	public class AcceptCallAction : IAction
	{
		public AcceptCallAction()
		{
		}
		
		public void Execute(ICall call)
		{
			if(typeof(IIncomingCall).IsAssignableFrom(call.GetType()))
				((IIncomingCall)call).AcceptCall();
		}
	}
}
