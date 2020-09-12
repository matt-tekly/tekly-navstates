using Tekly.Logging;

namespace Tekly.NavStates.Behaviours
{
	/// <summary>
	/// Perform the given Transition once the state is active for a period of time
	/// </summary>
    public class AutoTransitionBehaviour : MinimumLoadTimeBehaviour
    {
        public string Transition;
        public float SecondsToWait;

        private float m_waitTimer;
        
        private readonly TkLogger m_logger = TkLogger.Get<AutoTransitionBehaviour>();
        
        public override void OnStateEnter()
        {
	        base.OnStateEnter();
	        m_waitTimer = SecondsToWait;
        }

        public override void OnStateActiveUpdate(float deltaTime)
        {
	        if (!ValidateTransition()) {
		        return;
	        }

	        m_waitTimer -= deltaTime;

	        if (m_waitTimer <= 0) {
		        m_state.HandleTransition(Transition);    
	        }
        }
        
        private void Awake()
        {
	        ValidateTransition();
        }

        private bool ValidateTransition()
        {
	        if (!string.IsNullOrEmpty(Transition)) {
		        return true;
	        }

	        m_logger.ErrorContext("AutoTransitionBehaviour has empty Transition", this);
	        return false;
        }
    }
}