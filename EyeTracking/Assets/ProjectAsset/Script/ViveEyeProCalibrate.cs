using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;
using Tobii.XR;

public class ViveEyeProCalibrate : MonoBehaviour
{
    [Tooltip("Should the system do eye tracking calibration on awake?")]
    public bool calibrate_on_awake = true;

    private delegate void d();

    void Awake()
    {
        if (calibrate_on_awake)
        {
            int error = 1;
            while (error != 0)
            {
                error = SRanipal_Eye_API.LaunchEyeCalibration(Marshal.GetFunctionPointerForDelegate(new d(CalibrationCallback)));
                if (error != 0)
                    Debug.Log("Calibration Error: " + error);
            }
        }
        
    }

    void CalibrationCallback()
    {
        Debug.Log("Calibration Finished");
    }
}
