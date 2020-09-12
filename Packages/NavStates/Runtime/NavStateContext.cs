using Tekly.Logging;

namespace Tekly.NavStates
{
    public class NavStateContext
    {
        public readonly NavStateManager Manager;
        
        private readonly TkLogger m_logger = TkLogger.Get<NavStateContext>();
        private readonly bool m_enableDebugLogging;

        public NavStateContext(NavStateManager manager, bool enableDebugLogging)
        {
            Manager = manager;
            m_enableDebugLogging = enableDebugLogging;
        }

        public void NavStateModeChanged(NavState state)
        {
            if (m_enableDebugLogging) {
                m_logger.Info($"{state.Path} -> {state.Mode}");
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
#endif
        }
    }
}