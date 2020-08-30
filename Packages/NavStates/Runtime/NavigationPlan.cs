using System.Collections.Generic;

namespace Tekly.NavStates
{
	public class NavigationPlan
	{
		public readonly List<NavState> StatesToEnter = new List<NavState>();
		public readonly List<NavState> StatesToLeave = new List<NavState>();
		
		public bool IsComplete { get; private set; } 
		public NavState OriginState { get; set; }

		private int m_currentStateIndexToUnload;
		private int m_currentStateIndexToLoad;
		
		public NavigationPlan()
		{
			IsComplete = true;
		}

		public void NavigationStarted()
		{
			IsComplete = false;
			m_currentStateIndexToLoad = 0;
			m_currentStateIndexToUnload = 0;
		}

		public NavState[] NavigationEnded()
		{
			IsComplete = true;
			return StatesToEnter.ToArray();
		}

		public void Reset()
		{
			IsComplete = true;
			StatesToEnter.Clear();
			StatesToLeave.Clear();
		}

		public void AddStateToEnter(NavState navState)
		{
			StatesToEnter.Add(navState);
		}
		
		public void AddStateToLeave(NavState navState)
		{
			StatesToLeave.Add(navState);
		}

		public NavState CurrentStateToLoad()
		{
			if (m_currentStateIndexToLoad < StatesToEnter.Count) {
				return StatesToEnter[m_currentStateIndexToLoad];
			}
			return null;
		}

		public NavState CurrentStateToUnload()
		{
			if (m_currentStateIndexToUnload < StatesToLeave.Count) {
				return StatesToLeave[m_currentStateIndexToUnload];
			}
			return null;
		}

		public void StateLoaded()
		{
			if (m_currentStateIndexToLoad < StatesToEnter.Count) {
				++m_currentStateIndexToLoad;
			}
		}

		public void StateUnloaded()
		{
			m_currentStateIndexToUnload++;
		}
	}
}
