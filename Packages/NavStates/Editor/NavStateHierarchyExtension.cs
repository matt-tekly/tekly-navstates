using UnityEditor;
using UnityEngine;

namespace Tekly.NavStates
{
	[InitializeOnLoad]
	public static class NavStateHierarchyExtension
	{
		static NavStateHierarchyExtension()
		{
			EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchyItem;
		}

		private static void OnDrawHierarchyItem(int instanceId, Rect selectionRect)
		{
			var go = EditorUtility.InstanceIDToObject(instanceId) as GameObject;

			if (go == null) {
				return;
			}

			var navState = go.GetComponent<NavState>();

			if (navState == null) {
				return;
			}
			
			var content = EditorGUIUtility.ObjectContent(navState, typeof(NavState));

			var imageRect = selectionRect;
			imageRect.xMin = selectionRect.xMax - 75;
			imageRect.yMin += 1;
			imageRect.width = 14;
			imageRect.height = 14;
			GUI.DrawTexture(imageRect, content.image);

			var labelRect = selectionRect;
			labelRect.xMin = selectionRect.xMax - 59;
			GUI.Label(labelRect, navState.Mode.ToString());
		}
	}
}