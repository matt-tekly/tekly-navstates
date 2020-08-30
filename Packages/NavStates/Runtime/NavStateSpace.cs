using System;
using System.Collections.Generic;
using Tekly.Logging;
using UnityEngine;

namespace Tekly.NavStates
{
	/// <summary>
	/// NavStateSpace delineates a root of a NavState tree. They can be nested inside of other NavStateSpaces.
	/// </summary>
	public class NavStateSpace : MonoBehaviour
	{
		public bool UpdateSelf = true;
		public NavStateMachine NavStateMachine;
		public NavStateManager Manager;
		public bool EnableDebugLogging;

		private static readonly List<NavStateSpace> s_spaces = new List<NavStateSpace>();

		public static NavStateSpace Get(string name)
		{
			foreach (NavStateSpace space in s_spaces) {
				if (space.name == name) {
					return space;
				}
			}

			throw new Exception($"Didn't find NavStateSpace {name}");
		}

		private void Start()
		{
			Manager = new NavStateManager();
			Manager.Initialize(NavStateMachine, CreateContext(Manager));

			s_spaces.Add(this);
		}

		private void OnDestroy()
		{
			s_spaces.Remove(this);
		}

		public void Update()
		{
			if (UpdateSelf) {
				PerformUpdate();
			}
		}

		public void PerformUpdate()
		{
			Manager.StateUpdate(Time.deltaTime);
		}

		protected virtual NavStateContext CreateContext(NavStateManager manager)
		{
			return new NavStateContext(manager, TkLogger.GetLogger<NavState>(), EnableDebugLogging);
		}
	}
}
