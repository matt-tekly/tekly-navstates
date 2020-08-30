using UnityEditor;

namespace Tekly.NavStates.Editor
{
    [CustomEditor(typeof(NavStateMachine))]
    public class NavStateMachineInspector : NavStateInspector
    {

        private SerializedProperty m_defaultState;
        
        protected override void OnEnable()
        {
	        base.OnEnable();
	        m_defaultState = serializedObject.FindProperty("DefaultState");
        }

        protected override void DrawProperties()
        {
	        EditorGUILayout.PropertyField(m_defaultState);
        }
    }
}
