using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tobii.XR.GazeModifier
{
    public class GazeModifierSettingsExtension : ISettingsEditorExtension
    {
        private static readonly int _space = 3;
        private static readonly string _gazeModifierSettingsAssetPath = PathHelper.PathCombine("Resources", typeof(GazeModifierSettings).Name + ".asset");
        private readonly GazeModifierSettings _gazeModifierSettings;
        private TobiiXR_Settings _tobiiSettings;

        public GazeModifierSettingsExtension()
        {
            _gazeModifierSettings = LoadOrCreateDefaultConfiguration();
        }

        public int Priority
        {
            get
            {
                return 100;
            }
        }

        public void Init(TobiiXR_Settings settings)
        {
            _tobiiSettings = settings;
        }

        public void Render()
        {
            EditorGUILayout.LabelField("Gaze Modifier Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            GUILayout.Space(_space);
            GUILayout.BeginHorizontal();
            var enabled = GUILayout.Toggle(_tobiiSettings.EyeTrackingFilter is GazeModifierFilter, " Enabled");
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                _tobiiSettings.EyeTrackingFilterType = enabled ? typeof(GazeModifierFilter).AssemblyQualifiedName : null;
                Undo.RecordObject(_tobiiSettings, "Gaze Modifier enabled changed");
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(_tobiiSettings);
                AssetDatabase.SaveAssets();
            }

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Percentile:");
            var selectedPercentileIndex = (int)(GUILayout.HorizontalSlider(_gazeModifierSettings.SelectedPercentileIndex, 0,
                _gazeModifierSettings.NumberOfPercentiles - 1));
            EditorGUILayout.LabelField(_gazeModifierSettings.SelectedPercentileString, GUILayout.MaxWidth(50));

            GUILayout.EndHorizontal();

            GUILayout.Space(_space);
            EditorStyles.textArea.wordWrap = true;
            if (GUILayout.Button("Open Gaze Modifier documentation website"))
            {
                Application.OpenURL("https://vr.tobii.com/sdk/develop/unity/tools/gaze-modifier/");
            }
            GUILayout.Space(_space);

            EditorGUILayout.Separator();

            if (EditorGUI.EndChangeCheck())
            {
                _gazeModifierSettings.SelectedPercentileIndex = selectedPercentileIndex;
                Undo.RecordObject(_gazeModifierSettings, "Gaze Modifier settings changed");
                AssetDatabase.Refresh();
                EditorUtility.SetDirty(_gazeModifierSettings);
                AssetDatabase.SaveAssets();
            }
        }

        private static GazeModifierSettings LoadOrCreateDefaultConfiguration()
        {
            bool resourceExists;
            var settings = GazeModifierSettings.CreateDefaultSettings(out resourceExists);

            if (resourceExists) return settings;

            var sdkPath = Path.GetDirectoryName(PathHelper.FindPathToClass(typeof(GazeModifierSettings)));
            var filePath = PathHelper.PathCombine(sdkPath, _gazeModifierSettingsAssetPath);
            var assetPath = filePath.Replace(Application.dataPath, "Assets");

            Debug.Log(assetPath);

            if (File.Exists(filePath))
            {
                AssetDatabase.Refresh();
                settings = AssetDatabase.LoadAssetAtPath<GazeModifierSettings>(assetPath);
                return settings;
            }

            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();

            return settings;
        }
    }
}