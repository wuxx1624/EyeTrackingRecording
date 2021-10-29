// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using Tobii.XR;
using UnityEngine;

#if TOBIIXR_HTCPROVIDER
using ViveSR.anipal.Eye;
#endif

/// <summary>
/// Provides eye tracking data to TobiiXR using HTC's SR Anipal SDK. 
/// SR Anipal needs to be downloaded from https://hub.vive.com/en-US/profile/material-download and added to the project separately. 
/// Tested with SR Anipal version 1.1.0.1
/// </summary>
[CompilerFlag("TOBIIXR_HTCPROVIDER"), ProviderDisplayName("HTC Provider"), SupportedPlatform(XRBuildTargetGroup.Standalone)]
public class HTCProvider : IEyeTrackingProvider
{
    private Matrix4x4 _localToWorldMatrix = Matrix4x4.identity;

    public TobiiXR_EyeTrackingData EyeTrackingData { get; private set; }

    public Matrix4x4 LocalToWorldMatrix { get { return _localToWorldMatrix; } }

#if TOBIIXR_HTCPROVIDER
    private GameObject _htcGameObject;
    private HmdToWorldTransformer _hmdToWorldTransformer;

    public HTCProvider()
    {
        EyeTrackingData = new TobiiXR_EyeTrackingData();
        _hmdToWorldTransformer = new HmdToWorldTransformer(estimatedEyeTrackerLatency_s: 0.040f);
        EnsureHTCFrameworkRunning();
    }

    private void EnsureHTCFrameworkRunning()
    {
        if (_htcGameObject != null) return;
        _htcGameObject = new GameObject("HTC")
        {
            hideFlags = HideFlags.HideInHierarchy
        };
        _htcGameObject.AddComponent<SRanipal_Eye_Framework>();
    }
#endif

    public void Tick()
    {
#if TOBIIXR_HTCPROVIDER
        _hmdToWorldTransformer.Tick();
        EnsureHTCFrameworkRunning();

        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

        EyeTrackingData.Timestamp = Time.unscaledTime;
        EyeTrackingData.GazeRay.IsValid = SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out EyeTrackingData.GazeRay.Origin, out EyeTrackingData.GazeRay.Direction);

        // Blink
        float eyeOpenness = 0;
        var eyeOpennessIsValid = SRanipal_Eye.GetEyeOpenness(EyeIndex.LEFT, out eyeOpenness);
        EyeTrackingData.IsLeftEyeBlinking = !eyeOpennessIsValid || eyeOpenness < 0.1;
        eyeOpennessIsValid = SRanipal_Eye.GetEyeOpenness(EyeIndex.RIGHT, out eyeOpenness);
        EyeTrackingData.IsRightEyeBlinking = !eyeOpennessIsValid || eyeOpenness < 0.1;

        // Convergence distance
        Vector3 leftRayOrigin, rightRayOrigin, leftRayDirection, rightRayDirection;
        var leftRayValid = SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out leftRayOrigin, out leftRayDirection);
        var rightRayValid = SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out rightRayOrigin, out rightRayDirection);

        if (leftRayValid && rightRayValid)
        {
            EyeTrackingData.ConvergenceDistanceIsValid = true;
            var convergenceDistance_mm = Convergence.CalculateDistance(
                        leftRayOrigin * 1000f,
                        leftRayDirection,
                        rightRayOrigin * 1000f,
                        rightRayDirection
                        );
            EyeTrackingData.ConvergenceDistance = convergenceDistance_mm / 1000f; // Convert to meters
        }
        else
        {
            EyeTrackingData.ConvergenceDistanceIsValid = false;
        }

        // Transform to world space
        _localToWorldMatrix = _hmdToWorldTransformer.GetLocalToWorldMatrix();
        EyeTrackingDataHelper.TransformGazeData(EyeTrackingData, _localToWorldMatrix);
#endif
    }

    public void Destroy()
    {
#if TOBIIXR_HTCPROVIDER
        GameObject.Destroy(_htcGameObject);
        _htcGameObject = null;
#endif
    }
}
