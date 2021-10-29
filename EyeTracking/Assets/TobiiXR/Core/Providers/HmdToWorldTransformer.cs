// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

namespace Tobii.XR
{
    public class HmdToWorldTransformer
    {
        struct HeadPoseSample
        {
            public long timestamp_us;
            public Matrix4x4 matrix;
        }

        private const int _estimatedPrediction_us = 33000;
        private readonly float _estimatedEyeTrackerLatency_s;
        private readonly int _headPoseDelayInFrames;
        private readonly HeadPoseSample[] _history;
        private readonly bool _useOpenVR = false;
        private Transform _cameraTransform;
        private int _writeIndex = 0;
        private OpenVRManager _openVRManager = new OpenVRManager();

        public Transform CameraTransform { get { return _cameraTransform; } }

        public HmdToWorldTransformer(float estimatedEyeTrackerLatency_s)
        {
            _estimatedEyeTrackerLatency_s = estimatedEyeTrackerLatency_s;
            var latency_s = (_estimatedPrediction_us / 1000000f) + estimatedEyeTrackerLatency_s;
            var frameLength_s = 1 / (XRDevice.refreshRate > 1 ? XRDevice.refreshRate : 90);
            _headPoseDelayInFrames = Mathf.CeilToInt(latency_s / frameLength_s);
            _history = new HeadPoseSample[_headPoseDelayInFrames + 1];
            _cameraTransform = CameraHelper.GetCameraTransform();
            _useOpenVR = OpenVRManager.IsAvailable();
        }

        public void Tick()
        {
            _history[_writeIndex].timestamp_us = Stopwatch.GetTimestamp() / 10 + _estimatedPrediction_us;
            _history[_writeIndex].matrix = GetCameraLocalToWorldMatrix();
            _writeIndex = (_writeIndex + 1) % _history.Length;
        }

        public Matrix4x4 GetLocalToWorldMatrix()
        {
            var centerEyeToHeadOffsetTransform = Matrix4x4.Translate(InputTracking.GetLocalPosition(XRNode.Head) - InputTracking.GetLocalPosition(XRNode.CenterEye));

            if (_useOpenVR)
            {
                var pos = InputTracking.GetLocalPosition(XRNode.CenterEye);
                var rot = InputTracking.GetLocalRotation(XRNode.CenterEye);
                var cameraToHmdOffsetTransform = GetCameraLocalToWorldMatrix() * Matrix4x4.TRS(pos, rot, Vector3.one).inverse;
                var headPose = _openVRManager.GetHeadPoseFor(secondsAgo: _estimatedEyeTrackerLatency_s);
                return centerEyeToHeadOffsetTransform * (cameraToHmdOffsetTransform * headPose);
            }
            else
            {
                var sample = _history[(_history.Length + _writeIndex - 1 - _headPoseDelayInFrames) % _history.Length];
                if (sample.timestamp_us == 0) return GetCameraLocalToWorldMatrix();
                return centerEyeToHeadOffsetTransform * sample.matrix;
            }
        }

        private Matrix4x4 GetCameraLocalToWorldMatrix()
        {
            if (_cameraTransform != null && _cameraTransform.gameObject.activeInHierarchy)
            {
                return _cameraTransform.localToWorldMatrix;
            }

            UnityEngine.Debug.Log("Camera transform invalid. Trying to retrieve a new.");
            _cameraTransform = CameraHelper.GetCameraTransform();
            return _cameraTransform != null ? _cameraTransform.localToWorldMatrix : Matrix4x4.identity;
        }
    }
}