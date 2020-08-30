using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Tekly.NavStates
{
	public static class NavStateUtils
	{
		public static void GetParents(NavState state, List<NavStateMachine> parentsOut)
		{
			parentsOut.Clear();
			if (state == null) {
				return;
			}

			var parent = state.StateMachine;

			while (true) {
				parentsOut.Add(parent);
				parent = parent.StateMachine;

				if (parent.IsRoot) {
					break;
				}
			}
		}

		public static NavState[] GetChildren(NavState state)
		{
			return GetComponentsInChildrenWithoutParent(state.gameObject);
		}

		private static NavState[] GetComponentsInChildrenWithoutParent(GameObject go)
		{
			var transform = go.transform;

			var components = new List<NavState>();

			var childCount = transform.childCount;
			for (var i = 0; i < childCount; i++) {
				var child = transform.GetChild(i);

				if (child.GetComponent<NavStateSpace>() != null) {
					continue;
				}

				var navState = child.GetComponent<NavState>();
				if (navState != null) {
					components.Add(navState);
				}
			}

			return components.ToArray();
		}

		public static bool NeedsToBeEntered(NavState state)
		{
			return state.Mode == StateMode.Inactive || state.NeedsReenter();
		}

		public static StateTransition GetTransition(string transition, StateTransition[] transitions)
		{
			if (transitions == null) {
				return null;
			}

			foreach (var stateTransition in transitions) {
				if (string.Equals(stateTransition.TransitionName, transition)) {
					return stateTransition;
				}
			}

			return null;
		}

		private static readonly List<NavState> s_scratchList = new List<NavState>(16);
		private static readonly StringBuilder s_builder = new StringBuilder();
		
		public static string CalculatePath(NavState state)
		{
			s_builder.Clear();
			s_scratchList.Clear();
			
			while (true) {
				s_scratchList.Add(state);
				
				if (state.IsRoot) {
					break;
				}
				
				state = state.StateMachine;
			}
			
			s_scratchList.Reverse();

			s_builder.Append(s_scratchList[0].Name);

			for (var index = 1; index < s_scratchList.Count; index++) {
				s_builder.Append("/");
				s_builder.Append(s_scratchList[index].Name);
			}

			return s_builder.ToString();
		}
		
	}
}
