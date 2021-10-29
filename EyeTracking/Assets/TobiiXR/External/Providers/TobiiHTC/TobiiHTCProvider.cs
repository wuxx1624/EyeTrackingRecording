// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using System;
using Tobii.XR;
using UnityEngine;

/// <summary>
/// Provider that, at runtime, chooses whether to use HTC provider or Tobii provider for eye tracking data
/// </summary>
[CompilerFlag("TOBIIXR_HTCPROVIDER"), ProviderDisplayName("Tobii or HTC Provider"), SupportedPlatform(XRBuildTargetGroup.Standalone)]
public class TobiiHTCProvider : IEyeTrackingProvider
{
    private IEyeTrackingProvider _selectedProvider;

    public TobiiXR_EyeTrackingData EyeTrackingData { get { return _selectedProvider.EyeTrackingData; } }
    public Matrix4x4 LocalToWorldMatrix { get { return _selectedProvider.LocalToWorldMatrix; } }

    public TobiiHTCProvider()
    {
#if TOBIIXR_HTCPROVIDER
        if (ViveSR.anipal.Eye.SRanipal_Eye_API.IsViveProEye())
        {
            _selectedProvider = new HTCProvider();
            return;
        }
#endif

        var model = UnityEngine.XR.XRDevice.model.Replace(".", "").ToLower();
        if (model.Contains("vive"))
        {
            if (model.Equals("vive mv")) // Old HTC Vive only has retrofitted eye tracking
            {
                _selectedProvider = new TobiiProvider();
            }
            else // All other Vive models use HTC eye tracking
            {
                _selectedProvider = new HTCProvider();
            }
        }
        else // Fall back to Tobii Provider for unknown models
        {
            _selectedProvider = new TobiiProvider();
        }
    }

    public void Tick()
    {
        _selectedProvider.Tick();
    }

    public void Destroy()
    {
        _selectedProvider.Destroy();
    }
}
