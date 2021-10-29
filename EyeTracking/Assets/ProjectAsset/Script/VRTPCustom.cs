using Sigtrap.VrTunnellingPro;
using UnityEngine;
using Valve.VR;
using System;
using System.IO;

public class VRTPCustom : MonoBehaviour
{
    #region Parameters
    #region Gameobjects
    public GameObject _labModel;
    public GameObject _cameraEye;
    public GameObject _clliderModel;
    private Transform _cameraRig;
    private Tunnelling _tunnelling;
    #endregion

    #region Eye Tracking
    private EyeTrackingDataRecord _eyeData;
    public bool _useEyeTracking = false;
    #endregion

    #region physical velocity parameters
    private float _velSlew = 0.0f;
    public float _velocitySmoothing = 0.3f;
    private float _velSmoothed = 0.0f;
    public float _physicalCoverageMin = 0.5f;
    [Range(0f, 1f)]
    public float _velocityMin = 0.15f;
    [Range(0f, 1f)]
    public float _velocityMax = 0.3f;
    [Range(1f, 10f)]
    public float _velocityStrength = 1.0f;
    private float _physicalRestrictorSize;
    private Vector3 _lastPhysicalPos;
    private Vector3 _lastPhysicalRot;
    #endregion

    #region virtual velocity parameter
    private Vector3 _lastVirtualPos;
    private float _lastVirtualSpeed;
    private float _avSmoothedVirtual;
    private float _avSlewVirtual;
    private float _velSmoothedVirtual;
    private float _velSlewVirtual;
    private float _accelSmoothedVirtual;
    private float _accelSlewVirtual;
    private float _velocityMaxVirtual;
    private float _velocityMinVirtual = 0;
    private float _angularVelocityMaxVirtual;
    private float _angularVelocityMinVirtual = 0;
    private FixSpeedLocomotion _speedScript;
    #endregion

    #region cage
    public enum RestrictorMode { Physical, Virtual, Both };
    public RestrictorMode restrictorMode = RestrictorMode.Both;
    public enum MotionMode { Dynamic, Static };
    public MotionMode motionMode = MotionMode.Dynamic;
    public float _cageVanishSpeed = 0.01f;
    //private int _currentCage = 0;
    private bool use_cage = true;
    private bool previous_use_cage;
    // private bool cage_vanish = false;
    private float cage_size = 0.1f;
    private float transparencyLevel = 0f;
    private float _cageSlew;
    #endregion

    #region rayCasting
    public float _maxDistance = 3f;
    public float _minDistance = 0.5f;
    public float _distanceSlew = 0.0f;
    public float _distanceSmoothing = 0.1f;
    public float _distanceSmoothed = 0.0f;
    private int layerMask = 1 << 9;
    private int rayNum = 3;
    private Vector3 origin;
    [Range(0f, 1f)]
    private float sphereRadius = 0f;
    private Vector3[] rayDirection;
    private float[] distances;
    #endregion

    #region restrictor adjust
    public float restrictorChangePercent = 0.05f;
    private float effectCoverageDeadZone = 0.4f;
    private bool padClicked = false;
    #endregion

    #region debug
    public bool debug3Dmodel = false;
    public bool debugMessages;
    #endregion

    #endregion

    #region initial function
    void Awake()
    {

        _tunnelling = _cameraEye.GetComponent<Tunnelling>();
        if (_useEyeTracking)
            _eyeData = GetComponentInChildren<EyeTrackingDataRecord>();
        _cameraRig = _tunnelling.motionTarget;

        _lastPhysicalPos = _cameraEye.transform.localPosition;
        _lastPhysicalRot = _cameraEye.transform.localEulerAngles;

        // initial value
        //_tunnelling.effectCoverage_x = 0.75f;
        //_tunnelling.effectCoverage_y = 0.75f;
        _physicalRestrictorSize = _physicalCoverageMin;

        _speedScript = _cameraRig.GetComponent<FixSpeedLocomotion>();
        _velocityMaxVirtual = _speedScript.velocity.maxSpeed;
        _angularVelocityMaxVirtual = _speedScript.angularVelocity.maxSpeed;
    }
    #endregion


    #region update function
    void LateUpdate()
    {
        if (debug3Dmodel)
        {
            _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.CAGE_COLOR;
            _tunnelling.showScalarRestrictor = true;
            _tunnelling.scalarRestrictorSize = new Vector2(1.0f / RemapRadius(_physicalCoverageMin, _tunnelling.effectCoverage_x), 1.0f / RemapRadius(_physicalCoverageMin, _tunnelling.effectCoverage_y));
        }
        //else if (SteamVR_Actions.default_padClick.GetState(SteamVR_Input_Sources.Any))
        //OnPadClicked();
        else
        {
            padClicked = false;
            RestrictorUpdate();
        }
        //if (SteamVR_Actions.default_GrabGrip.GetState(SteamVR_Input_Sources.Any))
        //OnGripClicked();
    }
    #endregion

