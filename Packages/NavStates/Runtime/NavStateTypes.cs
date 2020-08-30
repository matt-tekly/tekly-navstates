using System;

namespace Tekly.NavStates
{
	public enum StateMode
	{
		Inactive,
		Loading,
		Active,
		Unloading
	}
	
	[Serializable]
	public class StateTransition
	{
		public NavState Target;
		public string TransitionName;
	}
	
	public class NavStateEvent
	{

	}
}
