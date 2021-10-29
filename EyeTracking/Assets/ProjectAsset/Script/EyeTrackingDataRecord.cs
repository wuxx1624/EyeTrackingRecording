using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.XR;
using Valve.VR;
public class EyeTrackingDataRecord : MonoBehaviour
{
    [Header("Game Object")]
    public Transform cameraEye;
    public GameObject experimentManager;
    private Transform cross;
    private VRTPCustom vrtpCustom;
    private CSVLogger logger;

    // Eye tracking Data
    private TobiiXR_EyeTrackingData eyeData;
    private Vector3 gazeOrigin;
    private float convergenceDistance;
    // Save last good data.
    private Vector3 _lastGoodOrigin;
    private Vector3 _lastGoodDirection;
    private Vector3 _previousDirection;
    private Transform _cameraTransform;
    private float _lastConvergenceDistance;
    // Running animation values.
    private Vector3 _smoothDampVelocity;

    [Header("Eye Behaviour Values")]
    public Vector3 gazeDirection = new Vector3(0,0,1);
    [Range(0, 1)]
    public float _gazeDirectionSmoothTime = 0.2f;
    private bool release = true;
    private float crossScale = 1.0f;
    public bool showCross = false;

    //public SteamVR_Action_Boolean buttonClick;
    //private bool defineLock = false;

    // Start is called before the first frame update
    void Start()
    {
        logger = GetComponent<CSVLogger>();
        logger.Initial();
        vrtpCustom = experimentManager.GetComponent<VRTPCustom>();
        // Cache the camera transform.
        _cameraTransform = CameraHelper.GetCameraTransform();
        cross = cameraEye.GetChild(0);
        if (showCross)
        {
            cross.gameObject.SetActive(true);
            crossScale = cross.localPosition.z;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get local copies.
        eyeData = TobiiXR.EyeTrackingData;

        // Get local transform direction.
        gazeOrigin = eyeData.GazeRay.Origin;
        gazeDirection = _cameraTransform.InverseTransformDirection(eyeData.GazeRay.Direction);
        
        if (!IsEyeDataGood(eyeData))
        {
            gazeOrigin = _lastGoodOrigin;
            gazeDirection = _lastGoodDirection;
        }
        
        _lastGoodOrigin = gazeOrigin;
        _lastGoodDirection = gazeDirection;

        convergenceDistance = eyeData.ConvergenceDistanceIsValid ? eyeData.ConvergenceDistance : _lastConvergenceDistance;
        _lastConvergenceDistance = convergenceDistance;

        // Record to file
        log();

        // Apply smoothing from previous frame to this one.
        gazeDirection = Vector3.SmoothDamp(_previousDirection, gazeDirection, ref _smoothDampVelocity, _gazeDirectionSmoothTime);
        _previousDirection = gazeDirection;


        // Debug
        /*
        bool gripClick = SteamVR_Actions.default_GrabGrip.GetState(SteamVR_Input_Sources.Any);
        if (gripClick && release)
        {
            Debug.Log(gazeOrigin);
            Debug.Log(gazeDirection);
            release = false;
        }
        else if (!gripClick && !release)
            release = true;
            */
        // Set the localPosition for cross
        if (cross.gameObject.activeSelf && vrtpCustom._useEyeTracking)
            cross.localPosition = new Vector3(gazeDirection.x * crossScale / gazeDirection.z, gazeDirection.y * crossScale / gazeDirection.z, crossScale);
    }

    void log()
    {
        logger.UpdateField("timestamp", eyeData.Timestamp.ToString());
        logger.UpdateField("trialId", GlobalControl.Instance.savedData.trial_id.ToString());
        logger.UpdateField("x_gazeDirection", gazeDirection.x.ToString());
        logger.UpdateField("y_gazeDirection", gazeDirection.y.ToString());
        logger.UpdateField("z_gazeDirection", gazeDirection.z.ToString());
        logger.UpdateField("x_Origin", gazeOrigin.x.ToString());
        logger.UpdateField("y_Origin", gazeOrigin.y.ToString());
        logger.UpdateField("z_Origin", gazeOrigin.z.ToString());
        logger.UpdateField("ConvergenceDistance", convergenceDistance.ToString());
        logger.UpdateField("LeftEyeBlink", eyeData.IsLeftEyeBlinking.ToString());
        logger.UpdateField("RightEyeBlink", eyeData.IsRightEyeBlinking.ToString());
        //logger.UpdateField("IsPeriphery", IsEyeFocusOnPeriphery().ToString());
    }

    /// <summary>
    /// Verifies whether the gaze ray can be used by checking its validity and the eye openness.
    /// </summary>
    /// <param name="eyeData">The eye to check validity and openness for.</param>
    /// <returns>True if ray is valid and eye is open.</returns>
    private static bool IsEyeDataGood(TobiiXR_EyeTrackingData eyeData)
    {
        return eyeData.GazeRay.IsValid && !eyeData.IsLeftEyeBlinking && !eyeData.IsRightEyeBlinking;
    }


    private bool IsEyeFocusOnPeriphery()
    {
        Vector2 gazePosition = new Vector2(gazeDirection.x / gazeDirection.z, gazeDirection.y / gazeDirection.z);
        Vector2 radius = new Vector2(1.0f - GlobalControl.Instance.savedData.effectCoverage.x, 1.0f - GlobalControl.Instance.savedData.effectCoverage.y);

        bool output = Mathf.Pow(gazePosition.x, 2) / Mathf.Pow(radius.x, 2) + Mathf.Pow(gazePosition.y, 2) / Mathf.Pow(radius.y, 2) > 1;
        /*
        if (buttonClick != null && buttonClick.GetState(SteamVR_Input_Sources.Any) && !defineLock)
        {
            if (output)
                Debug.Log("Periphery " + gazePosition + " " + radius);
            else
                Debug.Log("Center " + gazePosition + " " + radius);
            defineLock = true;
        }
        else if (buttonClick != null && !buttonClick.GetState(SteamVR_Input_Sources.Any))
        {
            defineLock = false;
        }
        */
        return output;
    }
}
