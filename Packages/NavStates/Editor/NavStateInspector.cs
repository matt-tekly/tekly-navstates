using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Tekly.NavStates.Editor
{
	[CustomEditor(typeof(NavState))]
	public class NavStateInspector : UnityEditor.Editor
	{
		private const float RECT_HEIGHT = 18;
		private const float PADDING = 2;

		private SerializedProperty m_transitions;
		private ReorderableList m_transitionList;

		private NavState m_state;

		private string m_transition;
		private List<string> m_validTransitions = new List<string>();

		protected virtual void OnEnable()
		{
			m_state = serializedObject.targetObject as NavState;
			
			m_transitions = serializedObject.FindProperty("Transitions");
			m_transitionList = new ReorderableList(serializedObject, m_transitions, true, false, true, true);

			m_transitionList.drawElementCallback = DrawElement;
			m_transitionList.drawHeaderCallback = DrawHeader;
		}

		private void DrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, "Transitions");
		}

		private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			rect.yMin += PADDING;
			rect.height = RECT_HEIGHT;

			SerializedProperty transition = m_transitions.GetArrayElementAtIndex(index);
			SerializedProperty transitionName = transition.FindPropertyRelative("TransitionName");
			SerializedProperty transitionTarget = transition.FindPropertyRelative("Target");

			Rect transitionNameRect = rect;
			transitionNameRect.width = rect.width * 0.45f - 2f;

			Rect targetRect = rect;
			targetRect.xMin += rect.width * 0.45f + 2f;

			EditorGUI.PropertyField(transitionNameRect, transitionName, GUIContent.none, false);
			EditorGUI.PropertyField(targetRect, transitionTarget, GUIContent.none, false);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			DrawProperties();

			DrawTransitions();

			DrawDebug();

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void DrawProperties()
		{
			
		}

		private void DrawActiveState()
		{
			if (EditorApplication.isPlaying) {
				GUI.enabled = false;
				EditorGUILayout.ObjectField("Active State", m_state.StateMachine.ActiveState, typeof(NavState), true);
				GUI.enabled = true;
			}
		}
		
		private void DrawTransitions()
		{
			GUI.enabled = !EditorApplication.isPlaying; 
			m_transitionList.DoLayoutList();
			GUI.enabled = true;
		}

		private void DrawDebug()
		{
			if (!EditorApplication.isPlaying) {
				return;
			}

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Label("Debug", EditorStyles.boldLabel);
			
			DrawActiveState();
			
			m_validTransitions.Clear();
			
			m_state.Manager.GetValidTransitions(m_validTransitions);
		
			if (m_validTransitions.Count > 0) {
				EditorGUILayout.BeginHorizontal();
				
				var content = m_validTransitions.Select(x => new GUIContent(x)).ToArray();
				int index = m_validTransitions.IndexOf(m_transition);
				index = index < 0 ? 0 : index;
				index = EditorGUILayout.Popup(new GUIContent("Transition"), index, content);
				m_transition = m_validTransitions[index];

				if (GUILayout.Button("Go", GUILayout.Width(30))) {
					m_state.HandleTransition(m_transition);
				}
				
				EditorGUILayout.EndHorizontal();
			} else {
				GUILayout.Label("No Valid Transitions");
			}

			if (GUILayout.Button("Transition To This")) {
				m_state.HandleTransition(m_state);
			}
			
			
			EditorGUILayout.EndVertical();
		}
	}

}
