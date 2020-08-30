using UnityEngine;

namespace Tekly.NavStates
{
	public class NavStateSpaceController : NavBehaviour
	{
		public NavStateSpace NavStateSpace;

		public override void OnStateActiveUpdate(float deltaTime)
		{
			NavStateSpace.PerformUpdate();
		}
		
#if UNITY_EDITOR
		public void OnValidate()
		{
			if (NavStateSpace == null) {
				return;
			}

			if (NavStateSpace.UpdateSelf) {
				Debug.LogError("NavStateSpaceController is updating a NavStateSpace that has UpdateSelf enabled", gameObject);
			}
		}
#endif
	}
}
