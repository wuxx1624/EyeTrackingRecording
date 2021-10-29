//Copyright © 2018 – Property of Tobii AB(publ) - All Rights Reserved

using UnityEngine;

namespace Tobii.XR
{
    public class EyeTrackingDataHelper
    {
        public void SetAllGazeRays(TobiiXR_EyeTrackingData data, Vector3 origin, Vector3 direction, bool isValid)
        {
            data.Timestamp = Time.unscaledTime;

            data.GazeRay.Origin = origin;
            data.GazeRay.Direction = direction.normalized;
            data.GazeRay.IsValid = isValid;
        }

        public static void Copy(TobiiXR_EyeTrackingData src, TobiiXR_EyeTrackingData dest)
        {
            dest.Timestamp = src.Timestamp;
            dest.GazeRay = src.GazeRay;
            dest.ConvergenceDistance = src.ConvergenceDistance;
            dest.ConvergenceDistanceIsValid = src.ConvergenceDistanceIsValid;
            dest.IsLeftEyeBlinking = src.IsLeftEyeBlinking;
            dest.IsRightEyeBlinking= src.IsRightEyeBlinking;
        }

        public static TobiiXR_EyeTrackingData Clone(TobiiXR_EyeTrackingData data)
        {
            var result = new TobiiXR_EyeTrackingData();
            Copy(data, result);
            return result;
        }

        public static void TransformGazeData(TobiiXR_EyeTrackingData eyeTrackingData, Matrix4x4 hmdOrigin)
        {
            if (eyeTrackingData.GazeRay.IsValid)
            {
                TransformToWorldSpace(ref eyeTrackingData.GazeRay, hmdOrigin);
            }
        }

        private static void TransformToWorldSpace(ref TobiiXR_GazeRay eyeData, Matrix4x4 hmdOrigin)
        {
            eyeData.Origin = hmdOrigin.MultiplyPoint(eyeData.Origin);
            eyeData.Direction = hmdOrigin.MultiplyVector(eyeData.Direction);
        }
    }
}
