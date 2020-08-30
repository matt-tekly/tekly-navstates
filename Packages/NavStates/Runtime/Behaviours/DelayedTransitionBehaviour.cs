namespace Tekly.NavStates.Behaviours
{
	/// <summary>
	/// Invokes the Transition after the state has been active for DelayTime
	/// </summary>
	public class DelayedTransitionBehaviour : NavBehaviour
	{
		public float DelayTime = 1.0f;
		public string Transition;

		private float m_timer;

		public override void OnStateEnter()
		{
			m_timer = DelayTime;
		}

		public override void OnStateActiveUpdate(float deltaTime)
		{
			m_timer -= deltaTime;

			if (m_timer <= 0) {
				m_state.HandleTransition(Transition);
			}
		} 
	}
}
