using System.Collections.Generic;

namespace Tekly.NavStates
{
	public class NavStateManager
	{
		public NavState Active => m_root.ActiveState;
		public NavState LastActiveState { get; private set; }
		
		public NavStateNavigator Navigator { get; } = new NavStateNavigator();
		public NavStateContext Context { get; private set; }
		
		private NavStateMachine m_root;

		public void Initialize(NavStateMachine rootStateMachine, NavStateContext context)
		{
			Context = context;

			m_root = rootStateMachine;
			m_root.Initialize(Context, m_root);

			Navigator.Initialize(context, m_root);
		}

		public void StateUpdate(float deltaTime)
		{
			Navigator.Update();

			m_root.StateUpdate(deltaTime);
		}

		public void HandleTransition(string transition)
		{
			NavState targetState = null;
			if (Navigator.TryGetStateForTransition(transition, ref targetState)){
				TransitionTo(targetState);
			} else {
				Context.Logger.Error($"Failed to find transition: {transition} [{Active.Path}]");
			}
		}
		
		public void TransitionTo(NavState targetState)
		{
			LastActiveState = Active;
			Navigator.Navigate(targetState);
		}

		public bool HandleEvent(NavStateEvent evt)
		{
			var state = Active;
			while (state != null) {
				if (state.HandleEvent(evt)) {
					return true;
				}
				state = state.StateMachine;
			}

			return false;
		}
		
		public void GetValidTransitions(List<string> transitions)
		{
			var state = Active;

			while (state != null) {
				foreach (var transition in state.Transitions) {
					transitions.Add(transition.TransitionName);
				}
				
				if (state.StateMachine == state || state.StateMachine == null) {
					break;
				}

				state = state.StateMachine;
			}
		}
	}
}
