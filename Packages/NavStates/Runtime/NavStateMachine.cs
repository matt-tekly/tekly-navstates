using UnityEngine;

namespace Tekly.NavStates
{
	public class NavStateMachine : NavState
	{
		private NavState m_activeChild;

		public NavState DefaultState;

		/// <summary>
		/// Returns the active leaf state
		/// </summary>
		public NavState ActiveState {
			get {
				if (Mode == StateMode.Loading || Mode == StateMode.Unloading) {
					return this;
				}
				NavStateMachine stateMachine = m_activeChild as NavStateMachine;
				return stateMachine != null ? stateMachine.ActiveState : m_activeChild;
			}
		}

		/// <summary>
		/// The Navigator will set the active child during navigation
		/// </summary>
		public void SetActiveChild(NavState state)
		{
			m_activeChild = state;
		}

		/// <summary>
		/// Called by the manager when the state machine hierarchy is initializing
		/// </summary>
		public override void Initialize(NavStateContext context, NavStateMachine stateMachine)
		{
			base.Initialize(context, stateMachine);

			NavState[] states = NavStateUtils.GetChildren(this);

			foreach (var state in states) {
				state.Initialize(m_context, this);
			}
		}

		protected override void OnStateActiveUpdate(float deltaTime)
		{
			base.OnStateActiveUpdate(deltaTime);

			if (m_activeChild != null) {
				m_activeChild.StateUpdate(deltaTime);
			}
		}
		
#if UNITY_EDITOR
		private void OnValidate()
		{
			if (DefaultState == null) {
				Debug.LogError($"{name} NavStateMachine doesn't have default state", gameObject);
			}
		}
#endif
	}

}

