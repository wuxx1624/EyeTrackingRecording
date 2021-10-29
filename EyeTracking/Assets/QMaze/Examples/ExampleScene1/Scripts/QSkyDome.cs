using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace qtools.qmaze.example1
{
	[ExecuteInEditMode]
	public class QSkyDome : MonoBehaviour 
	{
		public Transform domeCameraTransform;

		void LateUpdate()
		{
			if (Application.isPlaying)
			{
				if (domeCameraTransform != null) 
					transform.position = domeCameraTransform.position;
			}
		}

		#if UNITY_EDITOR
		void OnRenderObject  () 
		{
			if (SceneView.lastActiveSceneView != null && !Application.isPlaying)		
			{
				Camera camera = SceneView.lastActiveSceneView.camera;			
				if (camera != null) transform.position = camera.transform.position;
			}
		}
		#endif
	}
}