// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using UnityEngine;

namespace Tobii.XR
{
    /// <summary>
    /// Provides emulated gaze data to TobiiXR using camera position and rotation
    /// </summary>
    [ProviderDisplayName("Nose Direction Provider")]
    public class NoseDirectionProvider : IEyeTrackingProvider
    {
        private Transform _hmdOrigin;
        private EyeTrackingDataHelper _dataHelper = new EyeTrackingDataHelper();
        private readonly TobiiXR_EyeTrackingData _eyeTrackingData = new TobiiXR_EyeTrackingData();

        public Matrix4x4 LocalToWorldMatrix { get { return _hmdOrigin.localToWorldMatrix; } }

        public TobiiXR_EyeTrackingData EyeTrackingData
        {
            get { return _eyeTrackingData; }
        }
        
        public NoseDirectionProvider()
        {
            _hmdOrigin = CreateNewOrigin(GetType().Name);
        }

        public void Tick()
        {
            if (_hmdOrigin == null)
            {
                _hmdOrigin = CreateNewOrigin(GetType().Name);
            }

            _dataHelper.SetAllGazeRays(_eyeTrackingData, _hmdOrigin.position, _hmdOrigin.forward, true);
        }

        public void Destroy()
        {
            if (_hmdOrigin == null) return;

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(_hmdOrigin.gameObject);
            }
            else
            {
                Object.DestroyImmediate(_hmdOrigin.gameObject);
            }
#else
            Object.Destroy(_hmdOrigin.gameObject);
#endif
            _hmdOrigin = null;
        }

        private static Transform CreateNewOrigin(string name)
        {
            var parent = CameraHelper.GetCameraTransform();
            var hmdOrigin = new GameObject(string.Format("HmdOrigin_{0}", name)).transform;
            hmdOrigin.parent = parent;
            hmdOrigin.transform.localPosition = Vector3.zero;
            hmdOrigin.transform.localScale = Vector3.one;
            hmdOrigin.transform.localRotation = Quaternion.identity;

            return hmdOrigin;
        }
    }
}