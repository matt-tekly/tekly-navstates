using UnityEngine;

namespace Tekly.NavStates
{
	public class NavBehaviour : MonoBehaviour
	{
		protected NavStateContext m_context;
		protected NavState m_state;

		public void Initialize(NavStateContext context, NavState parent)
		{
			m_context = context;
			m_state = parent;

			Initialize();
		}

		/// <summary>
		/// Called during the initialize of the state tree
		/// </summary>
		protected virtual void Initialize()
		{
		}

		/// <summary>
		/// Called when the state has become active. This occurs after all the states in a transition have finished
		/// loading.
		/// </summary>
		public virtual void OnStateEnter()
		{
		}

		/// <summary>
		/// Called when the state has finished unloading
		/// </summary>
		public virtual void OnStateLeave()
		{
		}

		public virtual void OnStateLoadingUpdate(float deltaTime)
		{
		}

		public virtual void OnStateUnloadingUpdate(float deltaTime)
		{
		}

		public virtual void OnStateActiveUpdate(float deltaTime)
		{
		}

		/// <summary>
		/// Is this behaviour done loading yet? The state is considered done loading when all behaviours are done.
		/// </summary>
		public virtual bool IsDoneLoading()
		{
			return true;
		}

		public virtual bool IsDoneUnloading()
		{
			return true;
		}

		public virtual void OnStateStartLoading()
		{
		}
		
		/// <summary>
		/// Called when all the NavBehaviours on the same state are done loading
		/// </summary>
		public virtual void OnStateLoadingComplete()
		{
			
		}

		public virtual void OnStateStartUnloading()
		{
		}

		public virtual bool OnHandleEvent(NavStateEvent evt)
		{
			return false;
		}

		
	}
}