    #region restrictor update
    private void RestrictorUpdate()
    {
        // set restrictor size
        _tunnelling.showScalarRestrictor = true;
        (Vector2 virtualMotion, float angularMotion) = CalculateVirtualMotion();
        if (restrictorMode == RestrictorMode.Virtual)
        {
            if (motionMode == MotionMode.Dynamic)
            {
                _tunnelling.scalarRestrictorSize.x = virtualMotion.x;
                _tunnelling.scalarRestrictorSize.y = virtualMotion.y;
                if (angularMotion > 0.01 || angularMotion < -0.01)
                    _tunnelling.effectCoverageOffset_x = Math.Sign(angularMotion);
                else
                    _tunnelling.effectCoverageOffset_x = 0;
            }
            _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.COLOR;
            _tunnelling.transparencyLevel = 1.0f;
        }
        else if (restrictorMode == RestrictorMode.Physical)
        {
            _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.CAGE_COLOR;
            _tunnelling.showScalarRestrictor = false;
        }
        else
        {
            // check if move physically
            float physical_speed = CalculatePhysicalMotion(Time.deltaTime);
            use_cage = physical_speed > _physicalCoverageMin;

            if (use_cage)
            {
                // check the distance from obstacles
                CheckDistanceToCollider();
                _tunnelling.scalarRestrictorSize.x = Mathf.Max(virtualMotion.x, _physicalRestrictorSize);
                _tunnelling.scalarRestrictorSize.y = Mathf.Max(virtualMotion.y, _physicalRestrictorSize);
                _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.CAGE_COLOR;
                _tunnelling.transparencyLevel = transparencyLevel;
                cage_size = _physicalRestrictorSize;
            }
            else if (virtualMotion.x >= cage_size || virtualMotion.y >= cage_size)
            {
                _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.COLOR;
                _tunnelling.transparencyLevel = 1.0f;
                cage_size = _physicalCoverageMin;
                //cage_vanish = false;
                _tunnelling.scalarRestrictorSize.x = Mathf.Max(virtualMotion.x, cage_size);
                _tunnelling.scalarRestrictorSize.y = Mathf.Max(virtualMotion.y, cage_size);
            }
            else if (cage_size > _physicalCoverageMin)
            {
                cage_size = Mathf.SmoothDamp(cage_size, _physicalCoverageMin, ref _cageSlew, 0.2f);
                _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.CAGE_COLOR;
                _tunnelling.scalarRestrictorSize = new Vector2(cage_size, cage_size);
                _tunnelling.transparencyLevel = transparencyLevel;
            }

            /*
            else
            {
                // cage vanish smoothly

                if (previous_use_cage)
                {
                    cage_vanish = true;
                    cage_size = _physicalRestrictorSize;
                }
                else if (cage_vanish && (cage_size > _physicalCoverageMin))
                {
                    cage_size -= _cageVanishSpeed;
                }
                else if (cage_vanish && (cage_size <= _physicalCoverageMin))
                    cage_vanish = false;

            if (virtualMotion.x >= cage_size || virtualMotion.y >= cage_size)
                {
                    _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.COLOR;
                    _tunnelling.transparencyLevel = 1.0f;
                    cage_size = _physicalCoverageMin;
                    //cage_vanish = false;
                    _tunnelling.scalarRestrictorSize.x = Mathf.Max(virtualMotion.x, cage_size);
                    _tunnelling.scalarRestrictorSize.y = Mathf.Max(virtualMotion.y, cage_size);
                }
                else if (cage_size > _physicalCoverageMin)
                {
                    cage_size = Mathf.SmoothDamp(cage_size, _physicalCoverageMin, ref _cageSlew, 0.3f);
                    _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.CAGE_COLOR;
                    _tunnelling.scalarRestrictorSize = new Vector2(cage_size, cage_size);
                    _tunnelling.transparencyLevel = transparencyLevel;
                }
            }
            previous_use_cage = use_cage;
            */
        }

        if (_useEyeTracking && motionMode == MotionMode.Dynamic && virtualMotion.x > effectCoverageDeadZone && virtualMotion.y > effectCoverageDeadZone)
        {
            _tunnelling.effectOriginX = _eyeData.gazeDirection.x / _eyeData.gazeDirection.z;
            _tunnelling.effectOriginY = _eyeData.gazeDirection.y / _eyeData.gazeDirection.z;
        }
        else
        {
            _tunnelling.effectOriginX = 0;
            _tunnelling.effectOriginY = 0;
        }
    }
    #endregion


