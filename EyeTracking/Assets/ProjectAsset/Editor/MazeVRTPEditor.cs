using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeVRTP2))]
public class MazeVRTPEditor : Editor
{
    #region SPs
    private SerializedProperty _cameraRig;
    private SerializedProperty _maze;
    private SerializedProperty _practiceMaze;
    private SerializedProperty _customMaze;
    private SerializedProperty _userId;
    private SerializedProperty _switchToCustomMaze;
    private SerializedProperty _debug;
    private SerializedProperty _debugMazeId;
    private SerializedProperty _usePractice;
    #endregion

    MazeVRTP2 _script;

    static bool _showParameterSettings = true;
    static bool _showMazeSettings = true;
    static bool _showDebugSettings = true;

    void OnEnable()
    {
        _cameraRig = serializedObject.FindProperty("_cameraRig");
        _maze = serializedObject.FindProperty("_maze");
        _practiceMaze = serializedObject.FindProperty("_practiceMaze");
        _customMaze = serializedObject.FindProperty("_customMaze");
        _userId = serializedObject.FindProperty("_userId");
        _switchToCustomMaze = serializedObject.FindProperty("_switchToCustomMaze");
        _debug = serializedObject.FindProperty("_debug");
        _debugMazeId = serializedObject.FindProperty("_debugMazeId");
        _usePractice = serializedObject.FindProperty("_usePractice");
        _script = (MazeVRTP2)target;


        EditorApplication.update += OnEditorUpdate;
    }

    void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    void OnEditorUpdate()
    {
        Repaint();
    }

    public override void OnInspectorGUI()
    {
        // Draw header
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Space();

        _showParameterSettings = EditorGUILayout.Foldout(_showParameterSettings, "Parameter Settings");
        if (_showParameterSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_userId);
            EditorGUILayout.PropertyField(_usePractice);
            EditorGUILayout.PropertyField(_cameraRig);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        _showMazeSettings = EditorGUILayout.Foldout(_showMazeSettings, "Maze Settings");
        if (_showMazeSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            ++EditorGUI.indentLevel;
            EditorGUILayout.PropertyField(_maze);
            ++EditorGUI.indentLevel;
            if (_maze.isExpanded)
            {
                EditorGUILayout.PropertyField(_maze.FindPropertyRelative("Array.size"));
                for (int i = 0; i < _maze.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_maze.GetArrayElementAtIndex(i));
                }
            }
            --EditorGUI.indentLevel;
            --EditorGUI.indentLevel;

            EditorGUILayout.PropertyField(_practiceMaze);
            EditorGUILayout.PropertyField(_customMaze);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        _showDebugSettings = EditorGUILayout.Foldout(_showDebugSettings, "Debug");
        if (_showDebugSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_switchToCustomMaze);
            EditorGUILayout.PropertyField(_debug);
            EditorGUILayout.PropertyField(_debugMazeId);
            
            EditorGUILayout.EndVertical();
        }
        // Draw content
        EditorGUILayout.Space();
        // Finalise
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
