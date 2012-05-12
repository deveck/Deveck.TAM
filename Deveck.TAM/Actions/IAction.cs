using System;
using Deveck.TAM.Core;

namespace Deveck.TAM.Actions
{
	public interface IAction
	{
		void Execute(ICall call);
	}
}
