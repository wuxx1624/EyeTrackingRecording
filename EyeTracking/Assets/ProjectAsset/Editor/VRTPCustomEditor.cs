using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRTPCustom))]
public class VRTPCustomEditor : Editor
{
    #region SPs
    private SerializedProperty _labModel;
    private SerializedProperty _cameraEye;
    private SerializedProperty _clliderModel;

    private SerializedProperty _velocitySmoothing;
    private SerializedProperty _velocityMin;
    private SerializedProperty _velocityMax;
    private SerializedProperty _velocityStrength;

    private SerializedProperty _physicalCoverageMin;
    private SerializedProperty _maxDistance;
    private SerializedProperty _minDistance;
    private SerializedProperty _distanceSmoothing;
    private SerializedProperty _cageVanishSpeed;

    private SerializedProperty _restrictorChangePercent;

    private SerializedProperty _useEyeTracking;
    #endregion

    #region Labels
    const string MODEL_ONLY = "Use Physical Model Only";

    static readonly GUIContent _modelOnlyLabel = new GUIContent(MODEL_ONLY, "Only use physical model without color background?\nHelps players with strong sim-sickness.");
    #endregion

    VRTPCustom _script;

    static bool _showVelocitySettings = true;
    static bool _showModelSettings = true;
    static bool _showEffectSettings = true;

    void OnEnable()
    {
        _labModel = serializedObject.FindProperty("_labModel");
        _cameraEye = serializedObject.FindProperty("_cameraEye");
        _clliderModel = serializedObject.FindProperty("_clliderModel");

        _velocitySmoothing = serializedObject.FindProperty("_velocitySmoothing");
        _velocityMin = serializedObject.FindProperty("_velocityMin");
        _velocityMax = serializedObject.FindProperty("_velocityMax");
        _velocityStrength = serializedObject.FindProperty("_velocityStrength");

        _physicalCoverageMin = serializedObject.FindProperty("_physicalCoverageMin");

        _maxDistance = serializedObject.FindProperty("_maxDistance");
        _minDistance = serializedObject.FindProperty("_minDistance");
        _distanceSmoothing = serializedObject.FindProperty("_distanceSmoothing");
        _cageVanishSpeed = serializedObject.FindProperty("_cageVanishSpeed");

        _restrictorChangePercent = serializedObject.FindProperty("restrictorChangePercent");

        _useEyeTracking = serializedObject.FindProperty("_useEyeTracking");
        _script = (VRTPCustom)target;


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
        
        _showModelSettings = EditorGUILayout.Foldout(_showModelSettings, "Model Settings");
        if (_showModelSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_labModel);
            EditorGUILayout.PropertyField(_cameraEye);
            EditorGUILayout.PropertyField(_clliderModel);
            EditorGUILayout.EndVertical();

            if (_script._labModel == null)
            {
                EditorGUILayout.HelpBox("No 3d model specified!", MessageType.Error);
            }

            if (_script._cameraEye == null)
            {
                EditorGUILayout.HelpBox("No camera eye specified!", MessageType.Error);
            }

            if (_script._clliderModel == null)
            {
                EditorGUILayout.HelpBox("No RayCasting Collider specified!", MessageType.Error);
            }
        }

        _showEffectSettings = EditorGUILayout.Foldout(_showEffectSettings, "Effect Settings");
        if (_showEffectSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.PropertyField(_restrictorChangePercent);
            _script.restrictorMode = (VRTPCustom.RestrictorMode)EditorGUILayout.EnumPopup("Restrictor Mode", _script.restrictorMode);
            _script.motionMode = (VRTPCustom.MotionMode)EditorGUILayout.EnumPopup("Motion Mode", _script.motionMode);

            if (_script.restrictorMode == VRTPCustom.RestrictorMode.Both)
            {
                ++EditorGUI.indentLevel;
                EditorGUILayout.PropertyField(_maxDistance);
                EditorGUILayout.PropertyField(_minDistance);
                EditorGUILayout.PropertyField(_distanceSmoothing);
                EditorGUILayout.PropertyField(_cageVanishSpeed);
                EditorGUILayout.PropertyField(_physicalCoverageMin);
                --EditorGUI.indentLevel;
            }
            else if (_script.restrictorMode == VRTPCustom.RestrictorMode.Physical)
                EditorGUILayout.PropertyField(_physicalCoverageMin);

            EditorGUILayout.PropertyField(_useEyeTracking);
            EditorGUILayout.EndVertical();
        }

        if (_script.restrictorMode != VRTPCustom.RestrictorMode.Virtual)
        {
            _showVelocitySettings = EditorGUILayout.Foldout(_showVelocitySettings, "Physical Velocity Settings");
            if (_showVelocitySettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.PropertyField(_velocitySmoothing);
                EditorGUILayout.PropertyField(_velocityMin);
                EditorGUILayout.PropertyField(_velocityMax);
                EditorGUILayout.PropertyField(_velocityStrength);
                EditorGUILayout.EndVertical();
            }
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
