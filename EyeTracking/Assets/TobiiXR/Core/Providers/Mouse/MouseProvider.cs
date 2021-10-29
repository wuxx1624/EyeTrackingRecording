// Copyright © 2019 – Property of Tobii AB (publ) - All Rights Reserved

using UnityEngine;

namespace Tobii.XR
{
    /// <summary>
    /// Provides emulated gaze data to TobiiXR using the mouse. 
    /// Note: This provider adds a child camera to the main camera that will render on top. 
    /// Only use this provider for debugging in Unity Editor.
    /// </summary>
    [ProviderDisplayName("Mouse Provider")]
    public class MouseProvider : IEyeTrackingProvider
    {
        private EyeTrackingDataHelper _dataHelper = new EyeTrackingDataHelper();
        private readonly TobiiXR_EyeTrackingData _eyeTrackingData = new TobiiXR_EyeTrackingData();

        private static Camera _mouseProviderCamera;

        public Matrix4x4 LocalToWorldMatrix { get { return _mouseProviderCamera.transform.localToWorldMatrix; } }

        public TobiiXR_EyeTrackingData EyeTrackingData
        {
            get { return _eyeTrackingData; }
        }

        public void Tick()
        {
            if (_mouseProviderCamera == null)
            {
                _mouseProviderCamera = GetMouseProviderCamera(GetType().Name);
            }

            var mouseRay = _mouseProviderCamera.ScreenPointToRay(Input.mousePosition);

            _dataHelper.SetAllGazeRays(_eyeTrackingData, mouseRay.origin,mouseRay.direction,true);
        }

        public void Destroy()
        {
            if (_mouseProviderCamera == null) return;

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(_mouseProviderCamera.gameObject);
            }
            else
            {
                Object.DestroyImmediate(_mouseProviderCamera.gameObject);
            }
#else
            Object.Destroy(_mouseProviderCamera.gameObject);
#endif
            _mouseProviderCamera = null;
        }

        // TODO We should not create extra camera when mouse position issue is fixed in Unity
        // https://issuetracker.unity3d.com/issues/screenpointtoray-is-offset-when-used-in-vr-with-openvr-sdk
        private static Camera GetMouseProviderCamera(string name)
        {
            var parent = CameraHelper.GetCameraTransform();
            var camera = new GameObject(string.Format("{0} Camera", name)).AddComponent<Camera>();
            camera.transform.parent = parent;
            camera.transform.localPosition = Vector3.zero;
            camera.transform.localScale = Vector3.one;
            camera.transform.localRotation = Quaternion.identity;

            camera.stereoTargetEye = StereoTargetEyeMask.None;

            return camera;
        }
    }
}
