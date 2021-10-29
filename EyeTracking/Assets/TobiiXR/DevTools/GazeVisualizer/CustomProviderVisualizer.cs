// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using UnityEngine;

namespace Tobii.XR.GazeVisualizer
{

    [RequireComponent(typeof(GazeVisualizer))]
    public class CustomProviderVisualizer : MonoBehaviour
    {
        [HideInInspector]
        public string EyeTrackingProvider;
        private GazeVisualizer _gazeVisualizer;

        void Start()
        {
            _gazeVisualizer = GetComponent<GazeVisualizer>();
        }

        void Update()
        {
            if (_gazeVisualizer.EyetrackingProvider == null) return;
            _gazeVisualizer.EyetrackingProvider.Tick();
        }

        public void ChangeProvider(string newProvider)
        {
            if (newProvider != EyeTrackingProvider)
            {
                EyeTrackingProvider = newProvider;
                _gazeVisualizer.EyetrackingProvider = TobiiXR_Settings.GetProvider(AssemblyUtils.EyetrackingProviderType(EyeTrackingProvider));
            }
        }
    }
}