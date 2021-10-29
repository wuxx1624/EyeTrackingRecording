// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using UnityEngine;

namespace Tobii.XR
{
    /// <summary>
    /// Uses Tobii's Stream Engine library to provide eye tracking data to TobiiXR
    /// </summary>
    [ProviderDisplayName("Tobii Eye Tracking Provider")]
    public class TobiiProvider : IEyeTrackingProvider
    {
        private StreamEngineTracker _streamEngineTracker;
        private HmdToWorldTransformer _hmdToWorldTransformer;
        private readonly TobiiXR_EyeTrackingData _eyeTrackingData = new TobiiXR_EyeTrackingData();
        private Matrix4x4 _localToWorldMatrix;

        public Matrix4x4 LocalToWorldMatrix { get { return _localToWorldMatrix; } }

        public TobiiXR_EyeTrackingData EyeTrackingData
        {
            get { return _eyeTrackingData; }
        }

        public TobiiProvider()
        {
            _streamEngineTracker = new StreamEngineTracker();
            _hmdToWorldTransformer = new HmdToWorldTransformer(estimatedEyeTrackerLatency_s: 0.012f);
        }

        public TobiiProvider(StreamEngineTracker streamEngineTracker, HmdToWorldTransformer hmdToWorldTransformer)
        {
            _streamEngineTracker = streamEngineTracker;
            _hmdToWorldTransformer = hmdToWorldTransformer;
        }

        public void Tick()
        {
            _streamEngineTracker.Tick();
            _hmdToWorldTransformer.Tick();

            var data = _streamEngineTracker.LocalLatestData;

            _eyeTrackingData.Timestamp = Time.unscaledTime;
            _eyeTrackingData.GazeRay = data.GazeRay;
            _eyeTrackingData.IsLeftEyeBlinking = data.IsLeftEyeBlinking;
            _eyeTrackingData.IsRightEyeBlinking= data.IsRightEyeBlinking;
            _eyeTrackingData.ConvergenceDistance = data.ConvergenceDistance;
            _eyeTrackingData.ConvergenceDistanceIsValid= data.ConvergenceDistanceIsValid;

            _localToWorldMatrix = _hmdToWorldTransformer.GetLocalToWorldMatrix();
            EyeTrackingDataHelper.TransformGazeData(_eyeTrackingData, _localToWorldMatrix);
        }

        public void Destroy()
        {
            if (_streamEngineTracker != null)
            {
                _streamEngineTracker.Destroy();
                _streamEngineTracker = null;
            }
        }
    }
}