    #region ray casting
    private void CheckDistanceToCollider()
    {
        rayDirection = new Vector3[rayNum];
        distances = new float[rayNum];
        origin = _cameraEye.transform.position + new Vector3(0f, -1f, 0f);
        rayDirection[0] = _cameraEye.transform.forward;
        rayDirection[1] = _cameraEye.transform.right;
        rayDirection[2] = -1f * _cameraEye.transform.right;
        int i = 0;
        foreach (Vector3 direction in rayDirection)
        {
            RaycastHit hit;
            if (Physics.SphereCast(origin, sphereRadius, direction, out hit, _maxDistance, layerMask) && (hit.collider != null))
                distances[i] = hit.distance;
            else
                distances[i] = _maxDistance;
            i++;
        }
        float realTimeDistance = _maxDistance;
        foreach (float distance in distances)
        {
            realTimeDistance = Mathf.Min(realTimeDistance, distance);
        }
        // smooth restrictor size changing
        _distanceSmoothed = Mathf.SmoothDamp(_distanceSmoothed, realTimeDistance, ref _distanceSlew, _distanceSmoothing);

        // Check for divide-by-zero
        if (!Mathf.Approximately(_maxDistance, _minDistance))
        {
            realTimeDistance = Mathf.Clamp01((_distanceSmoothed - _minDistance) / (_maxDistance - _minDistance));
        }
        _physicalRestrictorSize = Mathf.Clamp01(RemapRadius(_physicalCoverageMin, 1.0f - realTimeDistance));
        transparencyLevel = Mathf.Lerp(0, 1, 1.0f - realTimeDistance);
        if (debugMessages)
            Debug.Log("distance: forward " + distances[0] + " left " + distances[1] + " right " + distances[2]);
    }

    /*
    private void OnDrawGizmosSelected() {
        int j = 0;
        foreach (Vector3 direction in rayDirection)
        {
            Gizmos.color = Color.red;
            Debug.DrawLine(origin, origin + direction * distances[j]);
            Gizmos.DrawWireSphere(origin + direction * distances[j], sphereRadius);
            j++;
        }
    }
    */
    #endregion

    #region virtual speed
    protected (Vector2, float) CalculateVirtualMotion()
    {
        float fx = 0;   // Total effect strength from all motion types
        Vector2 motion;


        // Rotation
        float angularMotion = 0;
        if (_tunnelling.useAngularVelocity)
        {
            float av = 0;
            // Check for divide-by-zero
            if (!Mathf.Approximately(_angularVelocityMaxVirtual, _angularVelocityMinVirtual))
            {
                _avSmoothedVirtual = Mathf.SmoothDamp(_avSmoothedVirtual, _speedScript.angularVelocity.speed, ref _avSlewVirtual, _tunnelling.angularVelocitySmoothing);
                av = Mathf.Clamp01((Mathf.Abs(_avSmoothedVirtual) - _angularVelocityMinVirtual) / (_angularVelocityMaxVirtual - _angularVelocityMinVirtual));
            }
            //av = (_cameraRig.GetComponent<FixSpeedLocomotion>().turnAngle - _angularVelocityMinVirtual) / (_angularVelocityMaxVirtual - _angularVelocityMinVirtual);
            //_avSmoothedVirtual = Mathf.SmoothDamp(_avSmoothedVirtual, av, ref _avSlewVirtual, _tunnelling.angularVelocitySmoothing);
            fx += av * _tunnelling.angularVelocityStrength;
            angularMotion = _avSmoothedVirtual;
        }

        // Velocity
        if (_tunnelling.useVelocity)
        {
            float vel = 0;
            // Check for divide-by-zero
            if (!Mathf.Approximately(_velocityMaxVirtual, _velocityMinVirtual))
            {
                _velSmoothedVirtual = Mathf.SmoothDamp(_velSmoothedVirtual, _speedScript.velocity.speed, ref _velSlewVirtual, _tunnelling.velocitySmoothing);
                vel = Mathf.Clamp01((Mathf.Abs(_velSmoothedVirtual) - _velocityMinVirtual) / (_velocityMaxVirtual - _velocityMinVirtual));
            }
            fx += vel * _tunnelling.velocityStrength;
        }
        // Clamp and scale final effect strength
        float fy = RemapRadius(effectCoverageDeadZone, fx);
        fx = RemapRadius(effectCoverageDeadZone, fx);

        // Cache current motion params for next frame
        _lastVirtualPos = _cameraRig.position;

        motion.x = Mathf.Clamp01(fx);
        motion.y = Mathf.Clamp01(fy);

        return (motion, angularMotion);
    }
    #endregion

