namespace Tekly.NavStates.Behaviours
{
	/// <summary>
	/// Ensures a minimum load time for the state.
	/// </summary>
    public class MinimumLoadTimeBehaviour : NavBehaviour
    {
        public float MinimumLoadTime = 2.0f;
        
        private float m_timer;

        public override void OnStateStartLoading()
        {
            m_timer = MinimumLoadTime;
        }

        public override void OnStateLoadingUpdate(float deltaTime)
        {
            m_timer -= deltaTime;
        }

        public override bool IsDoneLoading()
        {
	        return m_timer <= 0;
        }
    }
}