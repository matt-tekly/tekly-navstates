using System.Collections.Generic;
using System.Diagnostics;

namespace Tekly.NavStates
{
	public class NavStateNavigator
	{
		private readonly NavigationPlan m_navigationPlan = new NavigationPlan();

		public NavStateMachine Root { get; private set; }
		
		/// <summary>
		/// The active leaf state
		/// </summary>
		public NavState CurrentState { get; private set; }
		
		public bool IsTransitioning => !m_navigationPlan.IsComplete;

		private readonly List<NavStateMachine> m_parentsScratch = new List<NavStateMachine>();

		private NavStateContext m_context;
		
		public void Initialize(NavStateContext context, NavStateMachine rootStateMachine)
		{
			m_context = context;
			Root = rootStateMachine;

			// Create a Navigation Plan to enter the root state initially
			m_navigationPlan.Reset();
			m_navigationPlan.AddStateToEnter(rootStateMachine);
			
			// Check if the destination is a parent state, and repeatedly add the default state until we get to a leaf state.
			AddDefaultStatesToEnter(rootStateMachine);
			m_navigationPlan.NavigationStarted();

			BeginLoadingState(Root);
		}
		
		public void Update()
		{
			UpdateCurrentState();
		}

		private void UpdateCurrentState()
		{
			switch (CurrentState.Mode) {
				case StateMode.Inactive:
				case StateMode.Active:
					break;
				case StateMode.Loading:
					if (CurrentState.IsDoneLoading()) {
						CurrentState.LoadingComplete();
						m_navigationPlan.StateLoaded();

						var nextState = m_navigationPlan.CurrentStateToLoad();
						if (nextState != null) {
							BeginLoadingState(nextState);
						} else {
							// All states have been loaded
							var statesToEnter = m_navigationPlan.NavigationEnded();
							EnterStates(statesToEnter);
						}
					}
					break;
				case StateMode.Unloading:
					if (CurrentState.IsDoneUnloading()) {
						OnCurrentStateUnloadingFinished();
					}
					break;
				default:
					m_context.Logger.Error($"CurrentState.Mode out of range: {(int)CurrentState.Mode}");
					break;
			}
		}
		
		public bool CanProcessTransition(string transition)
		{
			NavState targetState = null;
			return m_navigationPlan.IsComplete && TryGetStateForTransition(transition, ref targetState);
		}

		public void Navigate(NavState targetState)
		{
			InternalNavigateTo(targetState);
		}

		public bool TryGetStateForTransition(string transition, ref NavState targetState)
		{
			return CurrentState.TryGetStateForTransition(transition, ref targetState);
		}
		
		private void InternalNavigateTo(NavState destination)
		{
			// TODO: Handle where navigation is requested while a state is loading.
			UnityEngine.Debug.Assert(m_navigationPlan.IsComplete, "Navigation was requested before navigation finished!");

			m_navigationPlan.Reset();
			m_navigationPlan.OriginState = CurrentState;

			NavStateUtils.GetParents(destination, m_parentsScratch);
			m_parentsScratch.Reverse();

			NavStateMachine parentToEnterFrom = null;

			// Determine the states to be entered
			for (var index = 0; index < m_parentsScratch.Count; index++) {
				NavStateMachine parent = m_parentsScratch[index];
				if (parentToEnterFrom == null && NavStateUtils.NeedsToBeEntered(parent)) {
					parentToEnterFrom = parent;
				}

				if (parentToEnterFrom != null) {
					m_navigationPlan.AddStateToEnter(parent);
				}
			}

			m_navigationPlan.AddStateToEnter(destination);

			// Check if the destination is a state machine and repeatedly add the default state until we get to a leaf.
			if (!AddDefaultStatesToEnter(destination)) {
				return;
			}

			parentToEnterFrom = parentToEnterFrom == null ? destination.StateMachine : parentToEnterFrom.StateMachine;

			// Determine the states that will be exited
			EnqueueStatesToLeave(parentToEnterFrom);

			m_navigationPlan.NavigationStarted();

			// Begin the unloading process
			StartUnloadingStatesToLeave();

			UpdateCurrentState();
		}

		/// <summary>
		/// Checks if the given state is a StateMachine and adds the StateMachine's default state.
		/// </summary>
		private bool AddDefaultStatesToEnter(NavState destination)
		{
			var destinationStateMachine = destination as NavStateMachine;
			while (destinationStateMachine != null) {
				var defaultChild = destinationStateMachine.DefaultState;
				if (defaultChild == null) {
					m_context.Logger.Error($"Failed to navigate to next state. {destinationStateMachine.Path} is missing the default state");
					return false;
				}

				m_navigationPlan.AddStateToEnter(defaultChild);
				destinationStateMachine = defaultChild as NavStateMachine;
			}
			return true;
		}

		private void EnterStates(NavState[] statesToEnter)
		{
			foreach (var navState in statesToEnter) {
				navState.OnStateEnter();
			}
		}

		private void BeginLoadingState(NavState destination)
		{
			CurrentState = destination;
			CurrentState.StateMachine.SetActiveChild(CurrentState);
			CurrentState.StartLoading();
			UpdateCurrentState();
		}

		private void EnqueueStatesToLeave(NavState untilParent)
		{
			NavState activeState = CurrentState;

			while (activeState != untilParent) {
				m_navigationPlan.AddStateToLeave(activeState);
				activeState = activeState.StateMachine;
			}
		}

		private void StartUnloadingStatesToLeave()
		{
			List<NavState> statesToLeave = m_navigationPlan.StatesToLeave;
			int numStatesToLeave = statesToLeave.Count;
			for (int i = 0; i < numStatesToLeave; ++i) {
				NavState stateToLeave = statesToLeave[i];
				stateToLeave.StartUnloading();
			}
		}

		private void OnCurrentStateUnloadingFinished()
		{
			m_navigationPlan.StateUnloaded();

			if (m_navigationPlan.CurrentStateToUnload() != null) {
				CurrentState = m_navigationPlan.CurrentStateToUnload();
				UpdateCurrentState(); // This will check whether it is ready to finalize unloading and continue from there.
			} else {
				// All states that need to be unloaded have been. Now we can exit them.
				LeaveStates();
				BeginLoadingState(m_navigationPlan.CurrentStateToLoad());
			}
		}

		private void LeaveStates()
		{
			List<NavState> statesToLeave = m_navigationPlan.StatesToLeave;
			int numStatesToLeave = statesToLeave.Count;
			for (int i = 0; i < numStatesToLeave; ++i) {
				NavState stateToLeave = statesToLeave[i];
				stateToLeave.OnStateLeave();
			}
		}
	}
}
