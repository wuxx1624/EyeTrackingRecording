using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace qtools.qmaze
{
	public class QInspectorUtils 
	{	
		public static T getAsset<T>(string filter) where T: Object
		{
			string[] guids = AssetDatabase.FindAssets(filter);
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				if (path.Contains("QMaze")) return AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
			}
			return null;
		}

		public static void SetIcon(Object obj, Texture2D icon) 
		{
			System.Type typeOfEditorGUIUtility = typeof(EditorGUIUtility);
			MethodInfo setIconForObjectMethod = typeOfEditorGUIUtility.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
			setIconForObjectMethod.Invoke(null, new object[] { obj, icon });			
		}
	}
}