    #region phsical speed calculation
    private float CalculatePhysicalMotion(float dT)
    {
        float motion;
        float fx = 0.0f;

        // Velocity
        float vel = 0;
        if (_tunnelling.useVelocity)
        {
            vel = Mathf.Abs(Vector3.Distance(_cameraEye.transform.localPosition, _lastPhysicalPos) / dT);
            //if (debugMessages)
            //Debug.Log("vel: " + vel);
            _velSmoothed = Mathf.SmoothDamp(_velSmoothed, vel, ref _velSlew, _velocitySmoothing);
            float lm = 0;

            // Check for divide-by-zero
            if (!Mathf.Approximately(_velocityMax, _velocityMin))
            {
                lm = Mathf.Clamp01((_velSmoothed - _velocityMin) / (_velocityMax - _velocityMin));
            }
            //if (debugMessages)
            //Debug.Log("vel： "+ vel);
            fx += lm * _velocityStrength;
        }
        // Cache current motion params for next frame
        _lastPhysicalPos = _cameraEye.transform.localPosition;
        motion = Mathf.Clamp01(RemapRadius(_physicalCoverageMin, fx) * RemapRadius(_physicalCoverageMin, _physicalRestrictorSize));
        //if (debugMessages)
        //Debug.Log("physical speed: " + motion);
        return motion;
    }
    #endregion

    #region utilities
    private float RemapRadius(float min, float x)
    {
        x = Mathf.Lerp(min, 1, x);
        return x;
    }
    #endregion


    #region controller settings
    /*
    private void OnPadClicked()
    {
        if (!padClicked)
        {
            _tunnelling.backgroundMode = (restrictorMode == RestrictorMode.Physical) ? TunnellingBase.BackgroundMode.CAGE_COLOR : TunnellingBase.BackgroundMode.COLOR;
            _tunnelling.showScalarRestrictor = true;
            _tunnelling.transparencyLevel = 1.0f;
            padClicked = true;
            Vector2 touchpad = SteamVR_Actions.default_touchPad.GetAxis(SteamVR_Input_Sources.Any);
            //if (debugMessages)
            Debug.Log("x:" + touchpad.x + " y: " + touchpad.y);

            if (touchpad.y > 0.5f)
            {
                _tunnelling.effectCoverage_y -= restrictorChangePercent;
                _tunnelling.effectCoverage_y = Mathf.Max(_tunnelling.effectCoverage_y, 0.0f);
                _tunnelling.scalarRestrictorSize = new Vector2(1.0f, 1.0f);
                if (debugMessages)
                    Debug.Log("effectCoverage y:" + _tunnelling.effectCoverage_y);
            }
            else if (touchpad.y < -0.5f)
            {
                _tunnelling.effectCoverage_y += restrictorChangePercent;
                _tunnelling.effectCoverage_y = Mathf.Min(_tunnelling.effectCoverage_y, 1.0f);
                _tunnelling.scalarRestrictorSize = new Vector2(1.0f, 1.0f);
                if (debugMessages)
                    Debug.Log("effectCoverage y:" + _tunnelling.effectCoverage_y);
            }
            else if (touchpad.x > 0.5f)
            {
                _tunnelling.effectCoverage_x -= restrictorChangePercent;
                _tunnelling.effectCoverage_x = Mathf.Max(_tunnelling.effectCoverage_x, 0.0f);
                _tunnelling.scalarRestrictorSize = new Vector2(1.0f, 1.0f);
                if (debugMessages)
                    Debug.Log("effectCoverage x:" + _tunnelling.effectCoverage_x);
            }
            else if (touchpad.x < -0.5f)
            {
                _tunnelling.effectCoverage_x += restrictorChangePercent;
                _tunnelling.effectCoverage_x = Mathf.Min(_tunnelling.effectCoverage_x, 1.0f);
                _tunnelling.scalarRestrictorSize = new Vector2(1.0f, 1.0f);
                if (debugMessages)
                    Debug.Log("effectCoverage x:" + _tunnelling.effectCoverage_x);
            }
        }
    }*/
    /*
    private void OnGripClicked()
    {
        
        _labModel.transform.localEulerAngles = new Vector3(0f, -80f, 0f);  //new Vector3(0f, -120f, 0f);
        _labModel.transform.localPosition = _cameraEye.transform.localPosition + new Vector3(-1f, -1.5f, 3.1f); // new Vector3(-2.5f, -1.5f, 0.5f);
        _tunnelling.backgroundMode = TunnellingBase.BackgroundMode.CAGE_COLOR;
        _tunnelling.showScalarRestrictor = true;
        _tunnelling.scalarRestrictorSize = new Vector2(1.0f / RemapRadius(_physicalCoverageMin, _tunnelling.effectCoverage_x), 1.0f / RemapRadius(_physicalCoverageMin, _tunnelling.effectCoverage_y));
        _tunnelling.transparencyLevel = 1.0f;

        _clliderModel.transform.localEulerAngles = new Vector3(270f, -80f, 0f);
        _clliderModel.transform.localPosition = _labModel.transform.localPosition;
        
    }*/
    #endregion
}