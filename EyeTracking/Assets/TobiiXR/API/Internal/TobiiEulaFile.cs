namespace Tobii.XR
{
    public static class TobiiEulaFile
    {
        private static bool _eulaAccepted;

        public static bool IsEulaAccepted()
        {
#if !UNITY_EDITOR
            if (_eulaAccepted)
            {
                return true;
            }
#endif

            var settings = TobiiXR_Settings.LoadDefaultSettings();

            if (settings != null)
            {
                _eulaAccepted = settings.TobiiSDKEulaAccepted;
            }

            return _eulaAccepted;
        }

#if UNITY_EDITOR

        public static void SetEulaAccepted()
        {
            var settings = TobiiXR_Settings.LoadDefaultSettings();
            settings.TobiiSDKEulaAccepted = true;

            UnityEditor.EditorUtility.SetDirty(settings);
            UnityEditor.AssetDatabase.SaveAssets();
        }

#endif
    }
}