using Tekly.Logging;

namespace Tekly.NavStates
{
    public class NavStateContext
    {
        public readonly NavStateManager Manager;
        public readonly TkLogger Logger;
        private readonly bool m_enableDebugLogging;

        public NavStateContext(NavStateManager manager, TkLogger logger, bool enableDebugLogging)
        {
            Manager = manager;
            Logger = logger;
            m_enableDebugLogging = enableDebugLogging;
        }

        public void NavStateModeChanged(NavState state)
        {
            if (m_enableDebugLogging) {
                Logger.Info($"{state.Path} -> {state.Mode}");
            }
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
#endif
        }
    }
}