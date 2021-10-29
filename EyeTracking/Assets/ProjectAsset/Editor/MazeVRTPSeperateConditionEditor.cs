using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MazeVRTPSeperateCondition))]
public class MazeVRTPSeperateConditionEditor : Editor
{
    #region SPs
    private SerializedProperty _cameraRig;
    private SerializedProperty _maze;
    private SerializedProperty _path;
    private SerializedProperty _practiceMaze;
    private SerializedProperty _customMaze;
    private SerializedProperty _trialId;
    private SerializedProperty _sectionId;
    private SerializedProperty _currentCondition;
    private SerializedProperty _switchToCustomMaze;
    private SerializedProperty _randomGenerateConditon;
    private SerializedProperty _debug;
    private SerializedProperty _debugMazeId;
    private SerializedProperty _nextMaze;
    private SerializedProperty _usePractice;
    private SerializedProperty _conditionId;
    private SerializedProperty _frameLogger;
    private SerializedProperty _eventLogger;
    private SerializedProperty _grabgrip;
    private SerializedProperty _resetSign;
    private SerializedProperty _menu;
    private SerializedProperty _endDemoPanel;
    private SerializedProperty _resetPositionPanel;
    private SerializedProperty _controllerLaserPointerList;
    private SerializedProperty _cameraRigAngle;
    #endregion

    MazeVRTPSeperateCondition _script;
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
        _trialId = serializedObject.FindProperty("_trialId");
        _sectionId = serializedObject.FindProperty("_sectionId");
        _currentCondition = serializedObject.FindProperty("_currentCondition");
        _switchToCustomMaze = serializedObject.FindProperty("_switchToCustomMaze");
        _randomGenerateConditon = serializedObject.FindProperty("_randomGenerateConditon");
        _debug = serializedObject.FindProperty("_debug");
        _debugMazeId = serializedObject.FindProperty("_debugMazeId");
        _nextMaze = serializedObject.FindProperty("_nextMaze");
        _usePractice = serializedObject.FindProperty("_usePractice");
        _conditionId = serializedObject.FindProperty("_conditionId");
        _frameLogger = serializedObject.FindProperty("frameLogger");
        _eventLogger = serializedObject.FindProperty("eventLogger");
        _grabgrip = serializedObject.FindProperty("grabGrip");
        _resetSign = serializedObject.FindProperty("resetSign");
        _menu = serializedObject.FindProperty("menu");
        _endDemoPanel = serializedObject.FindProperty("endDemoPanel");
        _resetPositionPanel = serializedObject.FindProperty("resetPositionPanel");
        _script = (MazeVRTPSeperateCondition)target;
        _controllerLaserPointerList = serializedObject.FindProperty("controllerLaserPointerList");

        _cameraRigAngle = serializedObject.FindProperty("_cameraRigAngle");
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
            EditorGUILayout.PropertyField(_trialId);
            EditorGUILayout.PropertyField(_sectionId);
            EditorGUILayout.PropertyField(_conditionId);
            EditorGUILayout.PropertyField(_currentCondition);
            EditorGUILayout.PropertyField(_usePractice);
            EditorGUILayout.PropertyField(_cameraRig);
            EditorGUILayout.PropertyField(_frameLogger);
            EditorGUILayout.PropertyField(_eventLogger);
            EditorGUILayout.PropertyField(_grabgrip);
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
            EditorGUILayout.PropertyField(_resetSign, true);

            ++EditorGUI.indentLevel;
            EditorGUILayout.PropertyField(_menu, true);
            EditorGUILayout.PropertyField(_endDemoPanel, true);
            EditorGUILayout.PropertyField(_resetPositionPanel, true);

            EditorGUILayout.PropertyField(_controllerLaserPointerList);
            ++EditorGUI.indentLevel;
            if (_controllerLaserPointerList.isExpanded)
            {
                EditorGUILayout.PropertyField(_controllerLaserPointerList.FindPropertyRelative("Array.size"));
                for (int i = 0; i < _controllerLaserPointerList.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_controllerLaserPointerList.GetArrayElementAtIndex(i));
                }
            }
            --EditorGUI.indentLevel;
            --EditorGUI.indentLevel;

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        _showDebugSettings = EditorGUILayout.Foldout(_showDebugSettings, "Debug");
        if (_showDebugSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_randomGenerateConditon);
            EditorGUILayout.PropertyField(_switchToCustomMaze);
            EditorGUILayout.PropertyField(_nextMaze);
            EditorGUILayout.PropertyField(_debug);
            EditorGUILayout.PropertyField(_debugMazeId);

            EditorGUILayout.PropertyField(_cameraRigAngle);
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
