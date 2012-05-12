using System;
using Deveck.TAM.Core;

namespace Deveck.TAM.Triggers
{
	public interface ITrigger
	{
		string Name{get;}
		bool IsTriggered(ICall call, DateTime triggerDate);
	}
}
