using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tekly.NavStates
{
	/// <summary>
	/// Represents a state that can be 5 modes Inactive, Loading, Active or Unloading
	/// NavStates can have NavBehaviours attached to them that also receive the same events as the state
	/// </summary>
	public class NavState : MonoBehaviour
	{
		public StateTransition[] Transitions;

		public string Name => name;

		public string Path { get; private set; }

		public NavStateMachine StateMachine { get; private set; }
		public NavStateManager Manager => m_context.Manager;
		public bool IsRoot => StateMachine == this;

		public event Action<StateMode> ModeChanged;

		public StateMode Mode {
			get => m_mode;
			private set {
				if (m_mode != value) {
					m_mode = value;
				
					ModeChanged?.Invoke(m_mode);
					m_context.NavStateModeChanged(this);
				}
			}
		}

		protected NavStateContext m_context;
		
		private NavBehaviour[] m_behaviours;
		private StateMode m_mode;

		/// <summary>
		/// Called by the manager when the state machine hierarchy is navigating
		/// </summary>
		public virtual void Initialize(NavStateContext context, NavStateMachine parent)
		{
			m_context = context;
			StateMachine = parent;

			Path = NavStateUtils.CalculatePath(this);

			m_behaviours = GetComponents<NavBehaviour>();
			
			foreach (var beh in m_behaviours) {
				beh.Initialize(context, this);
			}
		}
		
		public void HandleTransition(string transition)
		{
			m_context.Manager.HandleTransition(transition);
		}
		
		public void HandleTransition(NavState navState)
		{
			m_context.Manager.TransitionTo(navState);
		}

		public bool NeedsReenter()
		{
			return false;
		}
		
		public void StartLoading()
		{
			Mode = StateMode.Loading;
			OnStateStartLoading();
		}
		
		public void LoadingComplete()
		{
			foreach (var behaviour in m_behaviours) {
				behaviour.OnStateLoadingComplete();
			}
		}

		public void StartUnloading()
		{
			Mode = StateMode.Unloading;
			OnStateStartUnloading();
		}

		public bool IsDoneLoading()
		{
			switch (Mode) {
				case StateMode.Loading:
					if (!AreBehavioursDoneLoading()) {
						return false;
					}
					
					Mode = StateMode.Active;
					return true;
				default:
					m_context.Logger.Warning($"IsDoneLoading() was called while not in the loading mode. State: {Name}");
					return false;
			}
		}

		public bool IsDoneUnloading()
		{
			return AreBehavioursDoneUnloading();
		}

		private bool AreBehavioursDoneLoading()
		{
			foreach (var beh in m_behaviours) {
				if (!beh.IsDoneLoading()) {
					return false;
				}
			}

			return true;
		}

		private bool AreBehavioursDoneUnloading()
		{
			foreach (var beh in m_behaviours) {
				if (!beh.IsDoneUnloading()) {
					return false;
				}
			}

			return true;
		}

		public void OnStateEnter()
		{
			Mode = StateMode.Active;

			foreach (var beh in m_behaviours) {
				beh.OnStateEnter();
			}
		}

		private void OnStateStartLoading()
		{
			foreach (var beh in m_behaviours) {
				beh.OnStateStartLoading();
			}
		}

		private void OnStateStartUnloading()
		{
			foreach (var beh in m_behaviours) {
				beh.OnStateStartUnloading();
			}
		}

		public void OnStateLeave()
		{
			Mode = StateMode.Inactive;

			foreach (var beh in m_behaviours) {
				beh.OnStateLeave();
			}
		}

		public void StateUpdate(float deltaTime)
		{
			switch (Mode) {
				case StateMode.Inactive:
					break;
				case StateMode.Loading:
					OnStateLoadingUpdate(deltaTime);
					break;
				case StateMode.Active:
					OnStateActiveUpdate(deltaTime);
					break;
				case StateMode.Unloading:
					OnStateUnloadingUpdate(deltaTime);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected virtual void OnStateLoadingUpdate(float deltaTime)
		{
			foreach (var beh in m_behaviours) {
				beh.OnStateLoadingUpdate(deltaTime);
			}
		}

		protected virtual void OnStateUnloadingUpdate(float deltaTime)
		{
			foreach (var beh in m_behaviours) {
				beh.OnStateUnloadingUpdate(deltaTime);
			}
		}

		protected virtual void OnStateActiveUpdate(float deltaTime)
		{
			foreach (var beh in m_behaviours) {
				beh.OnStateActiveUpdate(deltaTime);
			}
		}

		public StateTransition GetLocalTransition(string transition)
		{
			return NavStateUtils.GetTransition(transition, Transitions);
		}

		public bool TryGetStateForTransition(string transition, ref NavState targetState)
		{
			StateTransition stateTransition = GetLocalTransition(transition);

			if (stateTransition != null) {
				targetState = stateTransition.Target;

				return true;
			}

			if (StateMachine == null || StateMachine == this) {
				return false;
			}

			return StateMachine.TryGetStateForTransition(transition, ref targetState);
		}

		public bool HandleEvent(NavStateEvent evt)
		{
			foreach (var beh in m_behaviours) {
				if (beh.OnHandleEvent(evt)) {
					return true;
				}
			}

			return false;
		}

		protected virtual bool OnHandleEvent(NavStateEvent evt)
		{
			return false;
		}
	}
}
