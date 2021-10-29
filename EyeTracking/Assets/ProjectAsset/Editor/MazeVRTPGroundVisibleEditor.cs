using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeVRTPGroundVisible))]
public class MazeVRTPGroundVisibleEditor : Editor
{
    #region SPs
    private SerializedProperty _cameraRig;
    private SerializedProperty _maze;
    private SerializedProperty _path;
    private SerializedProperty _practiceMaze;
    private SerializedProperty _customMaze;
    private SerializedProperty _userId;
    private SerializedProperty _trialId;
    private SerializedProperty _currentCondition;
    private SerializedProperty _switchToCustomMaze;
    private SerializedProperty _debug;
    private SerializedProperty _debugMazeId;
    private SerializedProperty _nextMaze;
    private SerializedProperty _usePractice;
    #endregion

    MazeVRTPGroundVisible _script;

    static bool _showParameterSettings = true;
    static bool _showMazeSettings = true;
    static bool _showDebugSettings = true;

    void OnEnable()
    {
        _cameraRig = serializedObject.FindProperty("_cameraRig");
        _maze = serializedObject.FindProperty("_maze");
        _path = serializedObject.FindProperty("_path");
        _practiceMaze = serializedObject.FindProperty("_practiceMaze");
        _customMaze = serializedObject.FindProperty("_customMaze");
        _userId = serializedObject.FindProperty("_userId");
        _trialId = serializedObject.FindProperty("_trialId");
        _currentCondition = serializedObject.FindProperty("_currentCondition");
        _switchToCustomMaze = serializedObject.FindProperty("_switchToCustomMaze");
        _debug = serializedObject.FindProperty("_debug");
        _debugMazeId = serializedObject.FindProperty("_debugMazeId");
        _nextMaze = serializedObject.FindProperty("_nextMaze");
        _usePractice = serializedObject.FindProperty("_usePractice");
        _script = (MazeVRTPGroundVisible)target;


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
            EditorGUILayout.PropertyField(_trialId);
            EditorGUILayout.PropertyField(_currentCondition);
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
            EditorGUILayout.PropertyField(_path);
            ++EditorGUI.indentLevel;
            if (_path.isExpanded)
            {
                EditorGUILayout.PropertyField(_path.FindPropertyRelative("Array.size"));
                for (int i = 0; i < _path.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_path.GetArrayElementAtIndex(i));
                }
            }
            --EditorGUI.indentLevel;
            --EditorGUI.indentLevel;

            EditorGUILayout.PropertyField(_maze);
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
            EditorGUILayout.PropertyField(_nextMaze);
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
