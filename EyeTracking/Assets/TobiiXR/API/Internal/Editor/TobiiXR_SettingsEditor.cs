// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using UnityEditor;
using UnityEngine;

namespace Tobii.XR
{
    [CustomEditor(typeof(TobiiXR_Settings))]

	public class TobiiXR_SettingsEditor : Editor 
	{
		public override void OnInspectorGUI()
		{
			EditorGUILayout.LabelField("Information", EditorStyles.boldLabel);

			EditorGUILayout.LabelField("TobiiXR_Description describes how to initialize TobiiXR.");

			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("To change these settings go to ");
			if(GUILayout.Button("Tobii Settings")) TobiiXR_SettingsEditorWindow.ShowWindow();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
	}
}