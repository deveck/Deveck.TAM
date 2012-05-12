using System;
using Deveck.TAM.Core;

namespace Deveck.TAM.Actions
{
	public class HangupAction : IAction
	{
		public HangupAction()
		{
		}
		
		public void Execute(ICall call)
		{
			call.Hangup();
		}
	}
}
